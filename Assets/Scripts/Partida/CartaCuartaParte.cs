using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartaCuartaParte : MonoBehaviour
{
    private bool _esColor1 = true;
    [SerializeField] public CartaCuartaPartePosicion posicion = 0;
    [SerializeField] private Sprite spriteColor1 = null;
    [SerializeField] private Sprite spriteColor2 = null;
    [SerializeField] private bool esCursor = false;
    private SpriteRenderer _spriteRenderer;
    private Image _image;
    private bool _esRefresh = false;

    public bool esColor1 { get => _esColor1; set => _esColor1 = value; }

    private void Start()
    {
        if (esCursor)
        {
            _image = GetComponent<Image>();
        } else
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        
    }
    private void Update()
    {
        if (esCursor && _esRefresh)
        {
            if (_image != null)
            {
                if (_esColor1 && _image.sprite == null)
                {
                    _image.sprite = spriteColor1;
                }
                else if (!_esColor1 && _image.sprite == null)
                {
                    _image.sprite = spriteColor2;
                }
            }
        } else
        {
            if (_spriteRenderer != null)
            {
                if (_esColor1 && _spriteRenderer.sprite == null)
                {
                    _spriteRenderer.sprite = spriteColor1;
                }
                else if (!_esColor1 && _spriteRenderer.sprite == null)
                {
                    _spriteRenderer.sprite = spriteColor2;
                }
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
        _esRefresh = true;
    }

}
