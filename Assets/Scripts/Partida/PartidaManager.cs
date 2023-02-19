using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartidaManager : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject cartaBasePrefab = null;
    [SerializeField] private GameObject fichaPrefab = null;
    [SerializeField] private GameObject dorsoPrefab = null;
    [SerializeField] private SO_Baraja so_baraja = null;
    private Camera mainCamera;
    private Baraja _baraja;
    

    public Camera MainCamera { get => mainCamera; set => mainCamera = value; }
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }
    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    private void OnEnable()
    {
        EventHandler.ClickEnTableroFase1Event += ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event += ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent += PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event += DespuesIntroFase1Event;
        ISaveableRegister();
    }
    private void OnDisable()
    {
        EventHandler.ClickEnTableroFase1Event -= ClickEnTableroFase1Event;
        EventHandler.ClickEnTableroFase2Event -= ClickEnTableroFase2Event;
        EventHandler.PuntoEnCuadranteEvent -= PuntoEnCuadranteEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event -= DespuesIntroFase1Event;
        ISaveableDeregister();
    }

    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        mainCamera = Camera.main;
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
    private void inicializaBaraja()
    {
        _baraja = new Baraja(so_baraja.cartas);
    }
    private void ClickEnTableroFase2Event(GridCursorFase2 cursor, bool esClickEnFicha)
    {
        if (cursor.CartaGO != null)
        {
            //Tiene carta flotante
            PonCartaEnTableroFase2(cursor.GetGridPositionForCursor(), cursor.CartaGO.GetComponent<Carta>());
        }
        else if (esClickEnFicha)
        {
            //Selecciona ficha
            PropiedadesCasillasManager.Instance.EliminaPiezaEnTablero(PropiedadesCasillasManager.Instance.getPiezaEnPosicion(cursor.GetGridPositionForCursor()));
            callJugadaHechaFase2();
        }
        else
        {
            //Selecciona carta
            Carta tmpCarta = PropiedadesCasillasManager.Instance.getCartaEnPosicion(cursor.GetGridPositionForCursor());
            cursor.SetCartaFlotante(tmpCarta);
            PropiedadesCasillasManager.Instance.EscondeCartaDePosicion(cursor.GetGridPositionForCursor());
        }

    }

    private bool checkFinalPartidaFase2()
    {
        bool retorno = false;
        int fichasPuestasJugador1 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ1;
        int fichasPuestasJugador2 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ2;
        
        //Check final de partida prematuro, si los dos jugadores no tienen ficha = empate
        if (fichasPuestasJugador1 == 0 && fichasPuestasJugador2 == 0)
        {
            SceneControllerManager.Instance.FadeAndKeepScene("EMPATE");
            EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            retorno = true;
        }
        //Check final de partida prematuro, si alguno de los dos jugadores no tienen ficha en tablero
        else if (fichasPuestasJugador1 == 0 || fichasPuestasJugador2 == 0)
        {
            bool esVictoriaJ1 = fichasPuestasJugador1 > 0;
            SceneControllerManager.Instance.FadeAndKeepScene("VICTORIA J" + (esVictoriaJ1 ? "1" : "2"));
            EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            retorno = true;
        }
        //Check final partida en fase 2, si el jugador solo tiene una ficha puesta Y no hay cartas arrancables
        else if (PropiedadesCasillasManager.Instance.EsFase2)
        {
            int fichasJugadorActual = PropiedadesCasillasManager.Instance.EsTurnoColor1 ? fichasPuestasJugador1 : fichasPuestasJugador2;
            int numeroCartasArrancables = 0;
            foreach (Carta c in PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Values)
            {
                if (c.NumCuartosConFicha == 0)
                {
                    numeroCartasArrancables++;
                }
            }
            if (fichasJugadorActual == 1 && numeroCartasArrancables == 0)
            {
                SceneControllerManager.Instance.FadeAndKeepScene("VICTORIA J" + (!PropiedadesCasillasManager.Instance.EsTurnoColor1 ? "1" : "2"));
                EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
                retorno = true;
            }


        }
        return retorno;
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
                //Check final de baraja = final de fase1
                if (_baraja.Count() == 0)
                {
                    EventHandler.CallAcabaFase1Event();
                    if (!checkFinalPartidaFase2())
                    {
                        //Si no acaba partida, empieza fase 2
                        SceneControllerManager.Instance.FadeAndKeepScene("FASE 2");
                        EventHandler.CallEmpiezaFase2Event();
                    }
                }
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
        SetVecinosNuevaCarta(cartaGO.GetComponent<Carta>(), posicion);
        //Debug.Log("Crea Carta en posicionFinal x=" + posicionFinal.x + " y=" + posicionFinal.y);
        //Debug.Log("Crea Carta en posicion x=" + posicion.x + " y=" + posicion.y);
        //LLamamos evento con el componente carta seteado por la baraja. Pasar las coordenadas reales
        EventHandler.CallPopCartaEnPosicion(new Vector3(posicion.x, posicion.y, posicion.z), cartaGO.GetComponent<Carta>(), 0, null);
        PropiedadesCasillasManager.Instance.JugadaEliminar();
        SceneControllerManager.Instance.ToggleAcciones();
        callJugadaHechaFase2();
        //Representacion visual de la carta flotante
        Destroy(carta.gameObject);
        
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
        cartaGO.GetComponent<Carta>().CartasVecinas = new HashSet<int>();
        //Inicializar diccionario casillas
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
        //LLamamos evento con el componente carta seteado por la baraja
        EventHandler.CallPopCartaEnPosicion(posicionFinal, cartaGO.GetComponent<Carta>(), _baraja.Count(), _baraja.GetSiguiente());
    }
    private void SetVecinosNuevaCarta(Carta cartaNueva, Vector3 posicionNueva)
    {
        //Quitamos referencia de antiguos vecinos
        foreach (Carta oldVecina in PropiedadesCasillasManager.Instance.cartasEnCuadrantesOrtogonalesACoordenada((int)cartaNueva.PosicionTablero.x, (int)cartaNueva.PosicionTablero.y))
        {
            oldVecina.CartasVecinas.Remove(cartaNueva.OrdenCarta);
        }
        //Seteamos nuevos vecinos
        HashSet<int> idsVecinos = new HashSet<int>();
        foreach (Carta newVecina in PropiedadesCasillasManager.Instance.cartasEnCuadrantesOrtogonalesACoordenada((int)posicionNueva.x, (int)posicionNueva.y))
        {
            newVecina.CartasVecinas.Add(cartaNueva.OrdenCarta);
            idsVecinos.Add(newVecina.OrdenCarta);
        }
        cartaNueva.PosicionTablero = posicionNueva;
        cartaNueva.CartasVecinas = idsVecinos;
    }
    private Ficha InstanciaPieza(string key, FichaSerializable fichaSerializable)
    {
        Vector3 posicionFinal = new Vector3(fichaSerializable._cuadrante[2].x / 2, fichaSerializable._cuadrante[2].y, -mainCamera.transform.position.z);
        GameObject fichaGO = Instantiate(fichaPrefab, posicionFinal, Quaternion.identity);
        fichaGO.GetComponent<Ficha>().EsColor1 = fichaSerializable._esColor1;
        PropiedadesCasillasManager.Instance.DictCoordenadasFicha.Add(key, fichaGO.GetComponent<Ficha>());
        return fichaGO.GetComponent<Ficha>();
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
            if (!checkFinalPartidaFase2())
            {
                //Si no acaba partida, empieza fase 2
                SceneControllerManager.Instance.FadeAndKeepScene("FASE 2");
                EventHandler.CallEmpiezaFase2Event();
            }
        }
        //Check final de baraja = final de fase1
        if (_baraja.Count() == 0)
        {
            EventHandler.CallAcabaFase1Event();
            int fichasPuestasJugador1 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ1;
            int fichasPuestasJugador2 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ2;
            //Check final de partida prematuro, si los dos jugadores no tienen ficha = empate
            if (fichasPuestasJugador1 == 0 && fichasPuestasJugador2 == 0)
            {
                SceneControllerManager.Instance.FadeAndKeepScene("EMPATE");
                EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            }
            //Check final de partida prematuro, si alguno de los dos jugadores no tienen ficha en tablero
            else if (fichasPuestasJugador1 == 0 || fichasPuestasJugador2 == 0)
            {
                bool esVictoriaJ1 = fichasPuestasJugador1 > 0;
                SceneControllerManager.Instance.FadeAndKeepScene("VICTORIA J" + (esVictoriaJ1 ? "1" : "2"));
                EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
            }else
            {
                //Si no acaba partida, empieza fase 2
                SceneControllerManager.Instance.FadeAndKeepScene("FASE 2");
                EventHandler.CallEmpiezaFase2Event();
                return;
            }
        }
        EventHandler.CallJugadaHechaEvent();

    }
    private void callJugadaHechaFase2()
    {
        int fichasPuestasJugador1 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ1;
        int fichasPuestasJugador2 = PropiedadesCasillasManager.Instance.NumFichasPuestasJ2;
        int fichasJugadorActual = PropiedadesCasillasManager.Instance.EsTurnoColor1 ? fichasPuestasJugador1 : fichasPuestasJugador2;
        int numeroCartasArrancables = 0;
        foreach (Carta c in PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Values)
        {
            if (c.NumCuartosConFicha == 0)
            {
                numeroCartasArrancables++;
            }
        }
        //Final partida si el jugador siguiente (que ya esta seteado)
        if (fichasJugadorActual == 1 && numeroCartasArrancables == 0)
        {
            SceneControllerManager.Instance.FadeAndKeepScene("VICTORIA J" + (!PropiedadesCasillasManager.Instance.EsTurnoColor1 ? "1" : "2"));
            EventHandler.CallFinalPartidaEvent(fichasPuestasJugador1, fichasPuestasJugador2);
        }else
        {
            EventHandler.CallJugadaHechaEvent();
        }
    }

    #region
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave IsaveableSave()
    {
        SceneSave sceneSave = new SceneSave();
        GameObjectSave.sceneData.Remove(NombresEscena.Escena_PartidaNormal.ToString());
        sceneSave.boolDictionary = new Dictionary<string, bool>();

        if (_baraja._pilaCartas != null && _baraja._pilaCartas.Count > 0)
        {
            sceneSave.stringDictionary = new Dictionary<string, string>();
            for (int i = 0; i < _baraja._pilaCartas.Count; i++)
            {
                sceneSave.stringDictionary.Add(i.ToString(), _baraja._pilaCartas[i]);
            }
        }
        if (PropiedadesCasillasManager.Instance.DictCoordenadasCarta != null)
        {
            Dictionary<string, CartaSerializable> dictCoordenadasCartaSerializable = new Dictionary<string, CartaSerializable>();
            foreach (string key in PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Keys)
            {
                dictCoordenadasCartaSerializable.Add(key, new CartaSerializable(PropiedadesCasillasManager.Instance.DictCoordenadasCarta[key]));
            }
            sceneSave.dictCoordenadasCarta = dictCoordenadasCartaSerializable;
        }
        if (PropiedadesCasillasManager.Instance.DictCoordenadasFicha != null)
        {
            Dictionary<string, FichaSerializable> dictCoordenadasPiezaSerializable = new Dictionary<string, FichaSerializable>();
            foreach (string key in PropiedadesCasillasManager.Instance.DictCoordenadasFicha.Keys)
            {
                dictCoordenadasPiezaSerializable.Add(key, new FichaSerializable(PropiedadesCasillasManager.Instance.DictCoordenadasFicha[key]));
            }
            sceneSave.dictCoordenadasPieza = dictCoordenadasPiezaSerializable;
        }
        if (PropiedadesCasillasManager.Instance.CartasEscondidas != null)
        {
            List<CartaSerializable> cartasEscondidasSerializable = new List<CartaSerializable>();
            foreach (Carta c in PropiedadesCasillasManager.Instance.CartasEscondidas)
            {
                cartasEscondidasSerializable.Add(new CartaSerializable(c));
            }
            sceneSave.cartasEscondidas = cartasEscondidasSerializable;
        }
        if (PropiedadesCasillasManager.Instance.CartaEscondidaCursor != null)
        {
            sceneSave.cartaEscondidaCursor = new CartaSerializable(PropiedadesCasillasManager.Instance.CartaEscondidaCursor);
        }
        GameObjectSave.sceneData.Add(NombresEscena.Escena_PartidaNormal.ToString(), sceneSave);
        return GameObjectSave;
    }

    public void IsaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if (gameObjectSave.sceneData.TryGetValue(NombresEscena.Escena_PartidaNormal.ToString(), out SceneSave sceneSave))
            {

                if (sceneSave.stringDictionary != null && sceneSave.stringDictionary.Count > 0)
                {
                    List<string> cartasBaraja = new List<string>();
                    foreach (string cartaSave in sceneSave.stringDictionary.Values)
                    {
                        cartasBaraja.Add(cartaSave);
                    }
                    _baraja = new Baraja(cartasBaraja);
                }
                if (sceneSave.dictCoordenadasCarta != null && sceneSave.dictCoordenadasCarta.Count > 0)
                {
                    PropiedadesCasillasManager.Instance.DictCoordenadasCarta = new Dictionary<string, Carta>();
                    foreach (string key in sceneSave.dictCoordenadasCarta.Keys)
                    {
                        InstanciaCarta(key, sceneSave.dictCoordenadasCarta[key]);
                    }
                }
                if (sceneSave.dictCoordenadasPieza != null && sceneSave.dictCoordenadasPieza.Count > 0)
                {
                    PropiedadesCasillasManager.Instance.DictCoordenadasFicha = new Dictionary<string, Ficha>();
                    foreach (string key in sceneSave.dictCoordenadasPieza.Keys)
                    {
                        //InstanciaPieza(key, sceneSave.dictCoordenadasPieza[key]);
                    }
                }
                //if (sceneSave.cartasEscondidas != null && sceneSave.cartasEscondidas.Count > 0)
                //{
                //    foreach(CartaSerializable cartaSer in sceneSave.cartasEscondidas)
                //    {
                //        InstanciaCarta(cartaSer);
                //    }
                //}
                //private List<CartaSerializable> cartasEscondidasSerializable;
                //private CartaSerializable cartaEscondidaCursorSerializable;
            }
        }
    }

    public void IsaveableStoreScene(string sceneName)
    {
        //
    }

    public void IsaveableRestoreScene(string sceneName)
    {
        //
    }
    #endregion
}
