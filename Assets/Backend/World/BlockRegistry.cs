using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BlockGame.Backend
{
    public static class BlockRegistry
    {
        public static Block Air = new Block(0, "Air") { IsOpaque = false, IsSolid = false};
        public static Block Dirt = new Block(1, "Dirt") { ReprColor = new Color(0.5f, 0.32f, 0.23f), 
            TexCoords = new Vector2Int(0, 0)};
        public static Block Grass = new Block(2, "Grass")
        {
            ReprColor = Color.green,
            TexCoords = new TextureCoords(new Vector2Int(1, 0))
            {
                Top = new Vector2Int(2, 0),
                Bottom = new Vector2Int(0, 0)
            }
        };
        public static Block Stone = new Block(3, "Stone") {ReprColor = Color.grey, TexCoords = new Vector2Int(3, 0)};
        public static Block Sand = new Block(4, "Sand") {ReprColor = Color.yellow, TexCoords = new Vector2Int(4, 0)};
        public static Block Log = new Block(5, "Log") { 
            TexCoords = new TextureCoords(new Vector2Int(5, 0))
            {
                Top = new Vector2Int(6, 0),
                Bottom = new Vector2Int(6, 0)
            }
        };
        public static Block Leaves = new Block(6, "Leaves") { TexCoords = new Vector2Int(7, 0) };

        private static readonly Dictionary<int, Block> BlockByIdCache = new Dictionary<int, Block>();

        // Examines all static fields of BlockRegistry to find the block that has the given id
        public static Block GetBlockById (short blockId)
        {
            if (BlockByIdCache.ContainsKey(blockId)) 
                return BlockByIdCache[blockId];
            foreach (var field in typeof(BlockRegistry).GetFields(
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var blockType = field.GetValue(null);
                if (!(blockType is Block block)) continue;
                if (block.BlockId != blockId) continue;
                BlockByIdCache.Add(blockId, block);
                return block;
            }

            return default(Block);
        }
    }
}