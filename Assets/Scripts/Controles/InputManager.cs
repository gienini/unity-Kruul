using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridCursorFase1 gridCursorFase1 = null;
    [SerializeField] private GridCursorFase2 gridCursorFase2 = null;
    private bool _esMenuPausa = false;
    private bool _esFase1 = false;
    private bool _esFase2 = false;
    private void Update()
    {
        if (SceneControllerManager.Instance.EscenaActual == NombresEscena.Escena_PartidaNormal.ToString())
        {
            if (_esFase1)
            {
                ControlesFase1();
            }else if (_esFase2)
            {
                ControlesFase2();
            }
            
        }
    }
    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.AcabaFase2Event += AcabaFase2Event;
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
        EventHandler.AcabaFase1Event -= AcabaFase1Event;
        EventHandler.AcabaFase2Event -= AcabaFase2Event;
    }
    private void AcabaFase1Event()
    {
        _esFase1 = false;
    }
    private void AcabaFase2Event()
    {
        _esFase2 = false;
    }
    private void EmpiezaFase1Event()
    {
        EmpiezaFaseNuevaEvent();
        _esFase1 = true;
    }

    private void EmpiezaFase2Event()
    {
        EmpiezaFaseNuevaEvent();
        _esFase2 = true;
    }

    private void EmpiezaFaseNuevaEvent()
    {
        _esMenuPausa = false;
    }

    private void ControlesFase1()
    {
        if (!_esMenuPausa)
        {
            if (Input.GetMouseButtonDown(0) && gridCursorFase1.CursorPositionIsValid)
            {
                
                Vector3Int cursorGridPosition = gridCursorFase1.GetGridPositionForCursor();
                gridCursorFase1.CongelaYEsperaCarta();
                EventHandler.CallClickEnTableroFase1Event(cursorGridPosition);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AccionEscape();
        }
    }

    private void ControlesFase2()
    {
        if (!_esMenuPausa)
        {
            if (Input.GetMouseButton(0) && gridCursorFase2.CursorPositionIsValid)
            {
                EventHandler.CallClickEnTableroFase2Event(gridCursorFase2, gridCursorFase2.CursorPositionIsPieza);
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
