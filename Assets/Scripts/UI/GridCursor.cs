using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage = null;
    [SerializeField] private RectTransform cursorRectTransform = null;
    [SerializeField] private Sprite greenCursorSprite = null;
    [SerializeField] private Sprite redCursorSprite = null;

    private bool _cursorPositionIsValid = false;
    private bool _cursorIsEnabled = true;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void Start()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            Vector3Int cursorGridPosition = GetGridPositionForCursor();

            SetCursorValidity(cursorGridPosition);

            //Seteamos el valor pero no lo usamos aun

            cursorRectTransform.position = GetRectTransformPositionForCursor(cursorGridPosition);

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
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        //Representa un punto dentro de la pantalla del jugador
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);

        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition)
    {
        List<ValorCasilla> valoresCuadrante = PropiedadesCasillasManager.Instance.GetCuadranteEnCoordenada(cursorGridPosition.x, cursorGridPosition.y);
        bool esValida = true;
        if (valoresCuadrante != null)
        {
            foreach (ValorCasilla valorCasilla in valoresCuadrante)
            {
                if (valorCasilla.esOcupado || !valorCasilla.esTablero)
                {
                    esValida = false;
                }
            }
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
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 GetWorldPositionForCursor()
    {
        return new Vector3(grid.CellToWorld(GetGridPositionForCursor()).x + 0.5f, grid.CellToWorld(GetGridPositionForCursor()).y + 0.5f, 0f);
    }
}
