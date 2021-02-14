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