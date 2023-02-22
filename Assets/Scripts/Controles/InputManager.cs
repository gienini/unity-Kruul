using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridCursorFase1 gridCursorFase1 = null;
    [SerializeField] private GridCursorFase2 gridCursorFase2 = null;
    private bool _esMenuPausa = false;
    private bool _esClicando = false;
    private void Update()
    {
        if (SceneControllerManager.Instance.EscenaActual == NombresEscena.Escena_PartidaNormal.ToString())
        {
            if (PropiedadesCasillasManager.Instance.EsFase1)
            {
                ControlesFase1();
            }else if (PropiedadesCasillasManager.Instance.EsFase2)
            {
                ControlesFase2();
            }
            
        }
    }
    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
    }
    private void EmpiezaFase1Event()
    {
        EmpiezaFaseNuevaEvent();
    }

    private void EmpiezaFase2Event()
    {
        EmpiezaFaseNuevaEvent();
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
                EventHandler.CallClickEnTableroFase1Event(cursorGridPosition, !gridCursorFase1.EsCursorPieza);
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
            if (Input.GetMouseButtonDown(0) && gridCursorFase2.CursorPositionIsValid)
            {
                //EventHandler.CallClickEnTableroFase2Event(gridCursorFase2, gridCursorFase2.CursorPositionIsPieza);
                StartCoroutine(callClickRoutine());
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AccionEscape();
        }
    }
    private IEnumerator callClickRoutine()
    {
        if (!_esClicando)
        {
            _esClicando = true;
            EventHandler.CallClickEnTableroFase2Event(gridCursorFase2, gridCursorFase2.CursorPositionIsPieza);
            gridCursorFase2.CursorIsEnabled = false;
            yield return new WaitForSeconds(0.1f);
            gridCursorFase2.CursorIsEnabled = true;
            _esClicando = false;
            yield return null;
        }
        
    }

    public void AccionEscape()
    {
        _esMenuPausa = !_esMenuPausa;
        SceneControllerManager.Instance.ToggleMenuPausa();
    }
}
