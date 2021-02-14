using BlockGame.Backend;
using BlockGame.Components;
using Components.Player;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _movementVector = Vector3.zero;
    private Vector2 _lookVector = Vector2.zero;
    private Rigidbody _rigidbody;
    private readonly PlayerViewRaycaster _raycaster = new PlayerViewRaycaster();
    private WorldComponent _worldComponent;
    
    [SerializeField] public Camera playerCamera;
    [SerializeField] public Transform cameraAnchor;
    [SerializeField] public Transform focussedBlockCube;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _worldComponent = FindObjectOfType<WorldComponent>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleCamera();
        HandleMovement();
        HandleMouseInput();
    }

    private void HandleCamera ()
    {
        // Update camera position
        playerCamera.transform.position = cameraAnchor.transform.position;
        
        // Get relevant input for camera rotation
        _lookVector = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y"));
        _lookVector *= 7f;
        _lookVector.x *= 1.3f;

        // Get current rotation and modify it
        var currentPlayerRotation = playerCamera.transform.localRotation.eulerAngles;
        currentPlayerRotation.y += _lookVector.x;
        currentPlayerRotation.x += -_lookVector.y;

        // Clamp vertical camera movement 
        if (currentPlayerRotation.x > 180) currentPlayerRotation.x = currentPlayerRotation.x - 360f;
        currentPlayerRotation.x = Mathf.Clamp(currentPlayerRotation.x, -85f, 85f);

        // Apply new rotation
        playerCamera.transform.localRotation = Quaternion.Euler(currentPlayerRotation);
        var playerRotation = transform.rotation.eulerAngles;
        playerRotation.y = currentPlayerRotation.y;
        transform.rotation = Quaternion.Euler(playerRotation);
    }

    private void HandleMovement ()
    {
        // Lateral player movement
        // Get relevant input
        _movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        var speed = 4f;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= 2.1f;
        _movementVector = _movementVector.normalized * (speed);
        _movementVector = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * _movementVector;
        
        // Apply new velocity
        var vel = _rigidbody.velocity;
        vel.x = _movementVector.x;
        vel.z = _movementVector.z;
        _rigidbody.velocity = vel;
        
        // Player can jump if space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
        }
    }

    private void HandleMouseInput ()
    {
        var raycastResult = _raycaster.GetRaycastTarget(cameraAnchor.transform.position,
            playerCamera.transform.forward);
        
        // If the player left-clicks at a block that is close enough, it is destroyed
        if (raycastResult.Success && Input.GetMouseButtonDown(0))
        {
            raycastResult.ChunkData.SetBlock(raycastResult.BlockLocalPos, 0);
            raycastResult.ChunkComponent.InvalidateMesh();
            foreach (var neighboringChunkIndex in raycastResult.ChunkData.GetNeighborsCoordsIfAtEdge(
                raycastResult.BlockLocalPos))
            {
                _worldComponent.ChunkComponent(neighboringChunkIndex)?.InvalidateMesh();
            }
        }
        
        // If the player right-clicks at a block, a new block is placed
        if (raycastResult.Success && Input.GetMouseButtonDown(1))
        {
            _worldComponent.SetBlock(raycastResult.FacingBlockGlobalPos, (int)Random.Range(1, 7)).InvalidateMesh();
        }
        
        // DEBUG: If the player middle-clicks at a block, its world position is printed to the console
        if (raycastResult.Success && Input.GetMouseButtonDown(2))
        {
            _worldComponent.GetBlock(raycastResult.BlockGlobalPos, out var blockId);
            var block = BlockRegistry.GetBlockById(blockId);
            print(
                Vector3Int.FloorToInt(raycastResult.BlockGlobalPos).ToString()
                + $" [id={blockId}, name={block.Name}]"
                );
        }
        
        // Highlight the block the player is looking at, if close enough
        if (!raycastResult.Success) focussedBlockCube.gameObject.SetActive(false);
        else
        {
            focussedBlockCube.gameObject.SetActive(true);
            var center = raycastResult.ChunkData.LocalBlockPosToGlobalBlockPos(raycastResult.BlockLocalPos);
            // var center = raycastResult.GlobalBlockPos;
            var testCubeTransform = focussedBlockCube.transform;
            testCubeTransform.position = center;
            testCubeTransform.rotation = Quaternion.identity;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
