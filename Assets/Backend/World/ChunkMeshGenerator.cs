using System;
using System.Collections.Generic;
using BlockGame.Backend;
using UnityEngine;

namespace BlockGame.Backend
{
    public class ChunkMeshGenerator
    {
        private static readonly Vector3[] BottomFaceVertices = new[]
        {
            Vector3.zero, Vector3.right, Vector3.forward, Vector3.forward + Vector3.right
        };
        
        private static readonly Vector3[] TopFaceVertices = new[]
        {
            Vector3.up, Vector3.up + Vector3.forward, Vector3.up + Vector3.right, Vector3.one
        };
        
        private static readonly Vector3[] FrontFaceVertices = new[]
        {
            Vector3.right + Vector3.forward, Vector3.one, Vector3.forward, Vector3.up + Vector3.forward
        };

        private static readonly Vector3[] BackFaceVertices = new[]
        {
            Vector3.zero, Vector3.up, Vector3.right, Vector3.up + Vector3.right
        };

        private static readonly Vector3[] LeftFaceVertices = new[]
        {
            Vector3.forward, Vector3.forward + Vector3.up, Vector3.zero, Vector3.up
        };
        
        private static readonly Vector3[] RightFaceVertices = new[]
        {
            Vector3.right, Vector3.right + Vector3.up, Vector3.right + Vector3.forward, Vector3.one
        };

        private static readonly int[] TriangleRelativeIndices = new[] {0, 1, 2, 1, 3, 2};

        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private int _vertexIndex = 0;

        private static IEnumerable<Vector2> TexCoordToUvs (Vector2Int texCoord)
        {
            const float scaleFactor = 16f / 256f;
            var right = Vector2.right * scaleFactor;
            var up = Vector2.up * scaleFactor;
            var uvBase = new Vector2(texCoord.x * scaleFactor, 1f - texCoord.y * scaleFactor);
            return new []
            {
                // uvBase, uvBase + up, uvBase + right, uvBase + up + right
                // must be reverted, because for some reason the texture uv must be mirrored
                uvBase + up + right, uvBase + right, uvBase + up, uvBase
            };
        }

        private static Vector2Int GetTexCoordsForFace (IReadOnlyList<Vector3> faceVertices, Block block)
        {
            if (faceVertices == BackFaceVertices) return block.TexCoords.Back;
            if (faceVertices == BottomFaceVertices) return block.TexCoords.Bottom;
            if (faceVertices == FrontFaceVertices) return block.TexCoords.Front;
            if (faceVertices == LeftFaceVertices) return block.TexCoords.Left;
            if (faceVertices == RightFaceVertices) return block.TexCoords.Right;
            if (faceVertices == TopFaceVertices) return block.TexCoords.Top;
            return Vector2Int.zero;
        }

        private void AddBlockFace (Block block, Vector3Int blockChunkPos, IReadOnlyList<Vector3> faceVertices)
        {
            var vertexIndices = new int[4];
            
            for (var i = 0; i < 4; i++)
            {
                var vertex = faceVertices[i] + blockChunkPos;
                _vertices.Add(vertex);
                vertexIndices[i] = _vertexIndex;
                _vertexIndex++;
            }
            for (var i = 0; i < 6; i++)
            {
                _triangles.Add(vertexIndices[TriangleRelativeIndices[i]]);
            }
            _uvs.AddRange(TexCoordToUvs(GetTexCoordsForFace(faceVertices, block)));
        }

        private void BuildAllBlockFaces (Chunk chunkData, World world)
        {
            for (var index = 0; index < Chunk.BlockCount; index++)
            {
                var block = BlockRegistry.GetBlockById(chunkData.GetBlock(index));
                if (!block.IsOpaque) continue;

                var localBlockPos = Chunk.IndexToLocalBlockPos(index);
                var globalBlockPos = chunkData.IndexToGlobalBlockIndex(index);
                var field = world.GetBlockSolidAdjacencyField(globalBlockPos);

                void CheckFace (int i, IReadOnlyList<Vector3> faceVertices)
                {
                    if (!field[i]) AddBlockFace(block, localBlockPos, faceVertices);
                }

                CheckFace(0, LeftFaceVertices);
                CheckFace(1, RightFaceVertices);
                CheckFace(2, BottomFaceVertices);
                CheckFace(3, TopFaceVertices);
                CheckFace(4, BackFaceVertices);
                CheckFace(5, FrontFaceVertices);
            }
        }
        
        public (List<Vector3> vertices, List<int> triangles, List<Vector2> uvs) 
            GenerateMeshData (Chunk chunkData, World world)
        {
            BuildAllBlockFaces(chunkData, world);

            
            return (_vertices, _triangles, _uvs);
        }
    }
}