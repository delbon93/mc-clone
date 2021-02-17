using System.Collections.Generic;
using BlockGame.Components;
using UnityEngine;

namespace BlockGame.Backend
{
    public class ChunkComponentPool
    {
        private LinkedList<ChunkComponent> _chunkComponents;

        public ChunkComponentPool ()
        {
            _chunkComponents = new LinkedList<ChunkComponent>();
        }

        public bool HasFree () => _chunkComponents.Count > 0;
        
        public ChunkComponent Get ()
        {
            if (!HasFree()) return null;
            
            var chunkComponent = _chunkComponents.First.Value;
            _chunkComponents.RemoveFirst();
            
            return chunkComponent;
        }

        public void Free (ChunkComponent chunkComponent)
        {
            _chunkComponents.AddLast(chunkComponent);
        }
    }
}