[System.Serializable]

public class ValorCasilla 
{
    public int x;
    public int y;
    public bool esTablero = false;
    public bool esOcupado = false;
    public bool esColor1 = false;
    public bool esColor2 = false;

    public ValorCasilla()
    {
    }
    public ValorCasilla(ValorCasilla clone)
    {
        x = clone.x;
        y = clone.y;
        esTablero = clone.esTablero;
        esOcupado = clone.esOcupado;
        esColor1 = clone.esColor1;
        esColor2 = clone.esColor2;
    }
    public bool esVacia()
    {
        return esTablero && !esOcupado && !esColor1 && !esColor2;
    }
}
