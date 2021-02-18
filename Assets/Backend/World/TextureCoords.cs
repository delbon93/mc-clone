using UnityEngine;

namespace BlockGame.Backend.World
{
    public struct TextureCoords
    {
        public Vector2Int Top;
        public Vector2Int Bottom;
        public Vector2Int Right;
        public Vector2Int Left;
        public Vector2Int Front;
        public Vector2Int Back;

        public TextureCoords (Vector2Int coords)
        {
            Top = Bottom = Left = Right = Front = Back = coords;
        }

        // Assignment of a single Vector2Int to a blocks texture coords defaults all face's texture coords
        // to that single one
        public static implicit operator TextureCoords (Vector2Int v) => new TextureCoords(v);
    }
}