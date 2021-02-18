using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Backend.World
{
    public class World
    {
        private readonly IChunkContainer _loadedChunks;
        private readonly IWorldGenerator _worldGenerator;

        public World ()
        {
            _loadedChunks = new ChunkList();
            _worldGenerator = new WorldGenerator();
            _worldGenerator.Initialize();
        }

        private Chunk LoadChunk (Vector3Int chunkIndex)
        {
            if (_loadedChunks.HasChunkAtGlobalChunkPos(chunkIndex))
                return _loadedChunks.GetChunkByIndex(chunkIndex);
            var chunk = new Chunk(chunkIndex);
            _worldGenerator.GenerateChunk(ref chunk);
            _loadedChunks.AddChunk(chunk);
            return chunk;
        }

        public Chunk GetChunkAtWorldPos (Vector3Int worldPos, bool generateOnDemand = true) =>
            _loadedChunks.GetChunkByIndex(worldPos)
            ?? (generateOnDemand ? LoadChunk(worldPos) : default(Chunk));


        public Chunk SetBlock (Vector3Int globalBlockPos, short blockId)
        {
            var chunk = _loadedChunks.GetChunkByGlobalPos(globalBlockPos);
            if (chunk == default(Chunk)) return default(Chunk);

            var localBlockPos = globalBlockPos - chunk.GlobalIndex * Chunk.ChunkSize;
            chunk.SetBlock(localBlockPos, blockId);
            return chunk;
        }

        public Chunk GetBlock (Vector3Int globalBlockPos, out short blockId)
        {
            var chunk = _loadedChunks.GetChunkByGlobalPos(globalBlockPos);
            if (chunk == default(Chunk))
            {
                blockId = -1;
                return default;
            }

            var localBlockPos = globalBlockPos - chunk.GlobalIndex * Chunk.ChunkSize;
            blockId = chunk.GetBlock(localBlockPos);
            return chunk;
        }

        public bool[] GetBlockSolidAdjacencyField (Vector3Int globalBlockPos)
        {
            bool Check (Vector3Int delta)
            {
                var chunk = GetBlock(globalBlockPos + delta, out short blockId);
                return chunk == null || BlockRegistry.GetBlockById(blockId).IsSolid;
            }

            return new bool[6]
            {
                Check(Vector3Int.left), Check(Vector3Int.right),
                Check(Vector3Int.down), Check(Vector3Int.up),
                Check(new Vector3Int(0, 0, -1)), Check(new Vector3Int(0, 0, 1))
            };
        }
    }
}