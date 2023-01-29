using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridCursor gridCursor = null;
    private void Update()
    {
        if (Input.GetMouseButton(0) && gridCursor.CursorPositionIsValid)
        {
            Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
            EventHandler.CallClickEnTableroEvent(cursorGridPosition);
        }
    }
}
