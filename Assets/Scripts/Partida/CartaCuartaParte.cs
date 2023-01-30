using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartaCuartaParte : MonoBehaviour
{
    private bool _esColor1 = true;
    [SerializeField] public CartaCuartaPartePosicion posicion = 0;
    [SerializeField] private Sprite spriteColor1 = null;
    [SerializeField] private Sprite spriteColor2 = null;
    private SpriteRenderer spriteRenderer;

    public bool esColor1 { get => _esColor1; set => _esColor1 = value; }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (spriteRenderer != null)
        {
            if (_esColor1 && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = spriteColor1;
            }
            else if (!_esColor1 && spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = spriteColor2;
            }
        }
    }

    public void InicializaCuarto(string valorCuartosCarta)
    {
        
        char valor = 'E';
        switch (posicion)
        {
            case CartaCuartaPartePosicion.ArribaIzquierda:
                valor = valorCuartosCarta[0];
                break;
            case CartaCuartaPartePosicion.ArribaDerecha:
                valor = valorCuartosCarta[1];
                break;
            case CartaCuartaPartePosicion.AbajoIzquierda:
                valor = valorCuartosCarta[2];
                break;
            case CartaCuartaPartePosicion.AbajoDerecha:
                valor = valorCuartosCarta[3];
                break;
        }
        if (valor == '1')
        {
            esColor1 = true;
        }else if (valor == '2')
        {
            esColor1 = false;
        }
        Debug.Log(" new CUARTO-POSICION:"+ (int)posicion);
        Debug.Log(" new CUARTO-VALOR:" + valor);
    }

}
