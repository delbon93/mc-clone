using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockGame.Backend
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
            if (dist < mesaRadius) return (int)Mathf.Floor(topHeight);
            return (int)Mathf.Floor((mesaRadius / dist) * topHeight);
        }
        
        public void GenerateChunk (ref Chunk chunk)
        {
            for (var i = 0; i < Chunk.BlockCount; i++)
            {
                var blockToSet = BlockRegistry.Air;
                var worldPos = chunk.GlobalIndex * Chunk.ChunkSize + Chunk.IndexToLocalBlockPos(i);
                var currentHeight = (int)Mathf.Floor(chunk.IndexToGlobalBlockPos(i).y);

                var threshold = 8 + Random.value < 0.2f ? Random.Range(-2, 2) : 0;

                // foreach (var (pos, mesa, height) in _hills)
                // {
                //     threshold += Hill(pos, mesa, height, chunk, i);
                // }

                if (currentHeight == (int) threshold)
                {
                    blockToSet = Random.value < 0.08f ? BlockRegistry.Sand : BlockRegistry.Grass;
                }
                if (currentHeight < (int)threshold) blockToSet = BlockRegistry.Dirt;
                if (currentHeight < (int) threshold - 10) blockToSet = BlockRegistry.Stone;

                chunk.SetBlock(i, blockToSet.BlockId);
            }
        }
    }
}