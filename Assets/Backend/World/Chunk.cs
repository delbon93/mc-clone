using System.Collections.Generic;
using Data;
using UnityEngine;

namespace BlockGame.Backend.World
{
    public class Chunk
    {
        public const int ChunkSize = 16;
        public const int BlockCount = ChunkSize * ChunkSize * ChunkSize;

        public Vector3Int GlobalIndex { get; private set; }

        private readonly short[] _blocks;

        public Chunk (Vector3Int globalIndex)
        {
            GlobalIndex = globalIndex;
            _blocks = new short [BlockCount];
        }

        public short GetBlock (int index) => _blocks[index];
        public short GetBlock (Vector3Int localBlockPos) => GetBlock(LocalBlockPosToIndex(localBlockPos));
        public void SetBlock (int index, short blockId) => _blocks[index] = blockId;
        public void SetBlock (Vector3Int chunkPos, short blockId) => _blocks[LocalBlockPosToIndex(chunkPos)] = blockId;

        

        public static bool IsInChunkBounds (Vector3Int localBlockPos)
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
            var globalBlockPos = Vector3Int.FloorToInt(pointInBlock);
            var chunkOrigin = GlobalIndex * ChunkSize;
            return globalBlockPos - chunkOrigin;
        }

        public Vector3 LocalBlockPosToGlobalBlockPos (Vector3Int chunkPos)
        {
            var chunkOrigin = GlobalIndex * ChunkSize;
            var globalBlockPos = chunkOrigin + chunkPos;
            return (Vector3) globalBlockPos + Vector3.one * 0.5f;
        }

        public Vector3Int IndexToGlobalBlockIndex (int index)
        {
            return Vector3Int.FloorToInt(IndexToGlobalBlockPos(index));
        }

        public Vector3 IndexToGlobalBlockPos (int i) => LocalBlockPosToGlobalBlockPos(IndexToLocalBlockPos(i));


        public static Vector3Int GlobalPositionToChunkIndex (Vector3 position)
            => Vector3Int.FloorToInt(position / ChunkSize);

        public static List<Vector3Int> GetIndexSphere (int radius)
        {
            var list = new List<Vector3Int>();
            for (var z = -radius; z <= radius; z++)
            {
                for (var y = -radius; y <= radius; y++)
                {
                    for (var x = -radius; x <= radius; x++)
                    {
                        var v = new Vector3Int(x, y, z);
                        if (v.magnitude <= radius) list.Add(v);
                    }
                }
            }

            list.Sort((v1, v2) => v1.magnitude.CompareTo(v2.magnitude));

            return list;
        }
    }
}