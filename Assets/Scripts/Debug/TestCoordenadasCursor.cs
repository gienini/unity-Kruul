using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestCoordenadasCursor : MonoBehaviour
{
    [SerializeField] private GridCursorFase1 gridCursor = null;
    private TextMeshProUGUI textoCoordenadas;
    private TextMeshProUGUI textoCoordenadasUltimoClick;

    private void OnEnable()
    {
        EventHandler.ClickEnTableroFase1Event += UpdateTextoCoordenadasUltimoClick;
    }

    private void OnDisable()
    {
        EventHandler.ClickEnTableroFase1Event -= UpdateTextoCoordenadasUltimoClick;
    }

    private void UpdateTextoCoordenadasUltimoClick(Vector3 posicion)
    {
        textoCoordenadasUltimoClick.SetText("Coo Ultimo click = X: " + posicion.x + "Y: " + posicion.y);
    }

    private void Start()
    {
        List<TextMeshProUGUI> textos = new List<TextMeshProUGUI>(GetComponentsInChildren<TextMeshProUGUI>());
        foreach (TextMeshProUGUI reg in textos)
        {
            if (reg.gameObject.name == "Coordenadas")
            {
                textoCoordenadas = reg;
            }
            else if (reg.gameObject.name == "CooUltimoClick")
            {
                textoCoordenadasUltimoClick = reg;
            }
        }
        
    }
    private void Update()
    {
        if (gridCursor != null)
        {
            Vector3Int v3i = gridCursor.GetGridPositionForCursor();
            textoCoordenadas.SetText("Coo Actual = X: " + v3i.x + "Y: " + v3i.y);
        }
    }
}
