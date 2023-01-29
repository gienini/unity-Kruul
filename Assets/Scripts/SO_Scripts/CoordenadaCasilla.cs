using UnityEngine;
[System.Serializable]

public class CoordenadaCasilla
{
    public int x;
    public int y;

    public CoordenadaCasilla(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static explicit operator Vector2(CoordenadaCasilla CasillaCoordenada)
    {
        return new Vector2((float)CasillaCoordenada.x, (float)CasillaCoordenada.y);
    }

    public static explicit operator Vector2Int(CoordenadaCasilla CasillaCoordenada)
    {
        return new Vector2Int(CasillaCoordenada.x, CasillaCoordenada.y);
    }

    public static explicit operator Vector3(CoordenadaCasilla CasillaCoordenada)
    {
        return new Vector3((float)CasillaCoordenada.x, (float)CasillaCoordenada.y, 0f);
    }

    public static explicit operator Vector3Int(CoordenadaCasilla CasillaCoordenada)
    {
        return new Vector3Int(CasillaCoordenada.x, CasillaCoordenada.y, 0);
    }
}


