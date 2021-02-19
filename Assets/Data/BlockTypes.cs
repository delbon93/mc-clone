using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BlockGame.Backend.World;
using TMPro;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct BlockType
    {
        [SerializeField] public string blockName;
        [SerializeField] public short blockId;
        [SerializeField] public bool isOpaque;
        [SerializeField] public bool isSolid;
        [SerializeField] public Color reprColor;
        [SerializeField] public TextureCoords TexCoords;

        public BlockType (short blockId, string name = "NewBlock")
        {
            blockName = name;
            this.blockId = blockId;
            isOpaque = true;
            isSolid = true;
            reprColor = Color.white;
            TexCoords = Vector2Int.zero;
        }
    }
    
    [CreateAssetMenu(fileName = "BlockTypes", menuName = "Blocks/BlockTypes", order = 0)]
    public class BlockTypes : ScriptableObject
    {
        [SerializeField] private BlockType[] blockTypes = new BlockType[]{};
        private readonly Dictionary<short, BlockType> _idCache = new Dictionary<short, BlockType>();
        private readonly Dictionary<string, BlockType> _nameCache = new Dictionary<string, BlockType>();

        public int BlockCount => blockTypes.Length;

        public BlockType GetBlockById (short blockId)
        {
            if (_idCache.ContainsKey(blockId)) return _idCache[blockId];
            
            foreach (var block in blockTypes)
            {
                if (!block.blockId.Equals(blockId)) continue;
                _idCache[blockId] = block;
            }

            return _idCache[blockId];
        }

        public BlockType GetBlockByName (string blockName)
        {
            var lowerName = blockName.ToLower();
            if (_nameCache.ContainsKey(lowerName)) return _nameCache[lowerName];
            
            foreach (var block in blockTypes)
            {
                if (!block.blockName.ToLower().Equals(lowerName)) continue;
                _nameCache[lowerName] = block;
            }

            return _nameCache[lowerName];
        }
    }
}