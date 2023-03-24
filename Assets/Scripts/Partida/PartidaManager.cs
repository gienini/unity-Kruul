using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartidaManager : MonoBehaviour
{
    [SerializeField] private GameObject cartaBasePrefab = null;
    [SerializeField] private GameObject fichaPrefab = null;
    [SerializeField] private GameObject dorsoPrefab = null;
    [SerializeField] private SO_Baraja so_baraja = null;
    [SerializeField] private int fichasJugadorProximo;
    [SerializeField] private int numeroCartasArrancables;
    [SerializeField] private GameObject fuegoArtificialPrefab = null;
    private Camera mainCamera;
    private Baraja _baraja;
    private bool esAnimacionFuegos = false;

    public Camera MainCamera { get => mainCamera; set => mainCamera = value; }
   

    private void OnEnable()
    {
        EventHandler.ClickEnTableroFase1Event += ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event += ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent += PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event += DespuesIntroFase1Event;
        EventHandler.JugadaEliminarEvent += callJugadaHechaFase2;
        EventHandler.FinalPartidaEvent += FinalPartidaEvent;
    }
    private void OnDisable()
    {
        EventHandler.ClickEnTableroFase1Event -= ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event -= ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent -= PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event -= DespuesIntroFase1Event;
        EventHandler.JugadaEliminarEvent -= callJugadaHechaFase2;
        EventHandler.FinalPartidaEvent -= FinalPartidaEvent;
    }

    private void FinalPartidaEvent(int arg1, int arg2)
    {
        esAnimacionFuegos = true;
        StartCoroutine(fuegos());
    }

    private void Awake()
    {
    }

    private void Start()
    {
        mainCamera = Camera.main;
        esAnimacionFuegos = false;
    }
    private void DespuesIntroFase1Event()
    {
        PrimeraCartaFase1();
    }

    private void DespuesFadeOutEvent()
    {
        if (PropiedadesCasillasManager.Instance.EsFase1)
            inicializaBaraja();
    }
    public void EmpiezaPartida()
    {
        EventHandler.CallEmpiezaFase1Event();
    }
    private void EmpiezaFase1Event()
    {
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
    }
    private IEnumerator fuegos()
    {
        while (esAnimacionFuegos)
        {
            GameObject nuevoFuegoArtificial = Instantiate(fuegoArtificialPrefab, new Vector3(Random.Range(- 15, 15), -10, transform.position.z), Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
        
    }
    private void inicializaBaraja()
    {
        _baraja = new Baraja(so_baraja.cartas);
    }
    private void ClickEnTableroFase2Event(GridCursorFase2 cursor, bool esClickEnFicha)
    {
        if (cursor.CartaGO != null)
        {
            if (PropiedadesCasillasManager.Instance.EsJugadaSeleccionaNodo)
            {
                //Jugador selecciona nodo
                Debug.Log("click SELECCIONAR nodo X" + cursor.GetGridPositionForCursor().x + " Y" + cursor.GetGridPositionForCursor().y);
                PropiedadesCasillasManager.Instance.seleccionaNodo(cursor.GetGridPositionForCursor());
            }
            else
            {
                //Tiene carta flotante
                Debug.Log("click PONER carta X" + cursor.GetGridPositionForCursor().x + " Y" + cursor.GetGridPositionForCursor().y);
                PonCartaEnTableroFase2(cursor.GetGridPositionForCursor(), cursor.CartaGO.GetComponent<Carta>());
            }
            
        }
        else if (esClickEnFicha)
        {
            Debug.Log("click QUITAR ficha X" + cursor.GetGridPositionForCursor().x + " Y" + cursor.GetGridPositionForCursor().y);
            //Selecciona ficha
            PropiedadesCasillasManager.Instance.EliminaPiezaEnTablero(PropiedadesCasillasManager.Instance.getPiezaEnPosicion(cursor.GetGridPositionForCursor()));
            callJugadaHechaFase2();
        }
        else
        {
            Debug.Log("click QUITAR carta X" + cursor.GetGridPositionForCursor().x + " Y" + cursor.GetGridPositionForCursor().y);
            //Selecciona carta
            Carta tmpCarta = PropiedadesCasillasManager.Instance.getCartaEnPosicion(cursor.GetGridPositionForCursor());
            cursor.SetCartaFlotante(tmpCarta);
            PropiedadesCasillasManager.Instance.EscondeCartaDePosicion(cursor.GetGridPositionForCursor());
        }

    }
    private void ClickEnTableroFase1Event(Vector3 posicion, bool esAccionCarta)
    {
        if (esAccionCarta)
        {
            //Colocar carta en tablero
            ValorCasilla valorCasilla = PropiedadesCasillasManager.Instance.GetValorEnCoordenada((int)posicion.x, (int)posicion.y);
            if (valorCasilla != null && valorCasilla.esTablero && PropiedadesCasillasManager.Instance.CheckPositionValidityFase1(Vector3Int.FloorToInt(posicion), false, PropiedadesCasillasManager.Instance.EsTurnoColor1))
            {
                PonCartaEnTableroFase1(posicion, _baraja.Count() == 0);
            }
        }
        else
        {
            //Colocar ficha en tablero
            PropiedadesCasillasManager.Instance.checkPuntoEnPosicion(true, posicion, PropiedadesCasillasManager.Instance.EsTurnoColor1, null, false);
            callJugadaHechaFase1();
        }
    }
    private void PonCartaEnTableroFase1(Vector3 posicion, bool esUltima)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3(posicion.x / 2, posicion.y, -mainCamera.transform.position.z);
        //Vector3 posicionFinal = new Vector3(posicion.x, posicion.y, posicion.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count();
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = _baraja.Pop();
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        cartaGO.name = "carta" + cartaGO.GetComponent<Carta>().OrdenCarta;
        //Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, -mainCamera.transform.position.z), cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
        callJugadaHechaFase1();
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
        cartaGO.GetComponent<Carta>().CartasVecinas = carta.CartasVecinas;
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        cartaGO.GetComponent<Carta>().PosicionTablero = carta.PosicionTablero;
        cartaGO.name = "carta" + cartaGO.GetComponent<Carta>().OrdenCarta;
        //Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), 0, null);
        PropiedadesCasillasManager.Instance.JugadaEliminar();
        SceneControllerManager.Instance.ToggleAcciones();
        callJugadaHechaFase2();
        
        
    }
    private void PuntoEnCuadranteEvent(List<ValorCasilla> cuadrante, bool esPuntoColor1)
    {
        //Posicion. Se pasa la X / 2 para la representacion visual en el tablero
        Vector3 posicionFinal = new Vector3((cuadrante[2].x )/2f, cuadrante[2].y, -mainCamera.transform.position.z);

        //Debug.Log("Crea Punto en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Punto en posicion x=" + cuadrante[2].x + " y=" + cuadrante[2].y);

        GameObject ficha;
        if (esPuntoColor1)
        {
            ficha = Instantiate(fichaPrefab, posicionFinal, Quaternion.identity);
            ficha.GetComponent<Ficha>().EsColor1 = true;
        }
        else
        {
            ficha = Instantiate(fichaPrefab, posicionFinal, Quaternion.identity);
            ficha.GetComponent<Ficha>().EsColor1 = false;
        }
        ficha.GetComponent<Ficha>().Cuadrante = cuadrante;
        PropiedadesCasillasManager.Instance.registraFichaEnTablero(cuadrante[2], ficha.GetComponent<Ficha>());
    }

    private void PrimeraCartaFase1()
    {
        string valoresPrimeraCarta = _baraja.Pop();
        //Posicion
        Vector3 posicionFinal = new Vector3(0, 0, -mainCamera.transform.position.z);
        //GameObject carta
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count()+1;
        cartaGO.GetComponent<Carta>().ValorCuartosCarta = valoresPrimeraCarta;
        cartaGO.GetComponent<Carta>().CartasVecinas = new List<int>();
        cartaGO.name = "carta" + cartaGO.GetComponent<Carta>().OrdenCarta;
        //Inicializar diccionario casillas
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        //LLamamos evento con el componente carta seteado por la baraja
        EventHandler.CallPopCartaEnPosicion(posicionFinal, cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
    }
    private void SetVecinosNuevaCarta(Carta cartaNueva, Vector3 posicionNueva)
    {
        //Quitamos referencia de antiguos vecinos
        
        foreach (Carta carta in PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Values)
        {
            if (cartaNueva.CartasVecinas.Contains(carta.OrdenCarta))
            {
                carta.CartasVecinas.Remove(cartaNueva.OrdenCarta);
            }
        }
        //Seteamos nuevos vecinos
        HashSet<int> idsVecinos = new HashSet<int>();
        foreach (Carta newVecina in PropiedadesCasillasManager.Instance.cartasEnCuadrantesOrtogonalesACoordenada((int)posicionNueva.x, (int)posicionNueva.y))
        {
            newVecina.CartasVecinas.Add(cartaNueva.OrdenCarta);
            idsVecinos.Add(newVecina.OrdenCarta);
        }
        cartaNueva.PosicionTablero = posicionNueva;
        cartaNueva.CartasVecinas = new List<int>(idsVecinos);
    }
    private Carta InstanciaCarta(string key, CartaSerializable cartaSerializable)
    {
        Vector3 posicionFinal = new Vector3(cartaSerializable.PosicionTablero.x / 2, cartaSerializable.PosicionTablero.y, -mainCamera.transform.position.z);
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().SetValoresFromSerializable(cartaSerializable);
        PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Add(key, cartaGO.GetComponent<Carta>());
        return cartaGO.GetComponent<Carta>();

    }
    private void callJugadaHechaFase1()
    {
        //Check final de baraja = final de fase1
        if (_baraja.Count() == 0)
        {
            EventHandler.CallAcabaFase1Event();
            int fichasPuestasJugador1 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ1;
            int fichasPuestasJugador2 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ2;
            //Check final de partida prematuro, si los dos jugadores no tienen ficha = empate
            if (fichasPuestasJugador1 == 0 && fichasPuestasJugador2 == 0)
            {
                SceneControllerManager.Instance.FadeAndKeepScene("EMPATE", true);
                EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            }
            //Check final de partida prematuro, si alguno de los dos jugadores no tienen ficha en tablero
            else if (fichasPuestasJugador1 == 0 || fichasPuestasJugador2 == 0)
            {
                bool esVictoriaJ1 = fichasPuestasJugador1 > 0;
                SceneControllerManager.Instance.FadeAndKeepScene("VICTORIA J" + (esVictoriaJ1 ? "1" : "2"), true);
                EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            }else
            {
                //Si no acaba partida, empieza fase 2
                SceneControllerManager.Instance.FadeAndKeepScene("FASE 2");
                EventHandler.CallEmpiezaFase2Event();
            }
        }
        EventHandler.CallJugadaHechaEvent();

    }
    private void callJugadaHechaFase2()
    {
        int fichasPuestasJugador1 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ1;
        int fichasPuestasJugador2 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ2;
        bool esProximoTurnoJ1 = !PropiedadesCasillasManager.Instance.EsTurnoColor1;
        //Final partida empate
        if (fichasPuestasJugador1 == 0 && fichasPuestasJugador2 == 0)
        {
            SceneControllerManager.Instance.FadeAndKeepScene("EMPATE", true);
            EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            return;
        }
        
        fichasJugadorProximo = esProximoTurnoJ1 ? fichasPuestasJugador1 : fichasPuestasJugador2;
        numeroCartasArrancables = 0;
        foreach (Carta c in PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Values)
        {
            if (c.gameObject.activeSelf && c.NumCuartosConFicha == 0)
            {
                numeroCartasArrancables++;
            }
        }
        
        //Final partida si el jugador siguiente (que ya esta seteado)
        if (fichasJugadorProximo == 0 || (fichasJugadorProximo == 1 && numeroCartasArrancables == 0))
        {
            SceneControllerManager.Instance.FadeAndKeepScene("VICTORIA J" + (!esProximoTurnoJ1 ? "1" : "2"), true);
            EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
        }else
        {
            EventHandler.CallJugadaHechaEvent();
        }
    }

}
