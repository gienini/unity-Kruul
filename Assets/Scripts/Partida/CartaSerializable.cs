
using System.Collections.Generic;

[System.Serializable]
public class CartaSerializable
{
    public int numCuartosConFicha = 0;
    public string valorCuartosCarta = null;
    public Vector3Serializable PosicionTablero;
    public int OrdenCarta;
    public List<int> CartasVecinas;
    public CartaSerializable()
    {

    }
    public CartaSerializable(Carta c)
    {
        numCuartosConFicha = c.NumCuartosConFicha;
        valorCuartosCarta = c.ValorCuartosCarta;
        PosicionTablero = new Vector3Serializable(c.PosicionTablero.x, c.PosicionTablero.y, c.PosicionTablero.z);
        OrdenCarta = c.OrdenCarta;
        CartasVecinas = c.CartasVecinas;
    }
}
