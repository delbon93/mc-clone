using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Backend.World
{
    public class World
    {
        private readonly IChunkContainer _loadedChunks;
        private readonly IWorldGenerator _worldGenerator;
        private readonly GameData _gameData;

        public World (GameData gameData)
        {
            _loadedChunks = new ChunkList();
            _gameData = gameData;
            _worldGenerator = new WorldGenerator();
            _worldGenerator.Initialize();
        }

        private Chunk LoadChunk (Vector3Int chunkIndex)
        {
            if (_loadedChunks.HasChunkAtGlobalChunkPos(chunkIndex))
                return _loadedChunks.GetChunkByIndex(chunkIndex);
            var chunk = new Chunk(chunkIndex);
            _worldGenerator.GenerateChunk(ref chunk, _gameData);
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

        
    }
}