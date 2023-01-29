using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropiedadesCasillasManager : SingletonMonobehaviour<PropiedadesCasillasManager>
{
    private Dictionary<string, ValorCasilla> dictValoresCasilla;
    [SerializeField] private SO_PropiedadesCasilla[] propiedadesCasillaArray = null;

    private void OnEnable()
    {
        EventHandler.PopCartaEnPosicionEvent += RegistraCartaEnPosicion;
    }
    private void OnDisable()
    {
        EventHandler.PopCartaEnPosicionEvent -= RegistraCartaEnPosicion;
    }
    private void RegistraCartaEnPosicion(Vector3 posicion, Carta carta)
    {
        List<ValorCasilla> cuadrantes = GetCuadranteEnCoordenada((int)posicion.x, (int)posicion.y);
        //Setea valores de casillas
        for(int i = 0; i < cuadrantes.Count; i++)
        {
            ValorCasilla casilla = cuadrantes[i];
            casilla.esColor1 = carta.GetComponentsInChildren<CartaCuartaParte>()[i].esColor1;
            casilla.esColor2 = !carta.GetComponentsInChildren<CartaCuartaParte>()[i].esColor1;
            PropiedadCasilla nuevoValorColor1 = new PropiedadCasilla(new CoordenadaCasilla(casilla.x, casilla.y), CasillaPropiedadBool.esColor1, casilla.esColor1);
            PropiedadCasilla nuevoValorColor2 = new PropiedadCasilla(new CoordenadaCasilla(casilla.x, casilla.y), CasillaPropiedadBool.esColor2, casilla.esColor2);
            PropiedadCasilla nuevoValorCasillaOcupada = new PropiedadCasilla(new CoordenadaCasilla(casilla.x, casilla.y), CasillaPropiedadBool.esOcupada, true);
            PutValorEnDict(nuevoValorColor1);
            PutValorEnDict(nuevoValorColor2);
            PutValorEnDict(nuevoValorCasillaOcupada);
        }
        //Check for puntos
        List<List<ValorCasilla>> cuadrantesAdyacentes = GetCuadrantesAdyacentes((int)posicion.x, (int)posicion.y);
        foreach (List<ValorCasilla> cuadrante in cuadrantesAdyacentes)
        {
            bool esPuntoColor1 = true;
            bool esPuntoColor2 = true;
            Debug.Log("----------------Cuadrante");
            foreach (ValorCasilla casillaCuadrante in cuadrante)
            {
                if (!casillaCuadrante.esOcupado)
                {
                    esPuntoColor1 = false;
                    esPuntoColor2 = false;
                }
                if (casillaCuadrante.esColor1)
                {
                    esPuntoColor2 = false;
                }
                if (casillaCuadrante.esColor2)
                {
                    esPuntoColor1 = false;
                }
                
                Debug.Log("escanea x:" + casillaCuadrante.x + " y:" + casillaCuadrante.y + "escolor1: " + casillaCuadrante.esColor1 + " escolor2" + casillaCuadrante.esColor2);
            }

            if (esPuntoColor1 || esPuntoColor2)
            {
                Debug.Log("PUNTO");
                EventHandler.CallPuntoEnCuadranteEvent(cuadrante, esPuntoColor1);
            }
        }
    }

    private void Start()
    {
        InicializaDictValoresCasilla();
    }

    private void InicializaDictValoresCasilla()
    {
        if (propiedadesCasillaArray != null && propiedadesCasillaArray.Length > 0)
        {
            dictValoresCasilla = new Dictionary<string, ValorCasilla>();
            for (int i = 0; i < propiedadesCasillaArray.Length; i++)
            {
                //Elemento SO de una capa de propiedades
                foreach (PropiedadCasilla casilla in propiedadesCasillaArray[i].propiedadCasillaList)
                {
                    PutValorEnDict(casilla);
                }
            }
        }
    }

    private string GeneraKey(int x, int y)
    {
        return "x:" + x + "y:" +y;
    }


    private void PutValorEnDict(PropiedadCasilla casilla)
    {
        ValorCasilla nuevoValor;
        string key = GeneraKey(casilla.coordenada.x, casilla.coordenada.y);
        if (dictValoresCasilla.TryGetValue(key, out ValorCasilla value))
        {
            nuevoValor = value;
            dictValoresCasilla.Remove(key);
        }
        else
        {
            nuevoValor = new ValorCasilla();
        }
        nuevoValor.x = casilla.coordenada.x;
        nuevoValor.y = casilla.coordenada.y;
        switch (casilla.propiedad)
        {
            case CasillaPropiedadBool.esTablero:
                nuevoValor.esTablero = casilla.valor;
                break;
            case CasillaPropiedadBool.esOcupada:
                nuevoValor.esOcupado = casilla.valor;
                break;
            case CasillaPropiedadBool.esColor1:
                nuevoValor.esColor1 = casilla.valor;
                break;
            case CasillaPropiedadBool.esColor2:
                nuevoValor.esColor2 = casilla.valor;
                break;
            default:
                break;
        }
        dictValoresCasilla.Add(key, nuevoValor);

    }

    public List<List<ValorCasilla>> GetCuadrantesAdyacentes(List<ValorCasilla> cuadrante)
    {
        return GetCuadrantesAdyacentes(cuadrante[2].x, cuadrante[2].y);
    }
    /// <summary>
    /// Devuelve los cuadrantes adyacentes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<List<ValorCasilla>> GetCuadrantesAdyacentes(int x, int y)
    {
        List<List<ValorCasilla>> retorno = new List<List<ValorCasilla>>();
        for (int offsetX = -1; offsetX < 2; offsetX++)
        {
            for (int offsetY = 1; offsetY > -2; offsetY--)
            {
                //Debug.Log("GetAdyacentes>GetCuadranteEnCoordenada x:" +( x + offsetX )+ " y:" + (y + offsetY));
                retorno.Add(GetCuadranteEnCoordenada(x+offsetX, y+offsetY));
            }
        }
        return retorno;
    }

    /// <summary>
    /// Devuelve la casilla correspondiente y las de arriba y derecha
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>

    public List<ValorCasilla> GetCuadranteEnCoordenada(int x, int y)
    {
        List<ValorCasilla> valorCasillas = new List<ValorCasilla>();
        for (int offsetY = 1; offsetY >=0; offsetY--)
        {
            for (int offsetX = 0; offsetX < 2; offsetX++)
            {
                dictValoresCasilla.TryGetValue(GeneraKey(x+ offsetX, y+ offsetY), out ValorCasilla v);
                if (v == null)
                {
                    v = new ValorCasilla();
                    v.x = x + offsetX;
                    v.y = y + offsetY;
                }
                valorCasillas.Add(v);
            }
        }
        return valorCasillas;
    }
    public ValorCasilla GetValorEnCoordenada(int x, int y)
    {
        if (dictValoresCasilla.TryGetValue(GeneraKey(x, y), out ValorCasilla v))
        {
            return v;
        }
        else
        {
            return null;
        }
    }

}
