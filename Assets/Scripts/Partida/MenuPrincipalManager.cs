using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPrincipalManager : MonoBehaviour
{
    private CanvasGroup _canvasGroupComponent;
    private bool _esMenuPrincipalCargado = false;
    private void Start()
    {
        _canvasGroupComponent = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        EventHandler.AntesFadeOutEvent += AntesFadeOutEvent;
        EventHandler.MenuPrincipalEvent += MenuPrincipalEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
    }
    private void OnDisable()
    {
        EventHandler.AntesFadeOutEvent -= AntesFadeOutEvent;
        EventHandler.MenuPrincipalEvent -= MenuPrincipalEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
    }

    public void BotonEmpezarJuego()
    {
        SceneControllerManager.Instance.FadeAndLoadScene(Settings.NombreEscenaJuego);
    }

    private void EmpiezaFase1Event()
    {
        _esMenuPrincipalCargado = false;
    }

    private void MenuPrincipalEvent()
    {
        _esMenuPrincipalCargado = true;
    }
    private void AntesFadeOutEvent()
    {
        //gameObject.SetActive(false);
    }
}
