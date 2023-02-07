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
    public int OrdenCarta;
    public HashSet<int> CartasVecinas;

    public string ValorCuartosCarta { get => _valorCuartosCarta; set => setValorCuartosCarta(value); }
    public int NumCuartosConFicha { get => _numCuartosConFicha; set => _numCuartosConFicha = value; }

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
                _cuartasPartes[i].InicializaCuarto(ValorCuartosCarta);
            }
        }
    }
    private void OnEnable()
    {
    }
}
