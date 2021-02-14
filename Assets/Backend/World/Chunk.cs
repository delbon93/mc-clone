using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Backend
{
    public class Chunk
    {
        public const int ChunkSize = 16;
        public const int BlockCount = ChunkSize * ChunkSize * ChunkSize;

        public Vector3Int GlobalIndex { get; private set; }

        private readonly int[] _blocks;

        public Chunk (Vector3Int globalIndex)
        {
            GlobalIndex = globalIndex;
            _blocks = new int [BlockCount];
        }

        public int GetBlock (int index) => _blocks[index];
        public int GetBlock (Vector3Int localBlockPos) => GetBlock(LocalBlockPosToIndex(localBlockPos));
        public void SetBlock (int index, int blockId) => _blocks[index] = blockId;
        public void SetBlock (Vector3Int chunkPos, int blockId) => _blocks[LocalBlockPosToIndex(chunkPos)] = blockId;

        public bool[] GetBlockSolidAdjacencyField (Vector3Int chunkPos)
        {
            bool Check (Vector3Int delta) 
                => IsInChunkBounds(chunkPos + delta) && BlockRegistry.GetBlockById(GetBlock(chunkPos + delta)).IsSolid;

            return new bool[6]
            {
                Check(Vector3Int.left), Check(Vector3Int.right),
                Check(Vector3Int.down), Check(Vector3Int.up),
                Check(new Vector3Int(0, 0, -1)), Check(new Vector3Int(0, 0, 1))
            };
        }

        private static bool IsInChunkBounds (Vector3Int localBlockPos)
        {
            return localBlockPos.x >= 0 && localBlockPos.x < ChunkSize
                                   && localBlockPos.y >= 0 && localBlockPos.y < ChunkSize
                                   && localBlockPos.z >= 0 && localBlockPos.z < ChunkSize;
        }

        public List<Vector3Int> GetNeighborsCoordsIfAtEdge (Vector3Int localBlockPos)
        {
            var list = new List<Vector3Int>();
            if (localBlockPos.x == 0) list.Add(GlobalIndex + Vector3Int.left);
            else if (localBlockPos.x == 15) list.Add(GlobalIndex + Vector3Int.right);
            if (localBlockPos.y == 0) list.Add(GlobalIndex + Vector3Int.down);
            else if (localBlockPos.y == 15) list.Add(GlobalIndex + Vector3Int.up);
            if (localBlockPos.z == 0) list.Add(GlobalIndex + new Vector3Int(0, 0, -1));
            else if (localBlockPos.z == 15) list.Add(GlobalIndex + new Vector3Int(0, 0, 1));
            return list;
        }
        
        public static int LocalBlockPosToIndex (Vector3Int chunkPos)
        {
            return chunkPos.x + ChunkSize * chunkPos.y + ChunkSize * ChunkSize * chunkPos.z;
        }

        public static Vector3Int IndexToLocalBlockPos (int index)
        {
            var z = index / (ChunkSize * ChunkSize);
            var y = (index - z * ChunkSize * ChunkSize) / ChunkSize;
            var x = (index - z * ChunkSize * ChunkSize - y * ChunkSize);
            return new Vector3Int(x, y, z);
        }

        public Vector3Int RaycastHitToLocalBlockPos (Vector3 point, Vector3 normal)
        {
            var pointInBlock = point - normal * 0.5f;
            var globalBlockPos = new Vector3Int(
                (int)Mathf.Floor(pointInBlock.x), (int)Mathf.Floor(pointInBlock.y), (int)Mathf.Floor(pointInBlock.z));
            var chunkOrigin = GlobalIndex * ChunkSize;
            return globalBlockPos - chunkOrigin;
        }

        public Vector3 LocalBlockPosToGlobalBlockPos (Vector3Int chunkPos)
        {
            var chunkOrigin = GlobalIndex * ChunkSize;
            var globalBlockPos = chunkOrigin + chunkPos;
            return (Vector3)globalBlockPos + Vector3.one * 0.5f;
        }

        public Vector3Int IndexToGlobalBlockIndex (int index)
        {
            return Vector3Int.FloorToInt(IndexToGlobalBlockPos(index));
        }

        public Vector3 IndexToGlobalBlockPos (int i) => LocalBlockPosToGlobalBlockPos(IndexToLocalBlockPos(i));

        
    }
}