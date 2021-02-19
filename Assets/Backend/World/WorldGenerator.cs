using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockGame.Backend.World
{
    public class WorldGenerator : IWorldGenerator
    {
        private List<(Vector2 pos, float mesa, float height)> _hills;

        public void Initialize ()
        {
            _hills = new List<(Vector2 pos, float mesa, float height)>();
            for (var i = 0; i < 4; i++)
            {
                _hills.Add((
                    Random.insideUnitCircle * Random.value * 100f,
                    Random.Range(5f, 20f),
                    Random.Range(10f, 20f)));
            }
        }

        private int Hill (Vector2 hillPos, float mesaRadius, float topHeight, Chunk chunk, int blockIndex)
        {
            var blockPos = chunk.IndexToGlobalBlockPos(blockIndex);
            var flatBlockPos = new Vector2(blockPos.x, blockPos.z);
            var dist = (hillPos - flatBlockPos).magnitude;
            if (dist < mesaRadius) return (int) Mathf.Floor(topHeight);
            return (int) Mathf.Floor((mesaRadius / dist) * topHeight);
        }

        public void GenerateChunk (ref Chunk chunk, GameData gameData)
        {
            var BlockRegistry = gameData.blockRegistry;
            for (var i = 0; i < Chunk.BlockCount; i++)
            {
                var blockToSet = BlockRegistry.ByName("air");
                var worldPos = chunk.GlobalIndex * Chunk.ChunkSize + Chunk.IndexToLocalBlockPos(i);

                var threshold = Random.value < 0.05f ? Random.Range(-1, 1) : 0;

                foreach (var (pos, mesa, height) in _hills)
                {
                    threshold += Hill(pos, mesa, height, chunk, i);
                }

                if (worldPos.y == (int) threshold)
                {
                    blockToSet = Random.value < 0.08f ? 
                        BlockRegistry.ByName("sand") : BlockRegistry.ByName("grass");
                }

                if (worldPos.y < (int) threshold) blockToSet = BlockRegistry.ByName("dirt");
                if (worldPos.y < (int) threshold - 10) blockToSet = BlockRegistry.ByName("stone");

                chunk.SetBlock(i, blockToSet.blockId);
            }
        }
    }
}