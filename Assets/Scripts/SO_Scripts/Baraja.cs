using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baraja
{
    private List<string> _pilaCartas;
    public void Barajar(List<string> so_barajaCartas)
    {
        _pilaCartas = new List<string>();
        foreach (string valorCuartosCarta in so_barajaCartas)
        {
            _pilaCartas.Add(valorCuartosCarta);
        }
        _pilaCartas.Shuffle();
    }

    public string Pop()
    {
        string retorno = null;
        if (_pilaCartas != null &&_pilaCartas.Count > 0)
        {
            retorno = _pilaCartas[0];
            _pilaCartas.RemoveAt(0);
        }
        return retorno;
    }

    public string GetSiguiente()
    {
        string retorno = null;
        if (_pilaCartas != null && _pilaCartas.Count > 0)
        {
            retorno = _pilaCartas[0];
        }
        return retorno;
    }

    public int Count()
    {
        int retorno = 0;
        if (_pilaCartas != null && _pilaCartas.Count > 0)
        {
            retorno = _pilaCartas.Count;
        }
        return retorno;
    }
    public Baraja(List<string> so_barajaCartas)
    {
        Barajar(so_barajaCartas);
    }
}
