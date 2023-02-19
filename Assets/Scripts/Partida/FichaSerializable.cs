using System.Collections.Generic;
[System.Serializable]
public class FichaSerializable 
{
    public bool _esColor1;
    public List<ValorCasilla> _cuadrante;

    public FichaSerializable()
    {

    }
    public FichaSerializable(Ficha p)
    {
        _esColor1 = p.EsColor1;
        _cuadrante = p.Cuadrante;
    }
}
