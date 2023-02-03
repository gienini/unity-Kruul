using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour
{
    private CartaCuartaParte[] _cuartasPartes = null;
    private bool _esRefresh = true;
    [SerializeField] private int _numCuartosConFicha = 0;
    [SerializeField] private string _valorCuartosCarta = null;
    public Vector3 PosicionInicial;
    public int OrdenCarta;
    public HashSet<int> CartasVecinas;

    public string ValorCuartosCarta { get => _valorCuartosCarta; set => setValorCuartosCarta(value); }
    public int NumCuartosConFicha { get => _numCuartosConFicha; set => _numCuartosConFicha = value; }

    private void setValorCuartosCarta(string valor)
    {
        _valorCuartosCarta = valor;
        _esRefresh = true;
    }

    //private CoordenadaCasilla _coordenadaCasilla;
    //public CoordenadaCasilla CoordenadaCasilla { get => _coordenadaCasilla; set => _coordenadaCasilla = value; }
    void Start()
    {   
        
    }
    private void Update()
    {
        if (_esRefresh && _valorCuartosCarta != null && _valorCuartosCarta != "")
        {
            _cuartasPartes = GetComponentsInChildren<CartaCuartaParte>();
            for (int i = 0; i < _cuartasPartes.Length; i++)
            {
                _cuartasPartes[i].InicializaCuarto(ValorCuartosCarta);
            }
            _esRefresh = false;
        }
    }
}
