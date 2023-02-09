using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartidaManager : MonoBehaviour
{
    [SerializeField] private GameObject cartaBasePrefab = null;
    [SerializeField] private GameObject piezaPrefab = null;
    [SerializeField] private GameObject dorsoPrefab = null;
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
    List<GameObject> listDorsos;

    public Camera MainCamera { get => mainCamera; set => mainCamera = value; }

    private void OnEnable()
    {
        EventHandler.ClickEnTableroFase1Event += ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event += ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent += PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.AcabaFase2Event += AcabaFase2Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event += DespuesIntroFase1Event;
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
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event -= DespuesIntroFase1Event;
    }

    

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
    }
    private void DespuesIntroFase1Event()
    {
        PrimeraCartaFase1();
    }

    private void DespuesFadeOutEvent()
    {
        if (_esFase1)
        {
            inicializaBaraja();

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
    private void ClickEnTableroFase1Event(Vector3 posicion, bool esAccionCarta)
    {
        if (esAccionCarta)
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
        }else
        {
            PropiedadesCasillasManager.Instance.checkPuntoEnPosicion(true, posicion, _esTurnoJugador1, null, false);
            EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
            _esTurnoJugador1 = !_esTurnoJugador1;
        }
    }
    private void PonCartaEnTableroFase1(Vector3 posicion)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, -mainCamera.transform.position.z);
        //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count();
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        //Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, -mainCamera.transform.position.z), cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
        
        EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
    }

    private void PonCartaEnTableroFase2(Vector3 posicion, Carta carta)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, -mainCamera.transform.position.z);
        //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = carta.OrdenCarta;
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = carta.ValorCuartosCarta;
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        //Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), 0, null);
        PropiedadesCasillasManager.Instance.JugadaEliminar();
        SceneControllerManager.Instance.ToggleAcciones();
        EventHandler.CallJugadaHechaEvent(_esTurnoJugador1);
        //Representacion visual de la carta flotante
        Destroy(carta.gameObject);
        
    }
    private void PuntoEnCuadranteEvent(List<ValorCasilla> cuadrante, bool esPuntoColor1)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3((cuadrante[2].x )/2f, cuadrante[2].y, -mainCamera.transform.position.z);

        //Debug.Log("Crea Punto en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Punto en posicion x=" + cuadrante[2].x + " y=" + cuadrante[2].y);

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
        string valoresPrimeraCarta = _baraja.Pop();
        //Posicion
        Vector3 posicionFinal = new Vector3(0, 0, -mainCamera.transform.position.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count();
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = valoresPrimeraCarta;
        cartaGO.GetComponent<Carta>().CartasVecinas = new HashSet<int>();
        //Inicializar diccionario casillas
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        //LLamamos evento con el componente carta seteado por la baraja
        EventHandler.CallPopCartaEnPosicion(posicionFinal, cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
    }
    private void SetVecinosNuevaCarta(Carta cartaNueva, Vector3 posicion)
    {
        //Quitamos referencia de antiguos vecinos
        foreach (Carta oldVecina in PropiedadesCasillasManager.Instance.cartasEnCuadrantesOrtogonalesACoordenada((int)cartaNueva.PosicionTablero.x, (int)cartaNueva.PosicionTablero.y))
        {
            oldVecina.CartasVecinas.Remove(cartaNueva.OrdenCarta);
        }
        //Seteamos nuevos vecinos
        HashSet<int> idsVecinos = new HashSet<int>();
        foreach (Carta newVecina in PropiedadesCasillasManager.Instance.cartasEnCuadrantesOrtogonalesACoordenada((int)posicion.x, (int)posicion.y))
        {
            newVecina.CartasVecinas.Add(cartaNueva.OrdenCarta);
            idsVecinos.Add(newVecina.OrdenCarta);
        }
        cartaNueva.PosicionTablero = posicion;
        cartaNueva.CartasVecinas = idsVecinos;
    }
    public void EmpiezaPartida()
    {
        EventHandler.CallEmpiezaFase1Event();
    }
    
    private void inicializaBaraja()
    {
        _baraja = new Baraja(so_baraja.cartas);
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
