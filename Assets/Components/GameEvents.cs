using System.Collections;
using System.Collections.Generic;
using BlockGame.Components;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public delegate void ToggleChunkBordersEvent ();
    public delegate void EnterChunkEvent (Vector3Int chunkGlobalIndex);
    public delegate void ExitChunkEvent (Vector3Int chunkGlobalIndex);
    public delegate void ChangeInventorySelectionEvent (short newBlockId);

    public static event ToggleChunkBordersEvent ToggleChunkBorders;
    public static event EnterChunkEvent EnterChunk;
    public static event ExitChunkEvent ExitChunk;
    public static event ChangeInventorySelectionEvent ChangeInventorySelection;

    public static void OnToggleChunkBorders () => ToggleChunkBorders?.Invoke();
    public static void OnEnterChunk (Vector3Int chunkGlobalIndex) => EnterChunk?.Invoke(chunkGlobalIndex);
    public static void OnExitChunk (Vector3Int chunkGlobalIndex) => ExitChunk?.Invoke(chunkGlobalIndex);
    public static void OnChangeInventorySelection (short newblockid) => ChangeInventorySelection?.Invoke(newblockid);
}
