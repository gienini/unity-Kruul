using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursorFase1 : MonoBehaviour
{
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private GameObject cartaBasePrefab = null;

    private Vector2 _rectTransformPosFinalVolteaCartaColor1;
    private Vector2 _rectTransformPosFinalVolteaCartaColor2;
    private GameObject _cartaGO;
    private Canvas _canvas;
    private Grid _grid;
    private Camera _mainCamera;
    //private Carta _cartaBaseCursor;
    private bool _cursorPositionIsValid = false;
    private bool _cursorIsEnabled = false;
    private bool _esTurnoColor1 = true;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid && _cursorIsEnabled; set => _cursorPositionIsValid = value; }
    public GameObject CartaGO { get => _cartaGO; set => _cartaGO = value; }

    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += PopCartaEnPosicionEvent;
        EventHandler.AcabaFase1Event += AcabaFase1Event;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.AntesFadeOutEvent += AntesFadeOutEvent;
        EventHandler.DespuesIntroFase1Event += DespuesIntroFase1Event;
        EventHandler.DespuesVoltearCartaEvent += DespuesVoltearCartaEvent;
        EventHandler.JugadaHechaEvent += JugadaHechaEvent;
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
        EventHandler.DespuesVoltearCartaEvent -= DespuesVoltearCartaEvent;
        EventHandler.JugadaHechaEvent -= JugadaHechaEvent;
    }

    private void EmpiezaFase1Event()
    {
        
        CongelaYEsperaCarta();
        _grid = null;
    }

    private void JugadaHechaEvent(bool obj)
    {
        _esTurnoColor1 = !_esTurnoColor1;
    }

    private void DespuesVoltearCartaEvent()
    {
        _cursorIsEnabled = true;
        
    }

    public void CongelaYEsperaCarta()
    {
        _cursorIsEnabled = false;
        HideCursor();
    }

    private void AntesFadeOutEvent()
    {
        if (_cartaGO != null)
        {
            _cartaGO.SetActive(false);
        }
        _cursorIsEnabled = false;
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
        _cursorIsEnabled = false;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
        //_cartaBaseCursor = GetComponentInChildren<Carta>();
        if (_cartaGO != null)
        {
            _cartaGO.SetActive(false);
        }
        cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 0f);
    }

    private void Update()
    {
        if (_cursorIsEnabled && PropiedadesCasillasManager.Instance.EsDictCargado )
        {
            DisplayCursor();
        }else {
            HideCursor();
        }
    }

    private void DespuesIntroFase1Event()
    {
        _grid = GameObject.FindObjectOfType<Grid>();
        if (_rectTransformPosFinalVolteaCartaColor1 == null)
        {
            _rectTransformPosFinalVolteaCartaColor1 = GetRectTransformPositionForCursor(Settings.PosicionRobaCartaColor1Int);
        }
        if (_rectTransformPosFinalVolteaCartaColor2 == null)
        {
            _rectTransformPosFinalVolteaCartaColor2 = GetRectTransformPositionForCursor(Settings.PosicionRobaCartaColor2Int);
        }
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
        if (_grid != null && _cursorIsEnabled)
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
        return retorno;
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition)
    {
        if (CheckPositionValidity(cursorGridPosition))
        {
            SetCursorToValid();
        }else
        {
            SetCursorToInvalid();
        }
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        _cursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
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
}
