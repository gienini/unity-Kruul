using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartaCuartaParte : MonoBehaviour
{

    [SerializeField] private GameObject MiniColor1Child = null;
    [SerializeField] private GameObject MiniColor2Child = null;

    [SerializeField] private bool _esColor1 = true;
    [SerializeField] public CartaCuartaPartePosicion posicion = 0;
    
    [SerializeField] private bool esCursor = false;

    public bool esColor1 { get => _esColor1; set => _esColor1 = value; }

    private void Start()
    {
        //if (esCursor)
        //{
        //    _image = GetComponent<Image>();
        //} else
        //{
        //    _spriteRenderer = GetComponent<SpriteRenderer>();
        //}
        
        
    }
    private void setComponentsImage(GameObject miniColorChild)
    {
        foreach(Image image in miniColorChild.GetComponentsInChildren<Image>())
        {
            image.enabled = true;
            SpriteRenderer spriteRenderer = image.gameObject.GetComponent<SpriteRenderer>();
            image.sprite = spriteRenderer.sprite;
            image.color = spriteRenderer.color;
            spriteRenderer.enabled = false;
        }
    }
    private void Update()
    {
        if (_esColor1)
        {
            MiniColor1Child.SetActive(true);
            MiniColor2Child.SetActive(false);
            if (esCursor)
            {
                setComponentsImage(MiniColor1Child);
            }            
        }
        else
        {
            MiniColor2Child.SetActive(true);
            MiniColor1Child.SetActive(false);
            if (esCursor)
            {
                setComponentsImage(MiniColor2Child);
            }
        }
        //if (esCursor && _esRefresh)
        //{
        //    if (_image != null)
        //    {
        //        if (_esColor1 && _image.sprite == null)
        //        {
        //            _image.sprite = spriteColor1;
        //        }
        //        else if (!_esColor1 && _image.sprite == null)
        //        {
        //            _image.sprite = spriteColor2;
        //        }
        //    }
        //} else
        //{
        //    if (_spriteRenderer != null)
        //    {
        //        if (_esColor1 && (_spriteRenderer.sprite == null || _spriteRenderer.sprite == spriteColor2))
        //        {
        //            _spriteRenderer.sprite = spriteColor1;
        //        }
        //        else if (!_esColor1 && (_spriteRenderer.sprite == null || _spriteRenderer.sprite == spriteColor1))
        //        {
        //            _spriteRenderer.sprite = spriteColor2;
        //        }
        //    }
        //}
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
            _esColor1 = true;
        }else if (valor == '2')
        {
            _esColor1 = false;
        }
    }

}
