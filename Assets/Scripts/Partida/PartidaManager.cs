﻿using System.Collections;
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
    //CuentasFase

    private bool _esFase1 = false;
    private bool _esFase2 = false;

    //test
    //[SerializeField] private GameObject cartaBasePrefab2 = null;
    private bool _esTurnoJugador1 = true;
    private bool _esFase1PrimeraCarta = true;
    private void OnEnable()
    {
        EventHandler.ClickEnTableroFase1Event += ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event += ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent += PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.AcabaFase2Event += AcabaFase2Event;
    }
    private void OnDisable()
    {
        EventHandler.ClickEnTableroFase1Event -= ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event -= ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent -= PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.AcabaFase1Event -= AcabaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
        EventHandler.AcabaFase2Event -= AcabaFase2Event;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_esFase1PrimeraCarta)
        {
            _esFase1PrimeraCarta = false;
            PrimeraCartaFase1();
        }
    }
    private void ClickEnTableroFase2Event(GridCursorFase2 cursor, bool esClickEnPieza)
    {
        if (cursor.CartaGO != null)
        {
            //Tiene carta flotante
            PonCartaEnTableroFase2(cursor.GetGridPositionForCursor(), cursor.CartaGO.GetComponent<Carta>());
        }
        else if (esClickEnPieza)
        {
            //Selecciona ficha
            PropiedadesCasillasManager.Instance.EliminaPiezaEnTablero(PropiedadesCasillasManager.Instance.getPiezaEnPosicion(cursor.GetGridPositionForCursor()));
            EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
        }
        else
        {
            //Selecciona carta
            Carta tmpCarta = PropiedadesCasillasManager.Instance.getCartaEnPosicion(cursor.GetGridPositionForCursor());
            cursor.SetCartaFlotante(tmpCarta);
            PropiedadesCasillasManager.Instance.EscondeCartaDePosicion(cursor.GetGridPositionForCursor());
        }
        _esTurnoJugador1 = !_esTurnoJugador1;

    }
    private void ClickEnTableroFase1Event(Vector3 posicion)
    {
        ValorCasilla valorCasilla = PropiedadesCasillasManager.Instance.GetValorEnCoordenada((int)posicion.x, (int)posicion.y);
        if (valorCasilla != null && valorCasilla.esTablero)
        {
            PonCartaEnTableroFase1(posicion);
            if (_baraja.Count() == 0)
            {
                EventHandler.CallAcabaFase1Event();
                SceneControllerManager.Instance.FadeAndKeepScene("FASE 2");
                EventHandler.CallEmpiezaFase2Event();
            }
            _esTurnoJugador1 = !_esTurnoJugador1;
        }
    }
    private void PonCartaEnTableroFase1(Vector3 posicion)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, posicion.z);
        //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count();
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
        
        EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
    }

    private void PonCartaEnTableroFase2(Vector3 posicion, Carta carta)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, posicion.z);
        //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = carta.ValorCuartosCarta;
        cartaGO.GetComponent<Carta>().PosicionInicial = carta.PosicionInicial;
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), 0, null);

        EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);

        Destroy(carta.gameObject);
        PropiedadesCasillasManager.Instance.EliminarCartaFlotante();
        SceneControllerManager.Instance.ToggleAcciones();
    }
    private void PuntoEnCuadranteEvent(List<ValorCasilla> cuadrante, bool esPuntoColor1)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3((cuadrante[2].x )/2f, cuadrante[2].y, -mainCamera.transform.position.z);

        Debug.Log("Crea Punto en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        Debug.Log("Crea Punto en posicion x=" + cuadrante[2].x + " y=" + cuadrante[2].y);

        GameObject pieza;
        if (esPuntoColor1)
        {
            pieza = Instantiate(piezaPrefab, posicionFinal, Quaternion.identity);
            pieza.GetComponent<Pieza>().EsColor1 = true;
            _fichasPuestasJugador1++;
        }
        else
        {
            pieza = Instantiate(piezaPrefab, posicionFinal, Quaternion.identity);
            pieza.GetComponent<Pieza>().EsColor1 = false;
            _fichasPuestasJugador2++;
        }
        pieza.GetComponent<Pieza>().Cuadrante = cuadrante;
        PropiedadesCasillasManager.Instance.registraPiezaEnTablero(cuadrante[2], pieza.GetComponent<Pieza>());
    }

    private void PrimeraCartaFase1()
    {
        _baraja = new Baraja(so_baraja.cartas);

        //Posicion
        Vector3 posicionFinal = new Vector3(0, 0, -mainCamera.transform.position.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count();
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
        cartaGO.GetComponent<Carta>().CartasVecinas = new HashSet<int>();
        //Inicializar diccionario casillas
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        //LLamamos evento con el componente carta seteado por la baraja
        EventHandler.CallPopCartaEnPosicion(posicionFinal, cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
    }
    private void SetVecinosNuevaCarta(Carta cartaNueva, Vector3 posicion)
    {
        HashSet<int> idsVecinos = new HashSet<int>();
        foreach (Carta cartaVecina in PropiedadesCasillasManager.Instance.cartasEnCuadrantesOrtogonalesACoordenada((int)posicion.x, (int)posicion.y))
        {
            cartaVecina.CartasVecinas.Add(cartaNueva.OrdenCarta);
            idsVecinos.Add(cartaVecina.OrdenCarta);
        }
        cartaNueva.CartasVecinas = idsVecinos;
    }
    public void EmpiezaPartida()
    {
        EventHandler.CallEmpiezaFase1Event();
    }
    #region eventosFASE
    private void EmpiezaFase1Event()
    {
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        _esFase1 = true;
    }
    private void AcabaFase1Event()
    {
        _esFase1 = false;
    }

    private void EmpiezaFase2Event()
    {
        _esFase2 = true;
    }

    private void AcabaFase2Event()
    {
        _esFase2 = false;
    }
    #endregion
}
