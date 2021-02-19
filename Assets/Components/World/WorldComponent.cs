using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using BlockGame.Backend;
using BlockGame.Backend.World;
using UnityEngine;

namespace BlockGame.Components.World
{
    public class WorldComponent : MonoBehaviour
    {
        [SerializeField] public ChunkComponent chunkPrefab;

        public Backend.World.World World { get; private set; }

        private readonly Dictionary<Vector3Int, ChunkComponent> _chunkComponents
            = new Dictionary<Vector3Int, ChunkComponent>();

        private readonly ChunkComponentPool _chunkPool = new ChunkComponentPool();

        private List<Vector3Int> _indexSphere;

        private GameData _gameData;

        private void Awake ()
        {
            var radius = 4;
            _indexSphere = Chunk.GetIndexSphere(radius);
            _gameData = FindObjectOfType<GameData>();

            World = new Backend.World.World(_gameData);
            GameEvents.EnterChunk += GameEventsOnEnterChunk;
            PreloadWorld();
        }

        private void GameEventsOnEnterChunk (Vector3Int index)
        {
            StartCoroutine(nameof(ChunkLoadingCoroutine), index);
        }

        private IEnumerator ChunkLoadingCoroutine (Vector3Int index)
        {
            var loadedIndices = new Vector3Int[_indexSphere.Count()];
            for (var i = 0; i < loadedIndices.Length; i++)
            {
                loadedIndices[i] = _indexSphere[i] + index;
            }

            var indicesToUnload = _chunkComponents.Keys.Except(loadedIndices).ToArray();
            foreach (var i in indicesToUnload)
            {
                UnloadChunk(i);
                //yield return null;
            }

            foreach (var i in loadedIndices)
            {
                LoadChunk(i);
                yield return null;
            }
        }

        private void PreloadWorld ()
        {
            foreach (var index in _indexSphere)
                LoadChunk(index);
        }

        private void LoadChunk (Vector3Int index)
        {
            if (_chunkComponents.ContainsKey(index)) return;

            var chunkData = World.GetChunkAtWorldPos(index);

            ChunkComponent chunkComponent;

            if (_chunkPool.HasFree())
            {
                chunkComponent = _chunkPool.Get();
                chunkComponent.gameObject.SetActive(true);
                chunkComponent.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                chunkComponent = Instantiate(chunkPrefab, transform, true);
            }

            chunkComponent.name = $"Chunk {index.x},{index.y},{index.z}";
            chunkComponent.ChunkData = chunkData;
            chunkComponent.transform.position = index * Chunk.ChunkSize;
            _chunkComponents.Add(index, chunkComponent);
            foreach (var dir in OrthoDirExtensions.AllOrthogonal)
            {
                var neighborIndex = index + dir.ToVector3Int();
                if (_chunkComponents.ContainsKey(neighborIndex))
                {
                    chunkComponent.SetNeighbor(dir, _chunkComponents[neighborIndex]);
                    _chunkComponents[neighborIndex].InvalidateMesh();
                }
            }
        }

        private void UnloadChunk (Vector3Int index)
        {
            if (!_chunkComponents.ContainsKey(index)) return;

            var chunkComponent = _chunkComponents[index];
            chunkComponent.gameObject.SetActive(false);
            chunkComponent.GetComponent<MeshRenderer>().enabled = false;
            _chunkComponents.Remove(index);
            _chunkPool.Free(chunkComponent);
        }


        public ChunkComponent SetBlock (Vector3Int globalBlockPos, short blockId)
        {
            return _chunkComponents[World.SetBlock(globalBlockPos, blockId).GlobalIndex];
        }

        public ChunkComponent SetBlock (Vector3 globalBlockPos, short blockId)
        {
            return SetBlock(Vector3Int.FloorToInt(globalBlockPos), blockId);
        }

        public ChunkComponent GetBlock (Vector3Int globalBlockPos, out short blockId)
        {
            var chunk = World.GetBlock(globalBlockPos, out var outBlockId);
            blockId = outBlockId;
            if (chunk == null || !_chunkComponents.ContainsKey(chunk.GlobalIndex)) return null;
            return _chunkComponents[chunk.GlobalIndex];
        }

        public ChunkComponent GetBlock (Vector3 globalBlockPos, out short blockId)
        {
            var chunkComponent = GetBlock(Vector3Int.FloorToInt(globalBlockPos), out var outBlockId);
            blockId = outBlockId;
            return chunkComponent;
        }

        public ChunkComponent GetChunkComponent (Vector3Int chunkIndex)
        {
            return _chunkComponents.ContainsKey(chunkIndex) ? _chunkComponents[chunkIndex] : null;
        }
        
        public bool[] GetBlockSolidAdjacencyField (Vector3Int globalBlockPos)
        {
            bool Check (Vector3Int delta)
            {
                var chunk = GetBlock(globalBlockPos + delta, out short blockId);
                return chunk == null || _gameData.blockRegistry.GetBlockById(blockId).isSolid;
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