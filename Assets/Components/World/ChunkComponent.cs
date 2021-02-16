using System;
using System.Collections.Generic;
using BlockGame.Backend;
using UnityEngine;

namespace BlockGame.Components
{
    public class ChunkComponent : MonoBehaviour
    {

        private Chunk _chunkData;
        private bool _meshInvalid = true;
        private WorldComponent _worldComponent;

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
            _worldComponent = FindObjectOfType<WorldComponent>();
            GameEvents.ToggleChunkBorders += OnToggleChunkBorders;

        }

        private void OnToggleChunkBorders ()
        {
            chunkBorderRenderer.gameObject.SetActive(!chunkBorderRenderer.gameObject.activeSelf);
        }

        private void OnDestroy ()
        {
            GameEvents.ToggleChunkBorders -= OnToggleChunkBorders;
        }

        private void Update ()
        {
            if (_meshInvalid) GenerateChunkMesh();
        }

        private void GenerateChunkMesh ()
        {
            var (vertices, triangles, uvs) 
                = new ChunkMeshGenerator().GenerateMeshData(_chunkData, _worldComponent.World);
            _meshInvalid = false;
            var mesh = new Mesh {vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = uvs.ToArray()};
            mesh.RecalculateNormals();
            mesh.Optimize();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        public void InvalidateMesh ()
        {
            _meshInvalid = true;
        }
    }
}