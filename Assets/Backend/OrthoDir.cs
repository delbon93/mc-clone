using System;
using UnityEngine;

namespace Backend
{
    public enum OrthoDir
    {
        Left, Right, Up, Down, Forward, Back
    }

    public static class OrthoDirExtensions
    {
        private static readonly Vector3Int V2IBack = new Vector3Int(0, 0, -1);
        private static readonly Vector3Int V2IForward = new Vector3Int(0, 0, 1);
        public static Vector3Int ToVector3Int (this OrthoDir orthoDir)
        {
            switch (orthoDir)
            {
                case OrthoDir.Back: return V2IBack;
                case OrthoDir.Down: return Vector3Int.down;
                case OrthoDir.Forward: return V2IForward;
                case OrthoDir.Left: return Vector3Int.left;
                case OrthoDir.Right: return Vector3Int.right;
                case OrthoDir.Up: return Vector3Int.up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orthoDir), orthoDir, null);
            }
        }
    }
}