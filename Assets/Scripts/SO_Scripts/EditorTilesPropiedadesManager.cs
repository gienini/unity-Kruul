using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;
using System.Collections.Generic;

[ExecuteAlways]

public class EditorTilesPropiedadesManager : MonoBehaviour
{
#if UNITY_EDITOR

    private Tilemap tilemap;
    private Grid grid;
    [SerializeField] private SO_PropiedadesCasilla gridProperties = null;
    [SerializeField] private CasillaPropiedadBool gridBoolProperty = CasillaPropiedadBool.esTablero;

    private void OnEnable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridProperties != null)
            {
                gridProperties.propiedadCasillaList.Clear();
            }
        }
    }


    private void OnDisable()
    {
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null)
            {
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        //Recalcula los limites del mapa
        tilemap.CompressBounds();
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            gridProperties.propiedadCasillaList.Add(new PropiedadCasilla(new CoordenadaCasilla(x, y), gridBoolProperty, true));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (!Application.IsPlaying(gameObject))
        {
            Debug.Log("DESHABILITAR Capa Casillas>Propiedades");
        }


    }

#endif
}
