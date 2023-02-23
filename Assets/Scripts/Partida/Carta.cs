using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour
{
    private CartaCuartaParte[] _cuartasPartes = null;
    [SerializeField] private int _numCuartosConFicha = 0;
    [SerializeField] private string _valorCuartosCarta = null;
    public Vector3 PosicionTablero;
    [SerializeField] private int _ordenCarta;

    [SerializeField] private List<int> _cartasVecinas;

    public string ValorCuartosCarta { get => _valorCuartosCarta; set => setValorCuartosCarta(value); }
    public int NumCuartosConFicha { get => _numCuartosConFicha; set => _numCuartosConFicha = value; }
    public int OrdenCarta { get => _ordenCarta; set => setOrdenCarta(value); }
    public List<int> CartasVecinas { get => _cartasVecinas; set => _cartasVecinas = value; }

    private void setOrdenCarta(int i)
    {
        Debug.Log("ORDEN CARTA SET =" + i);
        _ordenCarta = i;
    }
    public void SetValoresFromSerializable(CartaSerializable cartaSerializable)
    {
        _numCuartosConFicha = cartaSerializable.numCuartosConFicha;
        _valorCuartosCarta = cartaSerializable.valorCuartosCarta;
        PosicionTablero = new Vector3(cartaSerializable.PosicionTablero.x, cartaSerializable.PosicionTablero.y, cartaSerializable.PosicionTablero.z);
        OrdenCarta = cartaSerializable.OrdenCarta;
        CartasVecinas = cartaSerializable.CartasVecinas;
    }
    private void setValorCuartosCarta(string valor)
    {
        _valorCuartosCarta = valor;
    }

    //private CoordenadaCasilla _coordenadaCasilla;
    //public CoordenadaCasilla CoordenadaCasilla { get => _coordenadaCasilla; set => _coordenadaCasilla = value; }
    void Start()
    {   
        
    }
    private void Update()
    {
        if (_valorCuartosCarta != null && _valorCuartosCarta != "")
        {
            _cuartasPartes = GetComponentsInChildren<CartaCuartaParte>();
            for (int i = 0; i < _cuartasPartes.Length; i++)
            {
                _cuartasPartes[i].InicializaCuarto(_valorCuartosCarta);
            }
        }
    }
    private void OnEnable()
    {
    }
}
