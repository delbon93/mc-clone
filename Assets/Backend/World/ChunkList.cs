using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Backend.World
{
    public class ChunkList : IChunkContainer
    {
        private readonly Dictionary<Vector3Int, Chunk> _chunks;

        public ChunkList () => _chunks = new Dictionary<Vector3Int, Chunk>();

        public void AddChunk (Chunk chunk) => _chunks.Add(chunk.GlobalIndex, chunk);
        public void RemoveChunk (Chunk chunk) => _chunks.Remove(chunk.GlobalIndex);
        public bool HasChunkAtGlobalChunkPos (Vector3Int worldPos) => _chunks.ContainsKey(worldPos);

        public Chunk GetChunkByIndex (Vector3Int chunkIndex)
            => !_chunks.ContainsKey(chunkIndex) ? default : _chunks[chunkIndex];

        public Chunk GetChunkByGlobalPos (Vector3 pos)
        {
            pos /= Chunk.ChunkSize;
            var chunkCoords = new Vector3Int(
                (int) Mathf.Floor(pos.x), (int) Mathf.Floor(pos.y), (int) Mathf.Floor(pos.z));
            return _chunks.ContainsKey(chunkCoords) ? _chunks[chunkCoords] : default;
        }
    }
}