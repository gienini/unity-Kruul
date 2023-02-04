using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropiedadesCasillasManager : SingletonMonobehaviour<PropiedadesCasillasManager>
{
    private Dictionary<string, ValorCasilla> _dictValoresCasilla;
    private Dictionary<string, Carta> _dictCoordenadasCarta;
    private Dictionary<string, Pieza> _dictCoordenadasPieza;
    private bool _esDictCargado = false;
    private Carta _cartaFlotante;
    private List<ValorCasilla> _cuadranteFlotante;
    private List<Carta> _cartasEscondidas;
    private List<List<ValorCasilla>> _cuadrantesEscondidos;
    [SerializeField] private SO_PropiedadesCasilla[] propiedadesCasillaArray = null;

    public bool EsDictCargado { get => _esDictCargado; set => _esDictCargado = value; }
    public Dictionary<string, ValorCasilla> DictValoresCasilla { get => _dictValoresCasilla; set => _dictValoresCasilla = value; }
    public Dictionary<string, Carta> DictCoordenadasCarta { get => _dictCoordenadasCarta; set => _dictCoordenadasCarta = value; }
    public Dictionary<string, Pieza> DictCoordenadasPieza { get => _dictCoordenadasPieza; set => _dictCoordenadasPieza = value; }

    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += RegistraCartaEnPosicion;
    }

    private void OnDisable()
    {
        EventHandler.PopCartaEnPosicionEvent -= RegistraCartaEnPosicion;
    }
    public void EscondeCartaDePosicion(Vector3 posicion)
    {
        if (DictCoordenadasCarta.TryGetValue(GeneraKey(posicion.x, posicion.y), out Carta carta))
        {
            if (carta.CartasVecinas.Count > 1)
            {
                _cartasEscondidas = new List<Carta>();
                _cuadrantesEscondidos = new List<List<ValorCasilla>>();

                Dictionary<int, Carta> dictCartasPorOrden = new Dictionary<int, Carta>();
                foreach (Carta cartaDict in DictCoordenadasCarta.Values)
                {
                    dictCartasPorOrden.Add(cartaDict.OrdenCarta, cartaDict);
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
                        }else
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
                        setArboles.Add(arbolNodos);

                    }
                    //no son todos iguales
                    if (setArboles.Count > 1)
                    {
                        //jugador seleccione nodo
                        Debug.Log("JUGADOR SELECCIONA NODO");
                    }
                }
                else
                {
                    //Solo un arbol de nodos para mantener OR Todos los arboles son iguales THEN nada
                }

                //Mas de un nodo para mantener
                //Set de sets, se compara size nodosMantener[1 con size resultado
                //Son iguales
                //NADA
                //Son diferentes = 
                //Se unifican los que sean iguales
                //Jugador selecciona uno para mantener, los otros se descartan
            }
            //Seteamos carta flotante
            escondeCarta(carta, true);
        }
    }
    private void escondeCarta(Carta carta, bool esCartaFlotante)
    {
        if (esCartaFlotante)
        {
            _cartaFlotante = carta;
            _cuadranteFlotante = RemoveCuadranteEnDict(carta.PosicionInicial);
        }else
        {
            _cartasEscondidas.Add(carta);
            _cuadrantesEscondidos.Add(RemoveCuadranteEnDict(carta.PosicionInicial));
        }
        carta.gameObject.SetActive(false);
    }

    public void DeshacerJugada()
    {
        _cartaFlotante.gameObject.SetActive(true);
        PutCuadranteEnDict(_cuadranteFlotante);
        if (_cartasEscondidas != null && _cuadrantesEscondidos != null)
        {
            for (int i = 0; i < _cartasEscondidas.Count && i < _cuadrantesEscondidos.Count; i++)
            {
                _cartasEscondidas[i].gameObject.SetActive(true);
                PutCuadranteEnDict(_cuadrantesEscondidos[i]);
            }
        }
    }

    public void EliminarCartaFlotante()
    {
        //Multiplicamos por 2 a X porque partimos de las coordenadas de la representacion visual
        DictCoordenadasCarta.Remove(GeneraKey(_cartaFlotante.gameObject.transform.position.x * 2 , _cartaFlotante.gameObject.transform.position.y));
        Destroy(_cartaFlotante);
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
    private void RegistraCartaEnPosicion(Vector3 posicion, Carta carta, int cartasRestantes, string cuartosProximaCarta)
    {
        RegistraCartaEnPosicion(posicion, carta);
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
    
    private void RegistraCartaEnPosicion(Vector3 posicion, Carta carta)
    {
        //Representa el espacio que ocupa la carta
        List<ValorCasilla> cuadrante = GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y);
        carta.PosicionInicial = posicion;
        //Setea dict de Cartas
        DictCoordenadasCarta.Add(GeneraKey(posicion.x, posicion.y), carta);
        //SEPUEDE carta.coordenadasCasilla = (int)posicion.x, (int)posicion.y
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

        checkPuntoEnPosicion(posicion);
    }
    private bool checkPuntoEnPosicion(Vector3 posicion)
    {
        return checkPuntoEnPosicion(true, posicion, true, null);
    }
    /// <summary>
    /// Checkea si se produce punto en la posicion. Si registraPunto es true solo mira los valores del dict. Si es false hay que pasarle una carta para que haga la comprobacion
    /// </summary>
    /// <param name="esRegistraPunto"></param>
    /// <param name="posicion"></param>
    /// <param name="esCheckColor1"></param>
    /// <returns></returns>
    public bool checkPuntoEnPosicion(bool esRegistraPunto, Vector3 posicion, bool esCheckColor1, Carta cartaToCheck)
    {
        List<ValorCasilla> cartaVirtual = null;
        if (!esRegistraPunto)
        {
            if (cartaToCheck.PosicionInicial.x == posicion.x && cartaToCheck.PosicionInicial.y == posicion.y)
            {
                //Caso de que carta flotante pase por coordenada inicial
                return false;
            }
            if (DictValoresCasilla.TryGetValue(GeneraKey(posicion.x, posicion.y), out ValorCasilla v))
            {
                if (v.esOcupado || !v.esTablero)
                {
                    //Caso de carta flotante pasando por encima de casillas ocupadas
                    return false;
                }
            }
            //Tomamos carta virtual
            cartaVirtual = GeneraCartaVirtual(posicion, cartaToCheck);
        }

        bool esAlgunPuntoEnPosicion = false;
        //Cuadrantes adyacentes teniendo en cuenta la carta virtual si se ha pedido
        List<List<ValorCasilla>> cuadrantesAdyacentes = GetCuadrantesAdyacentes((int)posicion.x, (int)posicion.y, cartaVirtual);
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

    private void Start()
    {
        
    }

    public void InicializaDictValoresCasilla()
    {
        _dictCoordenadasCarta = new Dictionary<string, Carta>();
        DictCoordenadasPieza = new Dictionary<string, Pieza>();
        if (propiedadesCasillaArray != null && propiedadesCasillaArray.Length > 0)
        {
            DictValoresCasilla = new Dictionary<string, ValorCasilla>();
            for (int i = 0; i < propiedadesCasillaArray.Length; i++)
            {
                //Elemento SO de una capa de propiedades
                foreach (PropiedadCasilla casilla in propiedadesCasillaArray[i].propiedadCasillaList)
                {
                    PutValorEnDict(casilla);
                }
            }
            EsDictCargado = true;
        }
    }
    private string GeneraKey(float x, float y)
    {
        return GeneraKey((int)x, (int)y);
    }
    private string GeneraKey(int x, int y)
    {
        return "x:" + x + "y:" +y;
    }

    public void PutCuadranteEnDict(List<ValorCasilla> cuadrante)
    {
        foreach(ValorCasilla casilla in cuadrante)
        {
            string key = GeneraKey(casilla.x, casilla.y);
            DictValoresCasilla.Remove(key);
            DictValoresCasilla.Add(key, casilla);
        }
    }
    public List<ValorCasilla> RemoveCuadranteEnDict(Vector3 posicion)
    {
        List<ValorCasilla> cuadrante = GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y);
        List<ValorCasilla> retorno = new List<ValorCasilla>();
        for (int i = 0; i < cuadrante.Count; i++)
        {
            retorno.Add(cuadrante[i]);
            RemoveValorEnDict(cuadrante[i].x, cuadrante[i].y);
        }
        return retorno;
    }
    private void RemoveValorEnDict(int x, int y)
    {
        ValorCasilla nuevoValor;
        string key = GeneraKey(x, y);
        if (DictValoresCasilla.TryGetValue(key, out ValorCasilla value))
        {
            nuevoValor = value;
            DictValoresCasilla.Remove(key);
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
        DictValoresCasilla.Add(key, nuevoValor);

    }
    private void PutValorEnDict(PropiedadCasilla casilla)
    {
        ValorCasilla nuevoValor;
        string key = GeneraKey(casilla.coordenada.x, casilla.coordenada.y);
        if (DictValoresCasilla.TryGetValue(key, out ValorCasilla value))
        {
            nuevoValor = value;
            DictValoresCasilla.Remove(key);
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
        DictValoresCasilla.Add(key, nuevoValor);

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
        for (int offsetY = 1; offsetY >=0; offsetY--)
        {
            for (int offsetX = 0; offsetX < 2; offsetX++)
            {
                if (DictValoresCasilla.TryGetValue(GeneraKey(x + offsetX, y + offsetY), out ValorCasilla v))
                {
                    if (cartaVirtual != null && cartaVirtual.Count > 0)
                    {
                        foreach (ValorCasilla casillaVirtual in cartaVirtual)
                        {
                            if (casillaVirtual.x == x + offsetX && casillaVirtual.y == y + offsetY)
                            {
                                //Casilla en carta virtual
                                v = casillaVirtual;
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
        if (DictValoresCasilla.TryGetValue(GeneraKey(x, y), out ValorCasilla v))
        {
            return v;
        }
        else
        {
            return null;
        }
    }
    public void registraPiezaEnTablero(ValorCasilla valorCasilla, Pieza pieza)
    {
        DictCoordenadasPieza.Add(GeneraKey(valorCasilla.x, valorCasilla.y), pieza);
        List<Carta> cartasAdyacentes = cartasAdyacentesACoordenada(valorCasilla);
        foreach(Carta c in cartasAdyacentes)
        {
            //se incrementa el valor de piezas de las cartas que esta tocando
            c.NumCuartosConFicha++;
        }
    }

    public void EliminaPiezaEnTablero(Pieza pieza)
    {

        Pieza piezaVisual;
        DictCoordenadasPieza.TryGetValue(GeneraKey(pieza.Cuadrante[2].x, pieza.Cuadrante[2].y), out piezaVisual);
        List<Carta> cartasAdyacentes = cartasAdyacentesACoordenada(pieza.Cuadrante[2]);
        foreach (Carta c in cartasAdyacentes)
        {
            //se disminuye el valor de piezas de las cartas que esta tocando
            c.NumCuartosConFicha--;
        }
        Destroy(piezaVisual.gameObject);
        DictCoordenadasPieza.Remove(GeneraKey(pieza.Cuadrante[2].x, pieza.Cuadrante[2].y));
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
        if (_dictCoordenadasCarta.TryGetValue(GeneraKey(x , y + 1), out c))
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

    public Pieza getPiezaEnPosicion(Vector3 posicion)
    {
        Pieza retorno = null;
        _dictCoordenadasPieza.TryGetValue(GeneraKey(posicion.x, posicion.y), out retorno);
        return retorno;
    }
}
