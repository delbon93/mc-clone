using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace BlockGame.Backend.World
{
    [Serializable]
    public struct FaceTextureCoordOverride
    {
        [SerializeField] public Vector2Int textureCoordinate;
        [SerializeField] public bool west;
        [SerializeField] public bool east;
        [SerializeField] public bool up;
        [SerializeField] public bool down;
        [SerializeField] public bool north;
        [SerializeField] public bool south;

        public bool IsDirectionSet (Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: return down;
                case Direction.East: return east;
                case Direction.North: return north;
                case Direction.South: return south;
                case Direction.Up: return up;
                case Direction.West: return west;
                default: return false;
            }
        }
    }
    
    [Serializable]
    public struct TextureCoords
    {
        [SerializeField] public Vector2Int defaultCoord;
        [SerializeField] public FaceTextureCoordOverride[] coordOverrides;

        public TextureCoords (Vector2Int coords)
        {
            defaultCoord = coords;
            coordOverrides = new FaceTextureCoordOverride[0];
        }

        public Vector2Int GetCoordForFace (Direction faceDirection)
        {
            for (var i = 0; i < coordOverrides.Length; i++)
            {
                if (coordOverrides[i].IsDirectionSet(faceDirection))
                    return coordOverrides[i].textureCoordinate;
            }
            return defaultCoord;
        }

        // Assignment of a single Vector2Int to a blocks texture coords defaults all face's texture coords
        // to that single one
        public static implicit operator TextureCoords (Vector2Int v) => new TextureCoords(v);

        public Vector2Int GetFrontal () => GetCoordForFace(Direction.South);
    }
}