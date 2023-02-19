
using System.Collections.Generic;
[System.Serializable]

public class SceneSave
{
    public Dictionary<string, int> intDictionary;
    public Dictionary<string, bool> boolDictionary;
    public Dictionary<string, string> stringDictionary;
    public Dictionary<string, int[]> intArrayDictionary;
    public Dictionary<string, ValorCasilla> dictValoresCasilla;
    public Dictionary<string, CartaSerializable> dictCoordenadasCarta;
    public Dictionary<string, FichaSerializable> dictCoordenadasPieza;
    public CartaSerializable cartaEscondidaCursor;
    public List<ValorCasilla> cuadranteEscondidoCursor;
    public List<CartaSerializable> cartasEscondidas;
    public List<List<ValorCasilla>> cuadrantesEscondidos;
    public Dictionary<string, List<Vector3Serializable>> dictVector3;
}
