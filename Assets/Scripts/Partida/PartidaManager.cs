using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartidaManager : MonoBehaviour
{
    [SerializeField] private GameObject cartaBasePrefab = null;
    [SerializeField] private GameObject piezaPrefab = null;
    [SerializeField] private SO_Baraja so_baraja = null;
    private Camera mainCamera;
    private Baraja _baraja;
    //CuentasJugadores
    private int _puntosJugador1 = 0;
    private int _puntosJugador2 = 0;
    private int _fichasPuestasJugador1 = 0;
    private int _fichasPuestasJugador2 = 0;
    //CuentasBarajas

    //test
    [SerializeField] private GameObject cartaBasePrefab2 = null;
    private bool _esTurnoJugador1 = true;
    private bool esFase1Cargada = false;
    private void OnEnable()
    {
        EventHandler.ClickEnTableroEvent += ClickEnTableroEvent;
        EventHandler.PuntoEnCuadranteEvent += PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
    }
    private void OnDisable()
    {
        EventHandler.ClickEnTableroEvent -= ClickEnTableroEvent;
        EventHandler.PuntoEnCuadranteEvent -= PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
    }
    private void Start()
    {
        mainCamera = Camera.main;
        EventHandler.CallEmpiezaFase1Event();
    }

    private void Update()
    {
        if (esFase1Cargada)
        {
            _baraja = new Baraja(so_baraja.cartas);

            //Posicion
            Vector3 posicionFinal = new Vector3(0, 0, -mainCamera.transform.position.z);
            //GameObject carta
            GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
            cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
            //Inicializar diccionario casillas
            PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
            //LLamamos evento con el componente carta seteado por la baraja
            EventHandler.CallPopCartaEnPosicion(posicionFinal, cartaGO.GetComponent<Carta>(), _baraja.Count());
            esFase1Cargada = false;
        }
    }
    private void ClickEnTableroEvent(Vector3 posicion)
    {
        ValorCasilla valorCasilla = PropiedadesCasillasManager.Instance.GetValorEnCoordenada((int)posicion.x, (int)posicion.y);
        if (valorCasilla != null && valorCasilla.esTablero)
        {
            //Posicion
            Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, posicion.z);
            //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
            //GameObject carta
            GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
            cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
            //LLamamos evento con el componente carta seteado por la baraja
            EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), _baraja.Count());
            Debug.Log("Crea Carta en posicion x=" + posicionFinal.x + " y=" + posicionFinal.y);
            EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
            _esTurnoJugador1 = !_esTurnoJugador1;
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



    private void EmpiezaFase1Event()
    {

        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        esFase1Cargada = true;

    }


}
