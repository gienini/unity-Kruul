using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PropiedadCasilla
{
    public CoordenadaCasilla coordenada;
    public CasillaPropiedadBool propiedad;
    public bool valor;

    public PropiedadCasilla(CoordenadaCasilla coordenada, CasillaPropiedadBool propiedad, bool valor)
    {
        this.coordenada = coordenada;
        this.propiedad = propiedad;
        this.valor = valor;
    }
}
