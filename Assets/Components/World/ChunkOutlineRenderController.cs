using System.Collections;
using System.Collections.Generic;
using BlockGame.Backend;
using BlockGame.Backend.World;
using UnityEngine;

namespace BlockGame.Components.World
{
    public class ChunkOutlineRenderController : MonoBehaviour
    {
        [SerializeField] public LineRenderer lineRenderer;

        private void Start ()
        {
            for (var i = 0; i < lineRenderer.positionCount; i++)
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i) * Chunk.ChunkSize);
        }
    }
}