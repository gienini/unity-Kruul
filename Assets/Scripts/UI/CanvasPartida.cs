using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasPartida : MonoBehaviour
{

    private TextMeshProUGUI _jugadorActTexto;
    private TextMeshProUGUI _fichasJugadasJ1Texto;
    private TextMeshProUGUI _fichasJugadasJ2Texto;
    private TextMeshProUGUI _barajaCartasTexto;
    private TextMeshProUGUI _faseTexto;

    private int _cartasRestantesBaraja = 0;
    private int _fichasJugadasJugador1 = 0;
    private int _fichasJugadasJugador2 = 0;
    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += PopCartaEnPosicionEvent;
        EventHandler.PuntoEnCuadranteEvent += PuntoEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2;
    }
    private void OnDisable()
    {
        EventHandler.PopCartaEnPosicionEvent -= PopCartaEnPosicionEvent;
        EventHandler.PuntoEnCuadranteEvent -= PuntoEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2;
    }
    void Start()
    {
        List<TextMeshProUGUI> textos = new List<TextMeshProUGUI>(GetComponentsInChildren<TextMeshProUGUI>());
        foreach (TextMeshProUGUI reg in textos)
        {
            if (reg.gameObject.name == "JugadorActTexto")
            {
                _jugadorActTexto = reg;
            }
            else if (reg.gameObject.name == "FichasJugadasJ1Texto")
            {
                _fichasJugadasJ1Texto = reg;
            }
            else if (reg.gameObject.name == "FichasJugadasJ2Texto")
            {
                _fichasJugadasJ2Texto = reg;
            }
            else if (reg.gameObject.name == "BarajaCartasTexto")
            {
                _barajaCartasTexto = reg;
            }
            else if (reg.gameObject.name == "FaseTexto")
            {
                _faseTexto = reg;
            }
        }
    }
    void Update()
    {
        _barajaCartasTexto.text = "Cartas restantes baraja: #".Replace("#", _cartasRestantesBaraja.ToString());
        _fichasJugadasJ1Texto.text = "Fichas jugador1: #".Replace("#", _fichasJugadasJugador1.ToString());
        _fichasJugadasJ2Texto.text = "Fichas jugador2: #".Replace("#", _fichasJugadasJugador2.ToString());
    }

    private void PopCartaEnPosicionEvent(Vector3 arg1, Carta arg2, int cartasRestantesBaraja, string cuartosProximaCarta)
    {
        _cartasRestantesBaraja = cartasRestantesBaraja;
    }
    private void PuntoEvent(List<ValorCasilla> arg1, bool esPuntoJugador1)
    {
        if (esPuntoJugador1)
        {
            _fichasJugadasJugador1++;
        }else
        {
            _fichasJugadasJugador2++;
        }
    }

    private void EmpiezaFase2()
    {
        _faseTexto.text = "FASE 2";
    }

    private void EmpiezaFase1()
    {
        _cartasRestantesBaraja = 0;
        _fichasJugadasJugador1 = 0;
        _fichasJugadasJugador2 = 0;
        _faseTexto.text = "FASE 1";
    }
}
