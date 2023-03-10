using System.Collections;
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
        SceneControllerManager.Instance.FadeAndLoadScene(NombresEscena.Escena_PartidaNormal.ToString());
        
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

    public void BotonGuardar()
    {
        SaveLoadManager.Instance.SaveDataToFile();
    }

    public void BotonCargar()
    {
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        SceneControllerManager.Instance.FadeAndLoadSceneCargar(NombresEscena.Escena_PartidaNormal.ToString());
    }
}
