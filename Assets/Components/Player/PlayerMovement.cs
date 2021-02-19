using System;
using System.Diagnostics;
using BlockGame.Backend;
using BlockGame.Backend.World;
using BlockGame.Components.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockGame.Components.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 _movementVector = Vector3.zero;
        private Vector2 _lookVector = Vector2.zero;
        private Rigidbody _rigidbody;
        private readonly PlayerViewRaycaster _raycaster = new PlayerViewRaycaster();
        private WorldComponent _worldComponent;
        private Vector3Int _currentChunkIndex = Vector3Int.zero;
        private GameData _gameData;
        private BlockDebugInfo _blockDebugInfo;

        private int _selectedBlockIndex = 1;

        public bool CanJump { get; set; } = true;

        [SerializeField] public Camera playerCamera;
        [SerializeField] public Transform cameraAnchor;
        [SerializeField] public Transform focussedBlockCube;

        private void Start ()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _worldComponent = FindObjectOfType<WorldComponent>();
            _gameData = FindObjectOfType<GameData>();
            _blockDebugInfo = FindObjectOfType<BlockDebugInfo>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameEvents.OnChangeInventorySelection(1);
        }

        private void Update ()
        {
            UpdateCamera();
            UpdateMovement();
            UpdateMouseInput();

            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (Input.GetKeyDown(KeyCode.C))
                GameEvents.OnToggleChunkBorders();
            if (_blockDebugInfo != null && Input.GetKeyDown(KeyCode.B))
            {
                _blockDebugInfo.ToggleShown();
            }
        }

        private void UpdateCamera ()
        {
            // Update camera position
            playerCamera.transform.position = cameraAnchor.transform.position;

            // Get relevant input for camera rotation
            _lookVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            _lookVector *= 4.8f;
            _lookVector.x *= 1.3f;

            // Get current rotation and modify it
            var currentPlayerRotation = playerCamera.transform.localRotation.eulerAngles;
            currentPlayerRotation.y += _lookVector.x;
            currentPlayerRotation.x += -_lookVector.y;

            // Clamp vertical camera movement 
            if (currentPlayerRotation.x > 180) currentPlayerRotation.x = currentPlayerRotation.x - 360f;
            currentPlayerRotation.x = Mathf.Clamp(currentPlayerRotation.x, -89f, 89f);

            // Apply new rotation
            playerCamera.transform.localRotation = Quaternion.Euler(currentPlayerRotation);
            var playerRotation = transform.rotation.eulerAngles;
            playerRotation.y = currentPlayerRotation.y;
            transform.rotation = Quaternion.Euler(playerRotation);
        }

        private void UpdateMovement ()
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
            if (Input.GetKeyDown(KeyCode.Space) && CanJump)
            {
                _rigidbody.AddForce(Vector3.up * 7f, ForceMode.VelocityChange);
            }

            var chunkIndex = Chunk.GlobalPositionToChunkIndex(transform.position);
            if (!chunkIndex.Equals(_currentChunkIndex))
            {
                _currentChunkIndex = chunkIndex;
                GameEvents.OnEnterChunk(chunkIndex);
            }
        }

        private void UpdateMouseInput ()
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
                    _worldComponent.GetChunkComponent(neighboringChunkIndex)?.InvalidateMesh();
                }
            }

            // If the mouse wheel is scrolled, select a different block to place
            var mouseWheel = Input.mouseScrollDelta.y > 0 ? 1 : (Input.mouseScrollDelta.y < 0 ? -1 : 0);
            if (mouseWheel != 0)
            {
                _selectedBlockIndex = (_selectedBlockIndex + mouseWheel) % _gameData.blockRegistry.BlockCount;
                
                if (_selectedBlockIndex == 0)
                    if (mouseWheel < 0)
                        _selectedBlockIndex = _gameData.blockRegistry.BlockCount - 1;
                    else
                        _selectedBlockIndex = 1;
                
                GameEvents.OnChangeInventorySelection(
                    _gameData.blockRegistry.ByRegistrationIndex(_selectedBlockIndex).blockId);
            }

            // If the player right-clicks at a block, a new block is placed
            if (raycastResult.Success && Input.GetMouseButtonDown(1))
            {
                var blockId = _gameData.blockRegistry.ByRegistrationIndex(_selectedBlockIndex).blockId;
                _worldComponent.SetBlock(raycastResult.FacingBlockGlobalPos, blockId).InvalidateMesh();
            }

            var debugText = "";
            if (raycastResult.Success && _blockDebugInfo != null)
            {
                _worldComponent.GetBlock(raycastResult.BlockGlobalPos, out var blockId);
                var block = _gameData.blockRegistry.ById(blockId);
                debugText = block + "\n@\n" + Vector3Int.FloorToInt(raycastResult.BlockGlobalPos);
                _blockDebugInfo.UpdateText(debugText);
            }
            else if (_blockDebugInfo != null)
            {
                _blockDebugInfo.UpdateText("");
            }

            // DEBUG: If the player middle-clicks at a block, its world position is printed to the console
            if (raycastResult.Success && Input.GetMouseButtonDown(2))
            {
                print(debugText);
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

            if (Input.GetKey(KeyCode.Tab))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        }
    }
}