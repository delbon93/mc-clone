using BlockGame.Backend;
using BlockGame.Backend.World;
using BlockGame.Components;
using BlockGame.Components.World;
using UnityEngine;

namespace BlockGame.Components.Player
{
    public class PlayerViewRaycaster
    {
        public struct Result
        {
            public bool Success;
            public ChunkComponent ChunkComponent;
            public Vector3Int BlockLocalPos;
            public Vector3Int FacingBlockLocalPos;
            public Vector3 BlockGlobalPos;
            public Vector3 FacingBlockGlobalPos;
            public Chunk ChunkData => ChunkComponent.ChunkData;
        }

        public Result GetRaycastTarget (Vector3 origin, Vector3 direction)
        {
            var result = new Result {Success = false};

            var ray = new Ray(origin, direction);
            if (!Physics.Raycast(ray, out var hitInfo, 5f, LayerMask.GetMask("World")))
                return result;

            result.Success = true;
            result.ChunkComponent = hitInfo.collider.gameObject.GetComponent<ChunkComponent>();
            var chunkData = result.ChunkComponent.ChunkData;
            result.BlockLocalPos = chunkData.RaycastHitToLocalBlockPos(hitInfo.point, hitInfo.normal);
            result.BlockGlobalPos = chunkData.LocalBlockPosToGlobalBlockPos(result.BlockLocalPos);
            result.FacingBlockLocalPos = chunkData.RaycastHitToLocalBlockPos(hitInfo.point, -hitInfo.normal);
            result.FacingBlockGlobalPos = chunkData.LocalBlockPosToGlobalBlockPos(result.FacingBlockLocalPos);
            return result;
        }
    }
}