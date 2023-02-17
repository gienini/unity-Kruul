using System.Collections.Generic;
[System.Serializable]
public class PiezaSerializable 
{
    public bool _esColor1;
    public List<ValorCasilla> _cuadrante;

    public PiezaSerializable()
    {

    }
    public PiezaSerializable(Pieza p)
    {
        _esColor1 = p.EsColor1;
        _cuadrante = p.Cuadrante;
    }
}
