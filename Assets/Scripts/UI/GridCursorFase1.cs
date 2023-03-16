using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursorFase1 : MonoBehaviour, ISaveable
{
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private GameObject cartaBasePrefab = null;
    [SerializeField] private Sprite greenCursorSpritePieza = null;
    [SerializeField] private Sprite redCursorSpritePieza = null;

    
    private bool _cursorPositionIsValid = false;
    public bool cursorIsEnabled = false;
    private bool _esCursorPieza = false;

    private GameObject _cartaGO;
    private Canvas _canvas;
    private Grid _grid;
    private Camera _mainCamera;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }
    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    public bool CursorPositionIsValid { get => _cursorPositionIsValid && cursorIsEnabled; set => _cursorPositionIsValid = value; }
    public GameObject CartaGO { get => _cartaGO; set => _cartaGO = value; }
    public bool EsCursorPieza { get => _esCursorPieza; set => _esCursorPieza = value; }

    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += PopCartaEnPosicionEvent;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.AntesFadeOutEvent += AntesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event += DespuesIntroFase1Event;
        EventHandler.AccionSeleccionadaEvent += AccionSeleccionadaEvent;
    }
    private void OnDisable()
    {
        EventHandler.PopCartaEnPosicionEvent -= PopCartaEnPosicionEvent;
        EventHandler.AcabaFase1Event -= AcabaFase1Event;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.AntesFadeOutEvent -= AntesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event -= DespuesIntroFase1Event;
        EventHandler.AccionSeleccionadaEvent -= AccionSeleccionadaEvent;
    }

    private void AccionSeleccionadaEvent(bool esAccionCarta)
    {
        cursorIsEnabled = true;
        _esCursorPieza = !esAccionCarta;
        DisplayCursor();
    }

    private void EmpiezaFase1Event()
    {
        
        CongelaYEsperaCarta();
        _grid = null;
    }

    public void CongelaYEsperaCarta()
    {
        cursorIsEnabled = false;
        HideCursor();
    }

    private void AntesFadeOutEvent()
    {
        if (_cartaGO != null && _cartaGO.transform.GetChild(0).gameObject != null)
        {
            _cartaGO.SetActive(false);
        }
        cursorIsEnabled = false;
    }

    private void DespuesFadeOutEvent()
    {
        if (_cartaGO != null)
        {
            _cartaGO.SetActive(true);
        }
    }

    private void AcabaFase1Event()
    {
        cursorIsEnabled = false;
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
        cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 0f);
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }
    private void Start()
    {
        if (_cartaGO != null)
        {
            _cartaGO.SetActive(false);
        }
    }

    private void Update()
    {
        if (cursorIsEnabled && PropiedadesCasillasManager.Instance.EsDictCargado && !SceneControllerManager.Instance.isFading)
        {
            DisplayCursor();
        }else {
            HideCursor();
        }
    }

    public void DespuesIntroFase1Event()
    {
        _grid = GameObject.FindObjectOfType<Grid>();
        cursorIsEnabled = true;
    }

    private void EmpiezaFase2Event()
    {
        EmpiezaFaseNuevaEvent();
    }

    private void EmpiezaFaseNuevaEvent()
    {
        
    }
    private void HideCursor()
    {
        cursorRectTransform.gameObject.SetActive(false);
        if (_cartaGO != null)
        {
            _cartaGO.gameObject.SetActive(false);
        }
    }
    private Vector3Int DisplayCursor()
    {
        if(_grid != null && cursorIsEnabled && _esCursorPieza)
        {
            cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 1f);
            cursorRectTransform.gameObject.SetActive(true);
            _cartaGO.gameObject.SetActive(false);
            Vector3Int cursorGridPosition = GetGridPositionForCursor();

            SetCursorValidity(cursorGridPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(cursorGridPosition);
            return cursorGridPosition;
        }
        else if (_grid != null && cursorIsEnabled)
        {
            cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 1f);
            cursorRectTransform.gameObject.SetActive(true);
            _cartaGO.gameObject.SetActive(true);
            Vector3Int cursorGridPosition = GetGridPositionForCursor();

            SetCursorValidity(cursorGridPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(cursorGridPosition);
            _cartaGO.gameObject.GetComponent<RectTransform>().position = GetRectTransformPositionForCursor(cursorGridPosition);

            return cursorGridPosition;
        }
        else
        {
            HideCursor();
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
        if (_esCursorPieza)
        {
            if (!PropiedadesCasillasManager.Instance.DictCoordenadasFicha.TryGetValue(PropiedadesCasillasManager.Instance.GeneraKey(cursorGridPosition.x, cursorGridPosition.y), out Ficha p))
            {
                retorno = PropiedadesCasillasManager.Instance.checkPuntoEnPosicion(false, cursorGridPosition, PropiedadesCasillasManager.Instance.EsTurnoColor1, null, false);
            }
        }
        else
        {
            List<ValorCasilla> valoresCuadrante = PropiedadesCasillasManager.Instance.GetCuadranteEnCoordenada(cursorGridPosition.x, cursorGridPosition.y);
            bool esNoOcupada = true;
            bool esDentroTablero = true;
            bool esAdyacenteAotra = PropiedadesCasillasManager.Instance.EsAlgunOcupadoEnCuadrantesOrtoAdyacente(cursorGridPosition.x, cursorGridPosition.y);

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

    private void SetCursorValidity(Vector3Int cursorGridPosition)
    {
        if (PropiedadesCasillasManager.Instance.CheckPositionValidityFase1(cursorGridPosition, _esCursorPieza, PropiedadesCasillasManager.Instance.EsTurnoColor1))
        {
            SetCursorToValid();
        }else
        {
            SetCursorToInvalid();
        }
    }

    private void SetCursorToValid()
    {
        if (_esCursorPieza)
        {
            cursorImage.sprite = greenCursorSpritePieza;
        }
        else
        {
            cursorImage.sprite = greenCursorSprite;
        }
        _cursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        if (_esCursorPieza)
        {
            cursorImage.sprite = redCursorSpritePieza;
        } else
        {
            cursorImage.sprite = redCursorSprite;
        }
        _cursorPositionIsValid = false;
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

    private void PopCartaEnPosicionEvent(Vector3 posicion, Carta carta, int cartasRestantesBaraja, string cuartosProximaCarta)
    {
        if (_cartaGO != null)
        {
            Destroy(_cartaGO);
        }
        //GameObject carta
        _cartaGO = Instantiate(cartaBasePrefab, _canvas.transform);
        _cartaGO.GetComponent<Carta>().ValorCuartosCarta = cuartosProximaCarta;
        _cartaGO.transform.SetParent(gameObject.transform);
        _cartaGO.transform.SetAsFirstSibling();
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
        sceneSave.boolDictionary.Add("cursorIsEnabled", cursorIsEnabled);
        sceneSave.boolDictionary.Add("esCursorPieza", _esCursorPieza);
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
                _grid = GameObject.FindObjectOfType<Grid>();
                if (sceneSave.boolDictionary != null)
                {
                    if (sceneSave.boolDictionary.TryGetValue("cursorIsEnabled", out bool cursorIsEnabled))
                    {
                        this.cursorIsEnabled = cursorIsEnabled;
                    }
                    if (sceneSave.boolDictionary.TryGetValue("esCursorPieza", out bool esCursorPieza))
                    {
                        _esCursorPieza = esCursorPieza;
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
