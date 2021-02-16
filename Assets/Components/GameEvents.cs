using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public delegate void ToggleChunkBordersEvent ();

    public static event ToggleChunkBordersEvent ToggleChunkBorders;

    public static void OnToggleChunkBorders ()
    {
        ToggleChunkBorders?.Invoke();
    }
}
