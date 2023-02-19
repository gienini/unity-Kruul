using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{
    public static event Action<Vector3, bool> ClickEnTableroFase1Event;

    public static void CallClickEnTableroFase1Event(Vector3 posicion, bool esAccionCarta)
    {
        if (ClickEnTableroFase1Event != null)
        {
            ClickEnTableroFase1Event(posicion, esAccionCarta);
        }
    }

    public static event Action<GridCursorFase2, bool> ClickEnTableroFase2Event;

    public static void CallClickEnTableroFase2Event(GridCursorFase2 cursor, bool esClickEnPieza)
    {
        if (ClickEnTableroFase2Event != null)
        {
            ClickEnTableroFase2Event(cursor, esClickEnPieza);
        }
    }

    public static event Action<Vector3, Carta, int, string> PopCartaEnPosicionEvent;

    public static void CallPopCartaEnPosicion(Vector3 posicion, Carta carta, int cartasRestantesBaraja, string cuartosProximaCarta)
    {
        if (PopCartaEnPosicionEvent != null)
        {
            PopCartaEnPosicionEvent(posicion, carta, cartasRestantesBaraja, cuartosProximaCarta);
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

    public static event Action EmpiezaFase1Event;
    public static void CallEmpiezaFase1Event()
    {
        if (EmpiezaFase1Event != null)
        {
            EmpiezaFase1Event();
        }
    }

    public static event Action EmpiezaFase2Event;
    public static void CallEmpiezaFase2Event()
    {
        if (EmpiezaFase2Event != null)
        {
            EmpiezaFase2Event();
        }
    }

    public static event Action AcabaFase1Event;
    public static void CallAcabaFase1Event()
    {
        if (AcabaFase1Event != null)
        {
            PropiedadesCasillasManager.Instance.EsTurnoColor1 = !PropiedadesCasillasManager.Instance.EsTurnoColor1;
            AcabaFase1Event();
        }
    }

    public static event Action AcabaFase2Event;
    public static void CallAcabaFase2Event()
    {
        if (AcabaFase2Event != null)
        {
            AcabaFase2Event();
        }
    }

    public static event Action JugadaHechaEvent;

    public static void CallJugadaHechaEvent()
    {
        if (JugadaHechaEvent != null)
        {
            PropiedadesCasillasManager.Instance.EsTurnoColor1 = !PropiedadesCasillasManager.Instance.EsTurnoColor1;
            JugadaHechaEvent();
        }
    }

    public static event Action AntesFadeOutEvent;
    public static void CallAntesFadeOutEvent()
    {
        if (AntesFadeOutEvent != null)
        {
            AntesFadeOutEvent();
        }
    }
    public static event Action FadeOutEvent;
    public static void CallFadeOutEvent()
    {
        if (FadeOutEvent != null)
        {
            FadeOutEvent();
        }
    }
    public static event Action DespuesFadeOutEvent;
    public static void CallDespuesFadeOutEvent()
    {
        if (DespuesFadeOutEvent != null)
        {
            DespuesFadeOutEvent();
        }
    }

    public static event Action MenuPrincipalEvent;
    public static void CallMenuPrincipalEvent()
    {
        if (MenuPrincipalEvent != null)
        {
            MenuPrincipalEvent();
        }
    }

    public static event Action DespuesIntroFase1Event;
    public static void CallDespuesIntroFase1Event()
    {
        if (DespuesIntroFase1Event != null)
        {
            DespuesIntroFase1Event();
        }
    }

    public static event Action<bool> AccionSeleccionadaEvent;
    public static void CallAccionSeleccionadaEvent(bool esCarta)
    {
        if (AccionSeleccionadaEvent != null)
        {
            AccionSeleccionadaEvent(esCarta);
        }
    }
}
