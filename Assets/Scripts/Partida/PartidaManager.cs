using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartidaManager : MonoBehaviour
{
    [SerializeField] private GameObject cartaBasePrefab = null;
    [SerializeField] private GameObject piezaPrefab = null;
    private Camera mainCamera;

    //test
    [SerializeField] private GameObject cartaBasePrefab2 = null;
    private bool esTurno = true;
    private void OnEnable()
    {
        EventHandler.ClickEnTableroEvent += ClickEnTableroEvent;
        EventHandler.PuntoEnCuadranteEvent += PuntoEnCuadranteEvent;
    }

    private void OnDisable()
    {
        EventHandler.ClickEnTableroEvent -= ClickEnTableroEvent;
        EventHandler.PuntoEnCuadranteEvent -= PuntoEnCuadranteEvent;
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void ClickEnTableroEvent(Vector3 posicion)
    {
        ValorCasilla valorCasilla = PropiedadesCasillasManager.Instance.GetValorEnCoordenada((int)posicion.x, (int)posicion.y);
        if (valorCasilla != null && valorCasilla.esTablero)
        {
            Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, posicion.z);
            if (esTurno)
            {
                Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
                EventHandler.CallPopCartaEnPosicion(posicion, cartaBasePrefab.GetComponent<Carta>());
            }
            else
            {
                Instantiate(cartaBasePrefab2, posicionFinal, Quaternion.identity);
                EventHandler.CallPopCartaEnPosicion(posicion, cartaBasePrefab2.GetComponent<Carta>());
            }
            Debug.Log("Crea Carta en posicion x=" + posicionFinal.x + " y=" + posicionFinal.y);
            esTurno = !esTurno;
        }        
    }
    private void PuntoEnCuadranteEvent(List<ValorCasilla> cuadrante, bool esPuntoColor1)
    {
        Debug.Log("PUNTO cuadrante[2].x=" + cuadrante[2].x + " cuadrante[2].y="+ cuadrante[2].y);
        Vector3 posicionFinal = new Vector3((cuadrante[2].x )/2, cuadrante[2].y , -mainCamera.transform.position.z);
        //Vector3 posicionFinal = new Vector3((cuadrante[2].x / 2) - 0.5f, cuadrante[2].y - 0.5f, -mainCamera.transform.position.z);

        Debug.Log("POSFINAL X=" + posicionFinal.x + " Y="+posicionFinal.y);
        Instantiate(piezaPrefab, posicionFinal, Quaternion.identity);
    }

}
