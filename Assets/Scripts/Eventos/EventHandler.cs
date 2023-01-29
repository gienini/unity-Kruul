using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{
    public static event Action<Vector3> ClickEnTableroEvent;

    public static void CallClickEnTableroEvent(Vector3 posicion)
    {
        if (ClickEnTableroEvent != null)
        {
            ClickEnTableroEvent(posicion);
        }
    }

    public static event Action<Vector3, Carta> PopCartaEnPosicionEvent;

    public static void CallPopCartaEnPosicion(Vector3 posicion, Carta carta)
    {
        if (PopCartaEnPosicionEvent != null)
        {
            PopCartaEnPosicionEvent(posicion, carta);
        }
    }

    public static event Action<List<ValorCasilla>, bool> PuntoEnCuadranteEvent;

    public static void CallPuntoEnCuadranteEvent(List<ValorCasilla> cuadrante, bool esColor1)
    {
        if (PuntoEnCuadranteEvent != null)
        {
            PuntoEnCuadranteEvent(cuadrante, esColor1);
        }
    }
}
