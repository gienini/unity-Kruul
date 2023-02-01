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
    private int _fichasPuestasJugador1 = 0;
    private int _fichasPuestasJugador2 = 0;
    //CuentasBarajas

    //test
    //[SerializeField] private GameObject cartaBasePrefab2 = null;
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
            EventHandler.CallPopCartaEnPosicion(posicionFinal, cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
            esFase1Cargada = false;
        }
    }
    private void ClickEnTableroEvent(Vector3 posicion)
    {
        ValorCasilla valorCasilla = PropiedadesCasillasManager.Instance.GetValorEnCoordenada((int)posicion.x, (int)posicion.y);
        if (valorCasilla != null && valorCasilla.esTablero)
        {
            //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
            Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, posicion.z);
            //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
            //GameObject carta
            GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
            cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
            //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
            EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
            Debug.Log("Crea Carta en posicion x=" + posicionFinal.x + " y=" + posicionFinal.y);
            EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
            _esTurnoJugador1 = !_esTurnoJugador1;
        }        
    }
    private void PuntoEnCuadranteEvent(List<ValorCasilla> cuadrante, bool esPuntoColor1)
    {
        Debug.Log("PUNTO cuadrante[1].x=" + cuadrante[1].x + " cuadrante[1].y="+ cuadrante[1].y);
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3((cuadrante[2].x )/2f, cuadrante[2].y, -mainCamera.transform.position.z);

        Debug.Log("POSFINAL X=" + posicionFinal.x + " Y="+posicionFinal.y);
        if (esPuntoColor1)
        {
            GameObject pieza = Instantiate(piezaPrefab, posicionFinal, Quaternion.identity);
            pieza.GetComponent<Pieza>().EsColor1 = true;
            _fichasPuestasJugador1++;
        }
        else
        {
            GameObject pieza = Instantiate(piezaPrefab, posicionFinal, Quaternion.identity);
            pieza.GetComponent<Pieza>().EsColor1 = false;
            _fichasPuestasJugador2++;
        }
        
    }



    private void EmpiezaFase1Event()
    {

        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        esFase1Cargada = true;

    }


    public void EmpiezaPartida()
    {
        EventHandler.CallEmpiezaFase1Event();
    }

}
