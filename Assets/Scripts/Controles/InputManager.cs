using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridCursor gridCursor = null;
    private bool _esMenuPausa = false;
    private void Update()
    {
        if (SceneControllerManager.Instance.EscenaActual == NombresEscena.Escena_PartidaNormal.ToString())
        {
            ControlesPartida();
        }
    }
    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFaseNuevaEvent;
        EventHandler.EmpiezaFase2Event += EmpiezaFaseNuevaEvent;
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFaseNuevaEvent;
        EventHandler.EmpiezaFase2Event -= EmpiezaFaseNuevaEvent;
    }

    private void EmpiezaFaseNuevaEvent()
    {
        _esMenuPausa = false;
    }

    private void ControlesPartida()
    {
        if (!_esMenuPausa)
        {
            if (Input.GetMouseButton(0) && gridCursor.CursorPositionIsValid)
            {
                Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                EventHandler.CallClickEnTableroEvent(cursorGridPosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AccionEscape();
        }
    }

    public void AccionEscape()
    {
        _esMenuPausa = !_esMenuPausa;
        SceneControllerManager.Instance.ToggleMenuPausa();
    }
}
