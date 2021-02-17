using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
    /// <summary>
    /// x-Axis: West to East
    /// y-Axis: Down to Up
    /// z-Axis: South to North
    /// </summary>
    public enum Direction
    {
        // -x  <->  +x
        West, East, 
        // -y  <->  +y
        Down, Up, 
        // -z  <->  +z
        South, North
    }

    public static class OrthoDirExtensions
    {
        private static readonly Vector3Int V2IBack = new Vector3Int(0, 0, -1);
        private static readonly Vector3Int V2IForward = new Vector3Int(0, 0, 1);

        private static readonly List<Direction> OrthogonalDirections = new List<Direction>()
        {
            Direction.West, Direction.East, Direction.Up, Direction.Down, Direction.North, Direction.South
        };
        
        public static Vector3Int ToVector3Int (this Direction direction)
        {
            switch (direction)
            {
                case Direction.South: return V2IBack;
                case Direction.Down: return Vector3Int.down;
                case Direction.North: return V2IForward;
                case Direction.West: return Vector3Int.left;
                case Direction.East: return Vector3Int.right;
                case Direction.Up: return Vector3Int.up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static Direction Opposite (this Direction direction)
        {
            switch (direction)
            {
                case Direction.South: return Direction.North;
                case Direction.Down: return Direction.Up;
                case Direction.North: return Direction.South;
                case Direction.West: return Direction.East;
                case Direction.East: return Direction.West;
                case Direction.Up: return Direction.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static IEnumerable<Direction> AllOrthogonal => OrthogonalDirections;

        

    }
}