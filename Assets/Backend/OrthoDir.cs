using System;
using System.Collections.Generic;
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

        private static readonly List<OrthoDir> AllDirs = new List<OrthoDir>()
        {
            OrthoDir.Left, OrthoDir.Right, OrthoDir.Up, OrthoDir.Down, OrthoDir.Forward, OrthoDir.Back
        };
        
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

        public static OrthoDir Opposite (this OrthoDir orthoDir)
        {
            switch (orthoDir)
            {
                case OrthoDir.Back: return OrthoDir.Forward;
                case OrthoDir.Down: return OrthoDir.Up;
                case OrthoDir.Forward: return OrthoDir.Back;
                case OrthoDir.Left: return OrthoDir.Right;
                case OrthoDir.Right: return OrthoDir.Left;
                case OrthoDir.Up: return OrthoDir.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orthoDir), orthoDir, null);
            }
        }

        public static List<OrthoDir> All => AllDirs;

    }
}