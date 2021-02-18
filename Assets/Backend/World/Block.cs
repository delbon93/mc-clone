using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockGame.Backend.World
{
    public struct Block
    {
        public string Name { get; set; }
        public short BlockId { get; private set; }
        public bool IsOpaque { get; set; }
        public bool IsSolid { get; set; }
        public Color ReprColor { get; set; }
        public TextureCoords TexCoords { get; set; }


        public Block (short blockId, string name = "NewBlock")
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