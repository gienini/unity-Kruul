using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursorFase2 : MonoBehaviour, ISaveable
{
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private Sprite greenCursorSpritePieza = null;
    [SerializeField] private Sprite redCursorSpritePieza = null;
    [SerializeField] private GameObject cartaBasePrefab = null;

    private GameObject _cartaGO;
    private Canvas _canvas;
    private Grid _grid;
    private Camera _mainCamera;
    private bool _cursorPositionIsValid = false;
    private bool _cursorPositionIsPieza = false;
    private bool _cursorIsEnabled = false;
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }
    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }
    public GameObject CartaGO { get => _cartaGO; set => _cartaGO = value; }
    public bool CursorPositionIsPieza { get => _cursorPositionIsPieza; set => _cursorPositionIsPieza = value; }

    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.AntesFadeOutEvent += AntesFadeOutEvent;
        EventHandler.JugadaHechaEvent += JugadaHechaEvent;
        ISaveableRegister();
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.AcabaFase1Event -= AcabaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.AntesFadeOutEvent -= AntesFadeOutEvent;
        EventHandler.JugadaHechaEvent -= JugadaHechaEvent;
        ISaveableDeregister();
    }

    private void JugadaHechaEvent()
    {
        if (CartaGO != null)
        {
            Destroy(CartaGO);
        }
    }

    private void AntesFadeOutEvent()
    {
        if (CartaGO != null)
        {
            CartaGO.SetActive(false);
        }
        cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 0f);
        _cursorIsEnabled = false;
    }

    private void DespuesFadeOutEvent()
    {
        if (CartaGO != null)
        {
            CartaGO.SetActive(true);
        }
        _cursorIsEnabled = true;
        cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 1f);
    }

    private void AcabaFase1Event()
    {
        _cursorIsEnabled = false;
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
        cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 0f);
    }

    private void Start()
    {
        if (CartaGO != null)
        {
            CartaGO.SetActive(false);
        }
    }

    private void Update()
    {
        if (_cursorIsEnabled && PropiedadesCasillasManager.Instance.EsDictCargado)
        {
            DisplayCursor();
        }
    }

    private void EmpiezaFase1Event()
    {
        EmpiezaFaseNuevaEvent();
    }

    private void EmpiezaFase2Event()
    {
        EmpiezaFaseNuevaEvent();
    }

    private void EmpiezaFaseNuevaEvent()
    {
        
    }

    private Vector3Int DisplayCursor()
    {
        if (_grid == null)
        {
            _grid = GameObject.FindObjectOfType<Grid>();
        }
        if (_grid != null)
        {
            Vector3Int cursorGridPosition = GetGridPositionForCursor();

            SetCursorValidity(cursorGridPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(cursorGridPosition);
            if (CartaGO != null)
            {
                CartaGO.gameObject.GetComponent<RectTransform>().position = GetRectTransformPositionForCursor(cursorGridPosition);
            }

            return cursorGridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        //Representa un punto en la escena completa
        Vector3 gridWorldPosition = _grid.CellToWorld(gridPosition);
        //Representa un punto dentro de la pantalla del jugador
        Vector2 gridScreenPosition = _mainCamera.WorldToScreenPoint(gridWorldPosition);

        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, _canvas);
    }

    public bool CheckPositionValidity(Vector3Int cursorGridPosition)
    {
        bool retorno = false;
        Ficha pieza = PropiedadesCasillasManager.Instance.getPiezaEnPosicion(GetGridPositionForCursor());
        //Tiene carta flotante
        if (CartaGO != null)
        {
            //Es ubicacion genera Punto para jugador actual y no es la ubicacion inicial de la carta
            if (PropiedadesCasillasManager.Instance.checkPuntoEnPosicion(false, GetGridPositionForCursor(), PropiedadesCasillasManager.Instance.EsTurnoColor1, CartaGO.GetComponent<Carta>(), true))
            {
                retorno = true;
            }
        }
        //Posicion pieza para retirar
        else if (pieza != null && ((PropiedadesCasillasManager.Instance.EsTurnoColor1 && pieza.EsColor1) || (!PropiedadesCasillasManager.Instance.EsTurnoColor1 && !pieza.EsColor1)))
        {
            retorno = true;
        }
        else
        {
            //Posicion carta para retirar
            Carta cartaEnPosicion = PropiedadesCasillasManager.Instance.getCartaEnPosicion(GetGridPositionForCursor());
            if (cartaEnPosicion != null && cartaEnPosicion.NumCuartosConFicha == 0)
            {
                retorno = true;
            }
        }
        return retorno;
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition)
    {
        Ficha pieza = PropiedadesCasillasManager.Instance.getPiezaEnPosicion(GetGridPositionForCursor());
        //Tiene carta flotante
        if (CartaGO != null)
        {
            //Es ubicacion genera Punto para jugador actual y no es la ubicacion inicial de la carta
            if (PropiedadesCasillasManager.Instance.checkPuntoEnPosicion(false, GetGridPositionForCursor(), PropiedadesCasillasManager.Instance.EsTurnoColor1, CartaGO.GetComponent<Carta>(), true))
            {
                SetCursorToValidCarta();
            }
            else
            {
                SetCursorToInvalid();
            }
        }
        //Posicion pieza para retirar. No se puede retirar piezas ajenas NI la ultima pieza de un jugador
        else if (pieza != null && PropiedadesCasillasManager.Instance.EsTurnoColor1 == pieza.EsColor1)
        {
            //No se puede retirar la ultima pieza propia
            if ((PropiedadesCasillasManager.Instance.EsTurnoColor1 ? PropiedadesCasillasManager.Instance.NumFichasPuestasJ1 : PropiedadesCasillasManager.Instance.NumFichasPuestasJ2) > 1)
            {
                SetCursorToValidPieza();
            }
        }else
        {
            //Posicion carta para retirar
            Carta cartaEnPosicion = PropiedadesCasillasManager.Instance.getCartaEnPosicion(GetGridPositionForCursor());
            if (cartaEnPosicion != null && cartaEnPosicion.NumCuartosConFicha == 0)
            {
                SetCursorToValidCarta();
            }else
            {
                SetCursorToInvalid();
            }
        }

    }
    private void SetCursorToValidCarta()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
        CursorPositionIsPieza = false;
    }
    private void SetCursorToValidPieza()
    {
        cursorImage.sprite = greenCursorSpritePieza;
        CursorPositionIsValid = true;
        CursorPositionIsPieza = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
        CursorPositionIsPieza = false;
    }

    /// <summary>
    /// Toma la posicion del cursor en la camera, lo transforma a WorldPosition y este lo devuelve como GridPosition (worldToCell) que es un Vector3Int
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetGridPositionForCursor()
    {
        //La Z la calculamos asi:
        //Camara esta a z = -10, de manera que los items estan enfrente (+10)
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z));
        return _grid.WorldToCell(worldPosition);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return new Vector3(_grid.CellToWorld(GetGridPositionForCursor()).x + 0.5f, _grid.CellToWorld(GetGridPositionForCursor()).y + 0.5f, 0f);
    }

    public void SetCartaFlotante(Carta carta)
    {
        if (CartaGO != null)
        {
            Destroy(CartaGO);
        }
        //GameObject carta
        CartaGO = Instantiate(cartaBasePrefab, _canvas.transform);
        CartaGO.GetComponent<Carta>().ValorCuartosCarta = carta.ValorCuartosCarta;
        CartaGO.GetComponent<Carta>().PosicionTablero = carta.PosicionTablero;
        CartaGO.GetComponent<Carta>().OrdenCarta = carta.OrdenCarta;
        CartaGO.transform.SetParent(gameObject.transform);
        CartaGO.transform.SetAsFirstSibling();
        SceneControllerManager.Instance.ToggleAcciones();
    }

    public void BotonEliminar()
    {
        Destroy(CartaGO);
        PropiedadesCasillasManager.Instance.JugadaEliminar();
        EventHandler.CallJugadaHechaEvent();
        SceneControllerManager.Instance.ToggleAcciones();

    }

    public void BotonDeshacer()
    {
        PropiedadesCasillasManager.Instance.DeshacerJugada();
        Destroy(CartaGO);
        SceneControllerManager.Instance.ToggleAcciones();
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
        sceneSave.boolDictionary.Add("cursorIsEnabled", _cursorIsEnabled);
        sceneSave.boolDictionary.Add("cursorPositionIsPieza", _cursorPositionIsPieza);
        if (_cartaGO != null)
        {
            sceneSave.cartaEscondidaCursor = new CartaSerializable(_cartaGO.GetComponent<Carta>());
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
                if (sceneSave.boolDictionary != null)
                {
                    if (sceneSave.boolDictionary.TryGetValue("cursorIsEnabled", out bool cursorIsEnabled))
                    {
                        _cursorIsEnabled = cursorIsEnabled;
                    }
                    if (sceneSave.boolDictionary.TryGetValue("cursorPositionIsPieza", out bool cursorPositionIsPieza))
                    {
                        _cursorPositionIsPieza = cursorPositionIsPieza;
                    }
                }
                if (sceneSave.cartaEscondidaCursor != null)
                {
                    _cartaGO = Instantiate(cartaBasePrefab, _canvas.transform);
                    _cartaGO.GetComponent<Carta>().ValorCuartosCarta = sceneSave.cartaEscondidaCursor.valorCuartosCarta;
                    _cartaGO.transform.SetParent(gameObject.transform);
                    _cartaGO.transform.SetAsFirstSibling();
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
}
