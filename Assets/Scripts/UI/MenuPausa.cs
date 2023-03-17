﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    private InputManager inputManager;
    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
    }
    public void BotonReiniciarPartida()
    {
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        SceneControllerManager.Instance.FadeAndLoadScene(Settings.NombreEscenaJuego);
        
    }

    public void BotonSalir()
    {
        SceneControllerManager.Instance.FadeAndLoadScene(NombresEscena.none.ToString());
    }

    public void BotonCancelar()
    {
        inputManager.AccionEscape();
    }

    public void BotonSalirDelJuego()
    {
        SceneControllerManager.Instance.Quit();
    }
}
