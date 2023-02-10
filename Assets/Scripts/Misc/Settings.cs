using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    //Efectos
    public static float TiempoLevantarCarta = 1f;
    public static float FadeDuration = 1f;
    //Partida
    public static int NumCartasTotal = 18;
    //Posiciones
    public static Vector3 PosicionRobaCartaColor1 = new Vector3(9.03f, -4.2f, 0);
    public static Vector3 PosicionRobaCartaColor2 = new Vector3(9.03f, -4.2f, 0);
    public static Vector3 PosicionRobaCartaCentroArriba = new Vector3(9.5f, -7.1f, 0);
    public static float SeparacionSpawnCartasUI = 8f;


    //AUTO
    public static Vector3Int PosicionRobaCartaColor1Int = new Vector3Int((int)PosicionRobaCartaColor1.x, (int)PosicionRobaCartaColor1.y, (int)PosicionRobaCartaColor1.z);
    public static Vector3Int PosicionRobaCartaColor2Int = new Vector3Int((int)PosicionRobaCartaColor2.x, (int)PosicionRobaCartaColor2.y, (int)PosicionRobaCartaColor2.z);
}
