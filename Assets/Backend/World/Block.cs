using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Backend
{
    public struct Block
    {
        public string Name { get; set; }
        public int BlockId { get; private set; }
        public bool IsOpaque { get; set; }
        public bool IsSolid { get; set; }
        public Color ReprColor { get; set; }
        public TextureCoords TexCoords { get; set; }
        

        public Block (int blockId, string name = "NewBlock")
        {
            Name = name;
            BlockId = blockId;
            IsOpaque = true;
            IsSolid = true;
            ReprColor = Color.white;
            TexCoords = Vector2Int.zero;
        }
    }
}