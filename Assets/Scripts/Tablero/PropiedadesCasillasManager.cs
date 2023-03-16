using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropiedadesCasillasManager : SingletonMonobehaviour<PropiedadesCasillasManager>, ISaveable
{
    private Dictionary<string, ValorCasilla> _dictValoresCasilla;
    private Dictionary<string, Carta> _dictCoordenadasCarta;
    private Dictionary<string, Ficha> _dictCoordenadasFicha;
    private bool _esDictCargado = false;
    private Carta _cartaEscondidaCursor;
    private List<ValorCasilla> _cuadranteEscondidoCursor;
    private float _cuadranteEscondidoCursorX;
    private float _cuadranteEscondidoCursorY;
    private List<Carta> _cartasEscondidas;
    private List<Ficha> _fichasEscondidas;
    private List<List<ValorCasilla>> _cuadrantesEscondidos;
    [SerializeField] private SO_PropiedadesCasilla[] propiedadesCasillaArray = null;

    private bool _esTurnoColor1;
    private bool _esFase1 = false;
    private bool _esFase2 = false;
    private int _numFichasPuestasJ1 = 0;
    private int _numFichasPuestasJ2 = 0;
    private bool _esJugadaSeleccionaNodo = false;
    public List<Carta>[] CartasSeleccionaNodo;

    //SAVE
    private string _iSaveableUniqueID;

    public GameObjectSave _gameObjectSave;
    public bool EsDictCargado { get => _esDictCargado; set => _esDictCargado = value; }
    public Dictionary<string, Carta> DictCoordenadasCarta { get => _dictCoordenadasCarta; set => _dictCoordenadasCarta = value; }
    public Dictionary<string, Ficha> DictCoordenadasFicha { get => _dictCoordenadasFicha; set => _dictCoordenadasFicha = value; }
    public List<Carta> CartasEscondidas { get => _cartasEscondidas; set => _cartasEscondidas = value; }
    public Carta CartaEscondidaCursor { get => _cartaEscondidaCursor; set => _cartaEscondidaCursor = value; }

    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    public bool EsTurnoColor1 { get => _esTurnoColor1; set => _esTurnoColor1 = value; }
    public bool EsFase1 { get => _esFase1; set => _esFase1 = value; }
    public bool EsFase2 { get => _esFase2; set => _esFase2 = value; }
    public int NumFichasPuestasJ1 { get => _numFichasPuestasJ1; set => _numFichasPuestasJ1 = value; }
    public int NumFichasPuestasJ2 { get => _numFichasPuestasJ2; set => _numFichasPuestasJ2 = value; }
    public bool EsJugadaSeleccionaNodo { get => _esJugadaSeleccionaNodo; set => _esJugadaSeleccionaNodo = value; }

    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += RegistraCartaEnPosicion;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.AcabaFase2Event += AcabaFase2Event;
    }

    private void OnDisable()
    {
        EventHandler.PopCartaEnPosicionEvent -= RegistraCartaEnPosicion;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
        EventHandler.AcabaFase1Event -= AcabaFase1Event;
        EventHandler.AcabaFase2Event -= AcabaFase2Event;
        ISaveableDeregister();
    }

    private void AcabaFase1Event()
    {
        _esFase1 = false;
    }

    private void EmpiezaFase1Event()
    {
        _esFase1 = true;
        _esTurnoColor1 = true;
    }

    private void EmpiezaFase2Event()
    {
        _esFase2 = true;
    }

    private void AcabaFase2Event()
    {
        _esFase2 = false;
    }

    protected override void Awake()
    {
        base.Awake();
        _iSaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        _gameObjectSave = new GameObjectSave();
    }
    public List<ValorCasilla> CheckPuntoEnTablero(bool esTurnoColor1)
    {
        List<ValorCasilla> retorno = new List<ValorCasilla>();
        foreach (ValorCasilla casilla in _dictValoresCasilla.Values)
        {
            if (!_dictCoordenadasFicha.TryGetValue(GeneraKey(casilla.x, casilla.y), out Ficha p) && checkPuntoEnPosicion(false, new Vector3(casilla.x, casilla.y), esTurnoColor1, null, false))
            {
                retorno.Add(casilla);
            }
        }
        return retorno;
    }
    public void EscondeCartaDePosicion(Vector3 posicion)
    {
        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(posicion.x, posicion.y), out Carta carta))
        {
            bool esToggle = true;
            _cartasEscondidas = new List<Carta>();
            _fichasEscondidas = new List<Ficha>();
            _cuadrantesEscondidos = new List<List<ValorCasilla>>();
            Dictionary<int, Carta> dictCartasPorOrden = new Dictionary<int, Carta>();
            //Seteamos carta flotante
            escondeCarta(carta, true);
            if (carta.CartasVecinas.Count > 1)
            {
                foreach (Carta cartaDict in _dictCoordenadasCarta.Values)
                {
                    if (dictCartasPorOrden.TryGetValue(cartaDict.OrdenCarta, out Carta c))
                    {
                        Debug.LogError("ERROR");
                    }else
                    {
                        dictCartasPorOrden.Add(cartaDict.OrdenCarta, cartaDict);
                    }
                }
                Dictionary<int, HashSet<int>> treeNodoVecinos = new Dictionary<int, HashSet<int>>();
                HashSet<int> resultadoFetch;
                int maxSize = 0;
                foreach (int indiceVecina in carta.CartasVecinas)
                {
                    resultadoFetch = new HashSet<int>();
                    resultadoFetch.Add(carta.OrdenCarta);
                    HashSet<int> set = fetchNodos(indiceVecina, resultadoFetch, dictCartasPorOrden);
                    if (set.Count > maxSize)
                    {
                        maxSize = set.Count;
                    }
                    treeNodoVecinos.Add(indiceVecina, set);
                }
                Dictionary<int, SortedSet<int>> nodosMantener = new Dictionary<int, SortedSet<int>>();
                foreach (int ordenNodo in treeNodoVecinos.Keys)
                {
                    if (treeNodoVecinos.TryGetValue(ordenNodo, out HashSet<int> set))
                    {
                        if (set.Count < maxSize)
                        {
                            //Se esconde el arbol de nodos
                            foreach (int ordenCarta in set)
                            {
                                if (dictCartasPorOrden.TryGetValue(ordenCarta, out Carta cartaEsconder))
                                {
                                    escondeCarta(cartaEsconder, false);
                                }
                            }
                        }
                        else
                        {
                            //Se mantiene el arbol de nodos
                            nodosMantener.Add(ordenNodo, new SortedSet<int>(set));
                        }
                    }
                }
                //Hay mas de un arbol de nodos para mantener
                if (nodosMantener.Keys.Count > 1)
                {
                    HashSet<SortedSet<int>> setArboles = new HashSet<SortedSet<int>>();
                    foreach (SortedSet<int> arbolNodos in nodosMantener.Values)
                    {
                        if (!setArboles.Contains(arbolNodos))
                        {
                            setArboles.Add(arbolNodos);
                        }

                    }
                    //no son todos iguales
                    if (setArboles.Count > 1)
                    {
                        esToggle = false;
                        //jugador seleccione nodo
                        Debug.Log("JUGADOR SELECCIONA NODO");
                        _esJugadaSeleccionaNodo = true;
                        int indice = 0;
                        CartasSeleccionaNodo = new List<Carta>[2];
                        foreach (SortedSet<int> arbol in setArboles)
                        {
                            CartasSeleccionaNodo[indice] = new List<Carta>();
                            foreach(int i in arbol)
                            {
                                if (dictCartasPorOrden.TryGetValue(i, out Carta c))
                                {
                                    CartasSeleccionaNodo[indice].Add(c);
                                }
                                
                            }
                            indice++;
                        }
                        //EventHandler.CallJugadorSeleccionaNodoEvent();
                    }
                }
            }
            if (esToggle)
            {
                SceneControllerManager.Instance.ToggleAcciones();
            }
        }
    }
    private void escondeCarta(Carta carta, bool esCartaFlotante)
    {
        if (esCartaFlotante)
        {
            _cartaEscondidaCursor = carta;
            _cuadranteEscondidoCursor = RemoveCuadranteEnDict(carta.PosicionTablero);
            _cuadranteEscondidoCursorX = carta.PosicionTablero.x;
            _cuadranteEscondidoCursorY = carta.PosicionTablero.y;
        }
        else
        {
            if (carta.gameObject.name != _cartaEscondidaCursor.gameObject.name)
            {
                _cartasEscondidas.Add(carta);
                List<ValorCasilla> nuevoCuadranteEsconder = RemoveCuadranteEnDict(carta.PosicionTablero);
                _cuadrantesEscondidos.Add(nuevoCuadranteEsconder);
                foreach (ValorCasilla valor in nuevoCuadranteEsconder)
                {
                    if (_dictCoordenadasFicha.TryGetValue(GeneraKey(valor.x, valor.y), out Ficha f))
                    {
                        _fichasEscondidas.Add(f);
                        f.gameObject.SetActive(false);
                        if (f.EsColor1)
                            _numFichasPuestasJ1--;
                        else
                            _numFichasPuestasJ2--;
                    }
                }
            }
        }
        carta.gameObject.SetActive(false);
    }

    public void DeshacerJugada()
    {
        _cartaEscondidaCursor.gameObject.SetActive(true);
        PutCuadranteEnDict(_cuadranteEscondidoCursor);
        if (_cartasEscondidas != null && _cuadrantesEscondidos != null)
        {
            for (int i = 0; i < _cartasEscondidas.Count && i < _cuadrantesEscondidos.Count; i++)
            {
                _cartasEscondidas[i].gameObject.SetActive(true);
                PutCuadranteEnDict(_cuadrantesEscondidos[i]);
            }
        }
        if (_fichasEscondidas != null && _fichasEscondidas.Count > 0)
        {
            foreach(Ficha f in _fichasEscondidas)
            {
                f.gameObject.SetActive(true);
                if (f.EsColor1)
                    _numFichasPuestasJ1++;
                else
                    _numFichasPuestasJ2++;
            }
        }
        _fichasEscondidas = null;
        _cartaEscondidaCursor = null;
        _cuadranteEscondidoCursor = null;
    }
    public void JugadaEliminar()
    {
        if (_cartaEscondidaCursor != null)
        {
            _dictCoordenadasCarta.Remove(GeneraKey(_cartaEscondidaCursor.PosicionTablero.x, _cartaEscondidaCursor.PosicionTablero.y));
            Destroy(_cartaEscondidaCursor.gameObject);
            if (_cartasEscondidas != null)
            {
                for (int i = 0; i < _cartasEscondidas.Count; i++)
                {
                    RemoveCuadranteEnDict(_cartasEscondidas[i].PosicionTablero);
                    _dictCoordenadasCarta.Remove(GeneraKey(_cartasEscondidas[i].PosicionTablero.x, _cartasEscondidas[i].PosicionTablero.y));
                    //Destroy(_cartasEscondidas[i].gameObject);
                }
            }
        }
        if (_fichasEscondidas != null && _fichasEscondidas.Count> 0)
        {
            foreach(Ficha f in _fichasEscondidas)
            {
                _dictCoordenadasFicha.Remove(GeneraKey(f.Cuadrante[2].x, f.Cuadrante[2].y));
                Destroy(f.gameObject);
            }
        }
        _fichasEscondidas = null;
        _cartaEscondidaCursor = null;
        _cuadranteEscondidoCursor = null;
    }
    private HashSet<int> fetchNodos(int nodoActual, HashSet<int> retorno, Dictionary<int, Carta> dictCartasPorOrden)
    {
        if (!retorno.Contains(nodoActual) && dictCartasPorOrden.TryGetValue(nodoActual, out Carta cartaActual))
        {
            retorno.Add(nodoActual);
            foreach (int indiceVecino in cartaActual.CartasVecinas)
            {
                if (!retorno.Contains(indiceVecino))
                {
                    foreach (int nodosRecursivos in fetchNodos(indiceVecino, retorno, dictCartasPorOrden))
                    {
                        retorno.Add(nodosRecursivos);
                    }
                }
            }
        }
        return retorno;
    }

    private List<ValorCasilla> GeneraCartaVirtual(Vector3 posicion, Carta carta)
    {
        //Representa el espacio que ocupa la carta
        List<ValorCasilla> cuadrante = GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y);

        //Si estamos en modo virtual, solo intentamos registrar cartas en posiciones vacias. En caso de estar intantando en casillas ocupadas abortamos
        foreach (ValorCasilla valorCasilla in cuadrante)
        {
            if (!valorCasilla.esVacia())
            {
                return null;
            }
        }

        List<ValorCasilla> retornoCartaVirtual = new List<ValorCasilla>();
        //Setea valores de casillas
        for (int i = 0; i < cuadrante.Count; i++)
        {
            //Generar carta virtual
            ValorCasilla casillaRetornoVirtual = new ValorCasilla();
            casillaRetornoVirtual.esColor1 = carta.ValorCuartosCarta[i] == '1';
            casillaRetornoVirtual.esColor2 = carta.ValorCuartosCarta[i] == '2';
            casillaRetornoVirtual.esOcupado = true;
            casillaRetornoVirtual.esTablero = cuadrante[i].esTablero;
            casillaRetornoVirtual.x = cuadrante[i].x;
            casillaRetornoVirtual.y = cuadrante[i].y;
            retornoCartaVirtual.Add(casillaRetornoVirtual);
        }

        return retornoCartaVirtual;
    }
    private void RegistraCartaEnPosicion(Vector3 posicion, Carta carta, int cartasRestantes, string cuartosProximaCarta)
    {
        RegistraCartaEnPosicion(posicion, carta);
    }

    private void RegistraCartaEnPosicion(Vector3 posicion, Carta carta)
    {
        //Representa el espacio que ocupa la carta
        List<ValorCasilla> cuadrante = GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y);
        if (_esFase2)
        {
            _dictCoordenadasCarta.Remove(GeneraKey(carta.PosicionTablero.x, carta.PosicionTablero.y));
        }
        carta.PosicionTablero = posicion;
        //Setea dict de Cartas
        _dictCoordenadasCarta.Add(GeneraKey(posicion.x, posicion.y), carta);
        //Setea valores de casillas
        for (int i = 0; i < cuadrante.Count; i++)
        {
            //Registrar carta
            ValorCasilla casilla = cuadrante[i];
            casilla.esColor1 = carta.ValorCuartosCarta[i] == '1';
            casilla.esColor2 = carta.ValorCuartosCarta[i] == '2';
            PropiedadCasilla nuevoValorColor1 = new PropiedadCasilla(new CoordenadaCasilla(casilla.x, casilla.y), CasillaPropiedadBool.esColor1, casilla.esColor1);
            PropiedadCasilla nuevoValorColor2 = new PropiedadCasilla(new CoordenadaCasilla(casilla.x, casilla.y), CasillaPropiedadBool.esColor2, casilla.esColor2);
            PropiedadCasilla nuevoValorCasillaOcupada = new PropiedadCasilla(new CoordenadaCasilla(casilla.x, casilla.y), CasillaPropiedadBool.esOcupada, true);

            PutValorEnDict(nuevoValorColor1);
            PutValorEnDict(nuevoValorColor2);
            PutValorEnDict(nuevoValorCasillaOcupada);
        }
        if (_esFase2)
        {
            ponFichaFase2(posicion);
        }
        
        //checkPuntoEnPosicion(posicion);
    }
    public void seleccionaNodo(Vector3 posicion)
    {
        for (int offsetX = 0; offsetX < 2; offsetX++)
        {
            for (int offsetY = 0; offsetY < 2; offsetY++)
            {
                if (_dictCoordenadasCarta.TryGetValue(GeneraKey(posicion.x + offsetX, posicion.y + offsetY), out Carta c))
                {
                    //Encontrada carta que pertenece al grupo
                    if (CartasSeleccionaNodo[0].Contains(c))
                    {
                        Debug.Log("SE HA SELECCIONADO EL 0");
                        nodoSeleccionado(0);
                    }
                    else if (CartasSeleccionaNodo[1].Contains(c))
                    {
                        Debug.Log("SE HA SELECCIONADO EL 1");
                        nodoSeleccionado(1);
                    }
                }
            }
        }

    }
    private void nodoSeleccionado(int index)
    {
        int noIndex = index == 0 ? 1 : 0;
        foreach (Carta cartaNodo in CartasSeleccionaNodo[index])
        {
            cartaNodo.muestraGrupoCarta();
            escondeCarta(cartaNodo, false);
        }
        foreach (Carta cartaNodo in CartasSeleccionaNodo[noIndex])
        {
            cartaNodo.muestraGrupoCarta();
        }
        _esJugadaSeleccionaNodo = false;
        SceneControllerManager.Instance.ToggleAcciones();
    }
    private void toggleGrupoCartas(int index)
    {
        int noIndex = index == 0 ? 1 : 0;
        foreach (Carta cartaGrupo in CartasSeleccionaNodo[index])
        {
            cartaGrupo.muestraCartaEliminar();
        }
        foreach (Carta cartaGrupo in CartasSeleccionaNodo[noIndex])
        {
            cartaGrupo.muestraGrupoCarta();
        }
    }
    /// <summary>
    /// Busca si la posicion del cursor pasa por enciuma de un grupo de cartas
    /// </summary>
    /// <param name="posicion"></param>
    /// <returns></returns>
    public bool checkGrupoEnPosicion(Vector3 posicion)
    {
        if (_dictValoresCasilla.TryGetValue(GeneraKey(posicion.x, posicion.y), out ValorCasilla casilla))
        {
            if (casilla.esOcupado)
            {
                //Jugador con cursor sobre grupo de cartas
                for (int offsetX = 0; offsetX < 2; offsetX++)
                {
                    for (int offsetY = 0; offsetY < 2; offsetY++)
                    {
                        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(posicion.x+offsetX, posicion.y + offsetY), out Carta c)) {
                            //Encontrada carta que pertenece al grupo
                            if (CartasSeleccionaNodo[0].Contains(c))
                            {
                                toggleGrupoCartas(0);
                            }
                            else if (CartasSeleccionaNodo[1].Contains(c))
                            {
                                toggleGrupoCartas(1);
                            }
                            return true;
                        }
                    }
                }
            }
        }
        foreach (List<Carta> grupo in CartasSeleccionaNodo)
        {
            foreach (Carta c in grupo)
            {
                c.muestraGrupoCarta();
            }
        }
        return false;
    }
    /// <summary>
    /// Checkea si se produce punto en la posicion. Si registraPunto es true solo mira los valores del dict. Si es false hay que pasarle una carta para que haga la comprobacion
    /// </summary>
    /// <param name="esRegistraPunto"></param>
    /// <param name="posicion"></param>
    /// <param name="esCheckColor1"></param>
    /// <returns></returns>
    public bool checkPuntoEnPosicion(bool esRegistraPunto, Vector3 posicion, bool esCheckColor1, Carta cartaToCheck, bool esCheckAdyacentes)
    {
        List<ValorCasilla> cartaVirtual = null;
        if (!esRegistraPunto && cartaToCheck != null)
        {
            if (cartaToCheck.PosicionTablero.x == posicion.x && cartaToCheck.PosicionTablero.y == posicion.y)
            {
                //Caso de que carta flotante pase por coordenada inicial
                return false;
            }
            foreach ( ValorCasilla casilla in GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y))
            {
                //Caso de carta flotante pasando por encima de casillas ocupadas
                if (casilla.esOcupado || !casilla.esTablero)
                {
                    return false;
                }
            }
            //if (_dictValoresCasilla.TryGetValue(GeneraKey(posicion.x, posicion.y), out ValorCasilla v))
            //{
            //    if (v.esOcupado || !v.esTablero)
            //    {
            //        //Caso de carta flotante pasando por encima de casillas ocupadas
            //        return false;
            //    }
            //}
            //Tomamos carta virtual
            cartaVirtual = GeneraCartaVirtual(posicion, cartaToCheck);
        }

        bool esAlgunPuntoEnPosicion = false;
        List<List<ValorCasilla>> cuadrantesAdyacentes = new List<List<ValorCasilla>>();
        if (esCheckAdyacentes)
        {
            //Cuadrantes adyacentes teniendo en cuenta la carta virtual si se ha pedido
            cuadrantesAdyacentes = GetCuadrantesAdyacentes((int)posicion.x, (int)posicion.y, cartaVirtual);
        }
        else
        {
            //El cuadrante de la posicion
            cuadrantesAdyacentes.Add(GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y));
        }

        foreach (List<ValorCasilla> cuadranteAdyacente in cuadrantesAdyacentes)
        {
            bool esPuntoColor1 = true;
            bool esPuntoColor2 = true;
            foreach (ValorCasilla casillaCuadrante in cuadranteAdyacente)
            {
                if (!casillaCuadrante.esOcupado)
                {
                    esPuntoColor1 = false;
                    esPuntoColor2 = false;
                }
                if (casillaCuadrante.esColor1)
                {
                    esPuntoColor2 = false;
                }
                if (casillaCuadrante.esColor2)
                {
                    esPuntoColor1 = false;
                }
            }

            if (esPuntoColor1 || esPuntoColor2)
            {
                if ((esCheckColor1 && esPuntoColor1) || (!esCheckColor1 && esPuntoColor2))
                {
                    esAlgunPuntoEnPosicion = true;
                }

                if (esRegistraPunto)
                {
                    EventHandler.CallPuntoEnCuadranteEvent(cuadranteAdyacente, esPuntoColor1);
                }
            }
        }
        return esAlgunPuntoEnPosicion;
    }

    public void ponFichaFase2(Vector3 posicion)
    {
        List<List<ValorCasilla>> cuadrantesAdyacentes =  GetCuadrantesAdyacentes((int)posicion.x, (int)posicion.y);

        foreach (List<ValorCasilla> cuadranteAdyacente in cuadrantesAdyacentes)
        {
            bool esPuntoColor1 = true;
            bool esPuntoColor2 = true;
            foreach (ValorCasilla casillaCuadrante in cuadranteAdyacente)
            {
                if (!casillaCuadrante.esOcupado)
                {
                    esPuntoColor1 = false;
                    esPuntoColor2 = false;
                }
                if (casillaCuadrante.esColor1)
                {
                    esPuntoColor2 = false;
                }
                if (casillaCuadrante.esColor2)
                {
                    esPuntoColor1 = false;
                }
            }
            if (esPuntoColor1)
            {
                EventHandler.CallPuntoEnCuadranteEvent(cuadranteAdyacente, true);
            }
            else if (esPuntoColor2)
            {
                EventHandler.CallPuntoEnCuadranteEvent(cuadranteAdyacente, false);
            }
        }
    }

    private void Start()
    {
        //PROBANDO a ver si va bien
        ISaveableRegister();
    }

    public void InicializaDictValoresCasilla()
    {
        _dictCoordenadasCarta = new Dictionary<string, Carta>();
        _dictCoordenadasFicha = new Dictionary<string, Ficha>();
        _numFichasPuestasJ1 = 0;
        _numFichasPuestasJ2 = 0;
        if (propiedadesCasillaArray != null && propiedadesCasillaArray.Length > 0)
        {
            _dictValoresCasilla = new Dictionary<string, ValorCasilla>();
            for (int i = 0; i < propiedadesCasillaArray.Length; i++)
            {
                //Elemento SO de una capa de propiedades
                foreach (PropiedadCasilla casilla in propiedadesCasillaArray[i].propiedadCasillaList)
                {
                    PutValorEnDict(casilla);
                }
            }
            _esDictCargado = true;
        }
    }
    public string GeneraKey(float x, float y)
    {
        return GeneraKey((int)x, (int)y);
    }
    public string GeneraKey(int x, int y)
    {
        return "x:" + x + "y:" + y;
    }

    public void PutCuadranteEnDict(List<ValorCasilla> cuadrante)
    {
        foreach (ValorCasilla casilla in cuadrante)
        {
            string key = GeneraKey(casilla.x, casilla.y);
            if (_dictValoresCasilla.TryGetValue(key, out ValorCasilla c))
            {
                c.esColor1 = casilla.esColor1;
                c.esColor2 = casilla.esColor2;
                c.esOcupado = casilla.esOcupado;
                c.esTablero = casilla.esTablero;
                _dictValoresCasilla.Remove(key);
            }
            else
            {
                c = new ValorCasilla(casilla);
            }
            _dictValoresCasilla.Add(key, c);
        }
    }
    public List<ValorCasilla> RemoveCuadranteEnDict(Vector3 posicion)
    {
        List<ValorCasilla> cuadrante = GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y);
        List<ValorCasilla> retorno = new List<ValorCasilla>();
        for (int i = 0; i < cuadrante.Count; i++)
        {
            retorno.Add(new ValorCasilla(cuadrante[i]));
            RemoveValorEnDict(cuadrante[i].x, cuadrante[i].y);
        }
        return retorno;
    }
    private void RemoveValorEnDict(int x, int y)
    {
        ValorCasilla nuevoValor;
        string key = GeneraKey(x, y);
        if (_dictValoresCasilla.TryGetValue(key, out ValorCasilla value))
        {
            nuevoValor = value;
            _dictValoresCasilla.Remove(key);
        }
        else
        {
            nuevoValor = new ValorCasilla();
        }
        nuevoValor.x = x;
        nuevoValor.y = y;
        nuevoValor.esOcupado = false;
        nuevoValor.esColor1 = false;
        nuevoValor.esColor2 = false;
        _dictValoresCasilla.Add(key, nuevoValor);

    }
    private void PutValorEnDict(PropiedadCasilla casilla)
    {
        ValorCasilla nuevoValor;
        string key = GeneraKey(casilla.coordenada.x, casilla.coordenada.y);
        if (_dictValoresCasilla.TryGetValue(key, out ValorCasilla value))
        {
            nuevoValor = value;
            _dictValoresCasilla.Remove(key);
        }
        else
        {
            nuevoValor = new ValorCasilla();
        }
        nuevoValor.x = casilla.coordenada.x;
        nuevoValor.y = casilla.coordenada.y;
        switch (casilla.propiedad)
        {
            case CasillaPropiedadBool.esTablero:
                nuevoValor.esTablero = casilla.valor;
                break;
            case CasillaPropiedadBool.esOcupada:
                nuevoValor.esOcupado = casilla.valor;
                break;
            case CasillaPropiedadBool.esColor1:
                nuevoValor.esColor1 = casilla.valor;
                break;
            case CasillaPropiedadBool.esColor2:
                nuevoValor.esColor2 = casilla.valor;
                break;
            default:
                break;
        }
        _dictValoresCasilla.Add(key, nuevoValor);

    }

    public bool EsAlgunOcupadoEnCuadrantesOrtoAdyacente(int x, int y)
    {
        bool retorno = false;
        List<List<ValorCasilla>> cuadrantes = GetCuadrantesAdyacentes(x, y);
        for (int i = 0; i < cuadrantes.Count; i++)
        {
            if (i != 0 && i != 2 && i != 6 && i != 8)
            {
                List<ValorCasilla> cuadrante = cuadrantes[i];
                foreach (ValorCasilla casilla in cuadrante)
                {
                    if (casilla.esOcupado)
                    {
                        retorno = true;
                    }
                }
            }
        }
        return retorno;
    }

    public List<List<ValorCasilla>> GetCuadrantesAdyacentes(List<ValorCasilla> cuadrante)
    {
        return GetCuadrantesAdyacentes(cuadrante[2].x, cuadrante[2].y);
    }
    public List<List<ValorCasilla>> GetCuadrantesAdyacentes(int x, int y)
    {
        return GetCuadrantesAdyacentes(x, y, null);
    }
    /// <summary>
    /// Devuelve los cuadrantes adyacentes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<List<ValorCasilla>> GetCuadrantesAdyacentes(int x, int y, List<ValorCasilla> cartaVirtual)
    {
        List<List<ValorCasilla>> retorno = new List<List<ValorCasilla>>();
        for (int offsetX = -1; offsetX < 2; offsetX++)
        {
            for (int offsetY = 1; offsetY > -2; offsetY--)
            {
                List<ValorCasilla> cuadrante = GetCuadranteEnCoordenada(x + offsetX, y + offsetY, cartaVirtual);
                retorno.Add(cuadrante);
            }
        }
        return retorno;
    }

    /// <summary>
    /// Devuelve la casilla correspondiente y las de arriba y derecha
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<ValorCasilla> GetCuadranteEnCoordenada(int x, int y)
    {
        return GetCuadranteEnCoordenada(x, y, null);
    }
    public List<ValorCasilla> GetCuadranteEnCoordenada(int x, int y, List<ValorCasilla> cartaVirtual)
    {
        List<ValorCasilla> valorCasillas = new List<ValorCasilla>();
        for (int offsetY = 1; offsetY >= 0; offsetY--)
        {
            for (int offsetX = 0; offsetX < 2; offsetX++)
            {
                if (_dictValoresCasilla.TryGetValue(GeneraKey(x + offsetX, y + offsetY), out ValorCasilla v))
                {
                    if (cartaVirtual != null && cartaVirtual.Count > 0)
                    {
                        foreach (ValorCasilla casillaVirtual in cartaVirtual)
                        {
                            if (casillaVirtual.x == x + offsetX && casillaVirtual.y == y + offsetY)
                            {
                                //Casilla en carta virtual
                                v = new ValorCasilla(casillaVirtual);
                            }
                        }
                    }
                }
                else
                {
                    v = new ValorCasilla();
                    v.x = x + offsetX;
                    v.y = y + offsetY;
                }
                valorCasillas.Add(v);
            }
        }
        return valorCasillas;
    }
    public ValorCasilla GetValorEnCoordenada(int x, int y)
    {
        if (_dictValoresCasilla.TryGetValue(GeneraKey(x, y), out ValorCasilla v))
        {
            return v;
        }
        else
        {
            return null;
        }
    }
    public void registraFichaEnTablero(ValorCasilla valorCasilla, Ficha pieza)
    {
        _dictCoordenadasFicha.Add(GeneraKey(valorCasilla.x, valorCasilla.y), pieza);
        if (pieza.EsColor1)
            _numFichasPuestasJ1++;
        else
            _numFichasPuestasJ2++;

        List<Carta> cartasAdyacentes = cartasAdyacentesACoordenada(valorCasilla);
        foreach (Carta c in cartasAdyacentes)
        {
            //se incrementa el valor de piezas de las cartas que esta tocando
            c.NumCuartosConFicha++;
        }
    }

    public void EliminaPiezaEnTablero(Ficha pieza)
    {

        Ficha piezaVisual;
        _dictCoordenadasFicha.TryGetValue(GeneraKey(pieza.Cuadrante[2].x, pieza.Cuadrante[2].y), out piezaVisual);
        List<Carta> cartasAdyacentes = cartasAdyacentesACoordenada(pieza.Cuadrante[2]);
        foreach (Carta c in cartasAdyacentes)
        {
            //se disminuye el valor de piezas de las cartas que esta tocando
            c.NumCuartosConFicha--;
        }
        Destroy(piezaVisual.gameObject);
        _dictCoordenadasFicha.Remove(GeneraKey(pieza.Cuadrante[2].x, pieza.Cuadrante[2].y));
        if (pieza.EsColor1)
            _numFichasPuestasJ1--;
        else
            _numFichasPuestasJ2--;
    }
    public List<Carta> cartasEnCuadrantesOrtogonalesACoordenada(int x, int y)
    {
        List<Carta> retorno = new List<Carta>();
        for (int offsetX = -1; offsetX < 2; offsetX++)
        {
            for (int offsetY = -1; offsetY < 2; offsetY++)
            {
                retorno.AddRange(cartasOrtogonalesACoordenada(x + offsetX, y + offsetY));
            }
        }
        return retorno;
    }
    private List<Carta> cartasAdyacentesACoordenada(ValorCasilla valorCasilla)
    {
        return cartasAdyacentesACoordenada(valorCasilla.x, valorCasilla.y);
    }
    private List<Carta> cartasAdyacentesACoordenada(int x, int y)
    {
        List<Carta> retorno = new List<Carta>();
        for (int offsetX = -1; offsetX < 2; offsetX++)
        {
            for (int offsetY = -1; offsetY < 2; offsetY++)
            {
                if (_dictCoordenadasCarta.TryGetValue(GeneraKey(x + offsetX, y + offsetY), out Carta c))
                {
                    retorno.Add(c);
                }
            }
        }
        return retorno;
    }

    private List<Carta> cartasOrtogonalesACoordenada(int x, int y)
    {
        List<Carta> retorno = new List<Carta>();
        Carta c;
        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(x - 1, y), out c))
        {
            retorno.Add(c);
        }
        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(x + 1, y), out c))
        {
            retorno.Add(c);
        }
        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(x, y + 1), out c))
        {
            retorno.Add(c);
        }
        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(x, y - 1), out c))
        {
            retorno.Add(c);
        }

        return retorno;
    }

    public Carta getCartaEnPosicion(Vector3 posicion)
    {
        Carta retorno = null;
        _dictCoordenadasCarta.TryGetValue(GeneraKey(posicion.x, posicion.y), out retorno);
        return retorno;
    }

    public Ficha getPiezaEnPosicion(Vector3 posicion)
    {
        Ficha retorno = null;
        _dictCoordenadasFicha.TryGetValue(GeneraKey(posicion.x, posicion.y), out retorno);
        return retorno;
    }
    public bool CheckPositionValidityFase1(Vector3Int cursorGridPosition, bool esCursorPieza, bool esTurnoColor1)
    {
        bool retorno = false;
        if (esCursorPieza)
        {
            if (!_dictCoordenadasFicha.TryGetValue(GeneraKey(cursorGridPosition.x, cursorGridPosition.y), out Ficha p))
            {
                retorno = checkPuntoEnPosicion(false, cursorGridPosition, esTurnoColor1, null, false);
            }
        }
        else
        {
            List<ValorCasilla> valoresCuadrante = GetCuadranteEnCoordenada(cursorGridPosition.x, cursorGridPosition.y);
            bool esNoOcupada = true;
            bool esDentroTablero = true;
            bool esAdyacenteAotra = EsAlgunOcupadoEnCuadrantesOrtoAdyacente(cursorGridPosition.x, cursorGridPosition.y);

            bool esValida = true;
            if (valoresCuadrante != null)
            {
                foreach (ValorCasilla valorCasilla in valoresCuadrante)
                {
                    if (valorCasilla.esOcupado)
                    {
                        esNoOcupada = false;
                    }
                    if (!valorCasilla.esTablero)
                    {
                        esDentroTablero = false;
                    }
                }
                esValida = esNoOcupada && esDentroTablero && esAdyacenteAotra;
                if (esValida)
                {
                    retorno = true;
                }
            }
        }
        return retorno;
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
        sceneSave.dictValoresCasilla = _dictValoresCasilla;
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("_esDictCargado", _esDictCargado);
        sceneSave.cuadranteEscondidoCursor = _cuadranteEscondidoCursor;
        sceneSave.cuadrantesEscondidos = _cuadrantesEscondidos;
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
                if (sceneSave.dictValoresCasilla != null)
                {
                    _dictValoresCasilla = sceneSave.dictValoresCasilla;
                }
                if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("_esDictCargado", out bool esDictCargado))
                {
                    _esDictCargado = esDictCargado;
                }
                if (sceneSave.cuadranteEscondidoCursor != null)
                {
                    _cuadranteEscondidoCursor = sceneSave.cuadranteEscondidoCursor;
                }
                if (sceneSave.cuadrantesEscondidos != null)
                {
                    _cuadrantesEscondidos = sceneSave.cuadrantesEscondidos;
                }
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