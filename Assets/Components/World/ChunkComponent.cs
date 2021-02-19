using System;
using System.Collections.Generic;
using System.Diagnostics;
using BlockGame.Backend;
using BlockGame.Backend.World;
using UnityEngine;
using UnityEngine.UI;

namespace BlockGame.Components.World
{
    public class ChunkComponent : MonoBehaviour
    {
        private bool _chunkBorders = false;

        private Chunk _chunkData;
        private bool _meshInvalid = false;
        private WorldComponent _worldComponent;
        private GameData _gameData;

        private Dictionary<Direction, ChunkComponent> _neighborComponents = new Dictionary<Direction, ChunkComponent>()
        {
            {Direction.South, null},
            {Direction.North, null},
            {Direction.West, null},
            {Direction.East, null},
            {Direction.Up, null},
            {Direction.Down, null}
        };

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        [SerializeField] public LineRenderer chunkBorderRenderer;

        public Chunk ChunkData
        {
            get => _chunkData;
            set
            {
                _chunkData = value;
                _meshInvalid = true;
            }
        }

        private void Start ()
        {
            _meshCollider = GetComponent<MeshCollider>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _worldComponent = FindObjectOfType<WorldComponent>();
            _gameData = FindObjectOfType<GameData>();
            GameEvents.ToggleChunkBorders += GameEventsOnToggleChunkBorders;
        }

        private void GameEventsOnToggleChunkBorders ()
        {
            _chunkBorders = !_chunkBorders;
            chunkBorderRenderer.gameObject.SetActive(_chunkBorders);
        }

        private void OnDestroy ()
        {
            GameEvents.ToggleChunkBorders -= GameEventsOnToggleChunkBorders;
        }

        private void Update ()
        {
            if (_meshInvalid) GenerateChunkMesh();
        }

        private void GenerateChunkMesh ()
        {
            var (vertices, triangles, uvs)
                = new ChunkMeshGenerator().GenerateMeshData(this, _worldComponent, _gameData);
            _meshInvalid = false;
            var mesh = new Mesh {vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = uvs.ToArray()};
            mesh.RecalculateNormals();
            mesh.Optimize();
            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        public void InvalidateMesh ()
        {
            _meshInvalid = true;
        }

        public void SetNeighbor (Direction direction, ChunkComponent neighbor, bool setBoth = true)
        {
            _neighborComponents[direction] = neighbor;
            if (setBoth && neighbor != default(ChunkComponent))
                neighbor.SetNeighbor(direction.Opposite(), this, false);
        }

        public ChunkComponent GetNeighbor (Direction direction) => _neighborComponents[direction];
        
        public bool[] GetBlockSolidAdjacencyField (Vector3Int chunkPos)
        {
            bool Check (Vector3Int delta)
                => Chunk.IsInChunkBounds(chunkPos + delta) 
                   && _gameData.blockRegistry.ById(_chunkData.GetBlock(chunkPos + delta)).isSolid;

            return new bool[6]
            {
                Check(Vector3Int.left), Check(Vector3Int.right),
                Check(Vector3Int.down), Check(Vector3Int.up),
                Check(new Vector3Int(0, 0, -1)), Check(new Vector3Int(0, 0, 1))
            };
        }
    }
}