using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;
    [SerializeField] private GameObject cartaBasePrefab = null;

    private GameObject _cartaGO;
    private Canvas _canvas;
    private Grid _grid;
    private Camera _mainCamera;
    //private Carta _cartaBaseCursor;
    private bool _cursorPositionIsValid = false;
    private bool _cursorIsEnabled = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += PopCartaEnPosicionEvent;
    }
    private void OnDisable()
    {
        EventHandler.PopCartaEnPosicionEvent -= PopCartaEnPosicionEvent;
    }

    private void Start()
    {
        _grid = GameObject.FindObjectOfType<Grid>();
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
        //_cartaBaseCursor = GetComponentInChildren<Carta>();
    }

    private void Update()
    {
        if (PropiedadesCasillasManager.Instance.EsDictCargado)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (_grid != null)
        {
            Vector3Int cursorGridPosition = GetGridPositionForCursor();

            SetCursorValidity(cursorGridPosition);

            cursorRectTransform.position = GetRectTransformPositionForCursor(cursorGridPosition);
            _cartaGO.gameObject.GetComponent<RectTransform>().position = GetRectTransformPositionForCursor(cursorGridPosition);

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

    private void SetCursorValidity(Vector3Int cursorGridPosition)
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
                SetCursorToValid();
            }
            else
            {
                SetCursorToInvalid();
            }
        }

    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
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
