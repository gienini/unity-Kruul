public enum CasillaPropiedadBool
{
    esTablero,
    esOcupada,
    esColor1,
    esColor2
}

public enum CartaCuartaPartePosicion
{
    ArribaIzquierda = 0,
    ArribaDerecha = 1,
    AbajoIzquierda = 2,
    AbajoDerecha = 3
}

public enum NombresEscena
{
    Escena_PartidaNormal,
    Escena_PartidaTutorial,
    Escena_2,
    none
}
/// <summary>
/// Relacion de eventos de sonido
/// </summary>
public enum SoundName
{
    none = 0,
    menuPrincipalMusica = 10,
    menuPrincipalBoton = 20,
    menuSettingsMusica = 30,
    partidaSeleccionarAccionCarta = 50,
    partidaPonerCarta = 60,
    partidaSeleccionarAccionFicha = 70,
    partidaPonerFicha = 80,
    partidaEventoSeleccionAccion = 90,
    partidaInicio = 95,
    partidaFinalFase1 = 100,
    partidaFinalPartidaGanador = 110,
    partidaFinalPartidaEmpate = 120
}