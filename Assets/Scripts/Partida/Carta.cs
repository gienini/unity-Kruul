using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour
{
    private CartaCuartaParte[] _cuartasPartes = null;
    private CoordenadaCasilla _coordenadaCasilla;
    [SerializeField] private Sprite spriteCartaCuartaParteColor1 = null;
    [SerializeField] private Sprite spriteCartaCuartaParteColor2 = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_cuartasPartes == null)
        {
            _cuartasPartes = GetComponentsInChildren<CartaCuartaParte>();
            for (int i = 0; i < _cuartasPartes.Length; i++)
            {
                CartaCuartaParte cartaCuartaParte = _cuartasPartes[i];
                if (cartaCuartaParte.esColor1)
                {
                    cartaCuartaParte.GetComponent<SpriteRenderer>().sprite = spriteCartaCuartaParteColor1;
                }
                else
                {
                    cartaCuartaParte.GetComponent<SpriteRenderer>().sprite = spriteCartaCuartaParteColor2;
                }

            }
        }
    }
}
