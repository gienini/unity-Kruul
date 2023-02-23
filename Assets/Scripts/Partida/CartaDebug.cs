using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CartaDebug : MonoBehaviour
{
    private TextMeshProUGUI texto;
    private Carta carta;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (texto == null)
        {
            texto = GetComponentInChildren<TextMeshProUGUI>();
        }
        if (carta == null)
        {
            carta = GetComponent<Carta>();
        }

        if (texto != null && carta != null)
        {
            texto.text = carta.OrdenCarta.ToString();
        }
    }
}
