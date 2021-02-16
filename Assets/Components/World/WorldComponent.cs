using System.Collections;
using System.Collections.Generic;
using Backend;
using BlockGame.Backend;
using UnityEngine;

namespace BlockGame.Components
{
    public class WorldComponent : MonoBehaviour
    {
        [SerializeField] public ChunkComponent chunkPrefab;
        
        public World World { get; private set; }

        private readonly Dictionary<Vector3Int, ChunkComponent> _chunkComponents 
            = new Dictionary<Vector3Int, ChunkComponent>();

        private void Start ()
        {
            World = new World();
            PreloadWorld();
        }

        private void PreloadWorld ()
        {
            const int r = 1;
            for (var z = -r; z <= r; z++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    for (var x = -r; x <= r; x++)
                    {
                        CreateChunk(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        private void CreateChunk (Vector3Int index)
        {
            var chunkData = World.GetChunkAtWorldPos(index);
            var chunkObject = Instantiate(chunkPrefab, transform, true);
            chunkObject.name = $"Chunk {index.x},{index.y},{index.z}";
            chunkObject.transform.position = index * Chunk.ChunkSize;
            chunkObject.ChunkData = chunkData;  
            _chunkComponents.Add(index, chunkObject);
            foreach (var dir in OrthoDirExtensions.All)
            {
                var neighborIndex = index + dir.ToVector3Int();
                if (_chunkComponents.ContainsKey(neighborIndex))
                    chunkObject.SetNeighbor(dir, _chunkComponents[neighborIndex]);
            }
        }

        public ChunkComponent SetBlock (Vector3Int globalBlockPos, int blockId)
        {
            return _chunkComponents[World.SetBlock(globalBlockPos, blockId).GlobalIndex];
        }

        public ChunkComponent SetBlock (Vector3 globalBlockPos, int blockId)
        {
            return SetBlock(Vector3Int.FloorToInt(globalBlockPos), blockId);
        }

        public ChunkComponent GetBlock (Vector3Int globalBlockPos, out int blockId)
        {
            var chunk = World.GetBlock(globalBlockPos, out var outBlockId);
            blockId = outBlockId;
            return _chunkComponents[chunk.GlobalIndex];
        }

        public ChunkComponent GetBlock (Vector3 globalBlockPos, out int blockId)
        {
            var chunkComponent = GetBlock(Vector3Int.FloorToInt(globalBlockPos), out var outBlockId);
            blockId = outBlockId;
            return chunkComponent;
        }

        public ChunkComponent ChunkComponent (Vector3Int chunkIndex)
        {
            return _chunkComponents.ContainsKey(chunkIndex) ? _chunkComponents[chunkIndex] : null;
        }
    }

}