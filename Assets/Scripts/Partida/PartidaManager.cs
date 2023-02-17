using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartidaManager : MonoBehaviour, ISaveable
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

    private void Update()
    {
    }
    private void DespuesIntroFase1Event()
    {
        PrimeraCartaFase1();
    }

    private void DespuesFadeOutEvent()
    {
        if (PropiedadesCasillasManager.Instance.EsFase1)
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
            EventHandler.CallJugadaHechaEvent();
        }
        else
        {
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
            ValorCasilla valorCasilla = PropiedadesCasillasManager.Instance.GetValorEnCoordenada((int)posicion.x, (int)posicion.y);
        if (valorCasilla != null && valorCasilla.esTablero && PropiedadesCasillasManager.Instance.CheckPositionValidityFase1(Vector3Int.FloorToInt(posicion), false, PropiedadesCasillasManager.Instance.EsTurnoColor1))
        {
            PonCartaEnTableroFase1(posicion, _baraja.Count() == 0);
            if (_baraja.Count() == 0)
            {
                EventHandler.CallAcabaFase1Event();
                EventHandler.CallJugadaHechaEvent();
                SceneControllerManager.Instance.FadeAndKeepScene("FASE 2");
                EventHandler.CallEmpiezaFase2Event();
            }
        }
        }else
        {
            PropiedadesCasillasManager.Instance.checkPuntoEnPosicion(true, posicion, PropiedadesCasillasManager.Instance.EsTurnoColor1, null, false);
            EventHandler.CallJugadaHechaEvent();
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
        if (!esUltima)
        {
            EventHandler.CallJugadaHechaEvent();
        }
        
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
        EventHandler.CallJugadaHechaEvent();
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
        cartaGO.GetComponent<Carta>().OrdenCarta = _baraja.Count()+1;
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
    private Pieza InstanciaPieza(string key, PiezaSerializable piezaSerializable)
    {
        Vector3 posicionFinal = new Vector3(piezaSerializable._cuadrante[2].x / 2, piezaSerializable._cuadrante[2].y, -mainCamera.transform.position.z);
        GameObject piezaGO = Instantiate(piezaPrefab, posicionFinal, Quaternion.identity);
        piezaGO.GetComponent<Pieza>().EsColor1 = piezaSerializable._esColor1;
        PropiedadesCasillasManager.Instance.DictCoordenadasPieza.Add(key, piezaGO.GetComponent<Pieza>());
        return piezaGO.GetComponent<Pieza>();
    }
    private Carta InstanciaCarta(string key, CartaSerializable cartaSerializable)
    {
        Vector3 posicionFinal = new Vector3(cartaSerializable.PosicionTablero.x / 2, cartaSerializable.PosicionTablero.y, -mainCamera.transform.position.z);
        GameObject cartaGO = Instantiate(cartaBasePrefab, posicionFinal, Quaternion.identity);
        cartaGO.GetComponent<Carta>().SetValoresFromSerializable(cartaSerializable);
        PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Add(key, cartaGO.GetComponent<Carta>());
        return cartaGO.GetComponent<Carta>();

    }


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
        if(PropiedadesCasillasManager.Instance.DictCoordenadasCarta != null)
        {
            Dictionary<string, CartaSerializable>  dictCoordenadasCartaSerializable = new Dictionary<string, CartaSerializable>();
            foreach (string key in PropiedadesCasillasManager.Instance.DictCoordenadasCarta.Keys)
            {
                dictCoordenadasCartaSerializable.Add(key, new CartaSerializable(PropiedadesCasillasManager.Instance.DictCoordenadasCarta[key]));
            }
            sceneSave.dictCoordenadasCarta = dictCoordenadasCartaSerializable;
        }
        if (PropiedadesCasillasManager.Instance.DictCoordenadasPieza != null)
        {
            Dictionary<string, PiezaSerializable>  dictCoordenadasPiezaSerializable = new Dictionary<string, PiezaSerializable>();
            foreach (string key in PropiedadesCasillasManager.Instance.DictCoordenadasPieza.Keys)
            {
                dictCoordenadasPiezaSerializable.Add(key, new PiezaSerializable(PropiedadesCasillasManager.Instance.DictCoordenadasPieza[key]));
            }
            sceneSave.dictCoordenadasPieza = dictCoordenadasPiezaSerializable;
        }
        if (PropiedadesCasillasManager.Instance.CartasEscondidas != null)
        {
            List<CartaSerializable>  cartasEscondidasSerializable = new List<CartaSerializable>();
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
                    foreach(string cartaSave in sceneSave.stringDictionary.Values)
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
                    PropiedadesCasillasManager.Instance.DictCoordenadasPieza = new Dictionary<string, Pieza>();
                    foreach (string key in sceneSave.dictCoordenadasPieza.Keys)
                    {
                        InstanciaPieza(key, sceneSave.dictCoordenadasPieza[key]);
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
    private void EmpiezaFase1Event()
    {
        PropiedadesCasillasManager.Instance.InicializaDictValoresCasilla();
    }
}
