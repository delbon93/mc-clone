using UnityEngine;

namespace BlockGame.Backend.World
{
    public interface IChunkContainer
    {
        void AddChunk (Chunk chunk);
        void RemoveChunk (Chunk chunk);
        bool HasChunkAtGlobalChunkPos (Vector3Int worldPos);
        Chunk GetChunkByIndex (Vector3Int chunkIndex);
        Chunk GetChunkByGlobalPos (Vector3 pos);
    }
}