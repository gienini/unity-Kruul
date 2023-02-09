using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPartidaUI : MonoBehaviour
{
    [SerializeField] private GameObject GrupoPiezasColor1 = null;
    [SerializeField] private GameObject GrupoPiezasColor2 = null;
    [SerializeField] private GameObject GrupoDorsosUI = null;
    [SerializeField] private GameObject DorsoPrefab = null;
    [SerializeField] private GameObject FaderPartidaUI = null;
    private List<Image> _piezasColor1;
    private List<Image> _piezasColor2;
    private int _numPiezasColor1 = 9;
    private int _numPiezasColor2 = 9;
    private Camera mainCamera;
    private bool _esTurnoJugador1 = true;
    private List<GameObject> _listDorsos;
    private float _camaraPosicionZ;
    private bool _esTriggerAnimacionInicial = false;
    private bool isFading;
    private CanvasGroup faderCanvasGroup;
    void Start()
    {
        //Orden inicial de los componentes
        FaderPartidaUI.SetActive(true);
        FaderPartidaUI.transform.SetAsLastSibling();
        FaderPartidaUI.GetComponent<Image>().color = new Color(0f,0f,0f,1f);
        faderCanvasGroup = FaderPartidaUI.GetComponent<CanvasGroup>();
        faderCanvasGroup.alpha = 0f;
        _piezasColor1 = new List<Image>();
        foreach (Image child in GrupoPiezasColor1.GetComponentsInChildren<Image>())
        {
            _piezasColor1.Add(child);
        }
        _piezasColor2 = new List<Image>();
        foreach (Image child in GrupoPiezasColor2.GetComponentsInChildren<Image>())
        {
            _piezasColor2.Add(child);
        }
    }
    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.JugadaHechaEvent += JugadaHechaEvent;
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.JugadaHechaEvent -= JugadaHechaEvent;
    }
    public void BotonPilaCartas()
    {
        escondeOpcionesAccion();
        EventHandler.CallAccionSeleccionadaEvent(true);
    }
    public void BotonPilaGrupoPiezas1()
    {
        escondeOpcionesAccion();
        _piezasColor1[_numPiezasColor1 - 1].enabled = false;
        _numPiezasColor1--;
        EventHandler.CallAccionSeleccionadaEvent(false);
    }
    public void BotonPilaGrupoPiezas2()
    {
        escondeOpcionesAccion();
        _piezasColor2[_numPiezasColor2 - 1].enabled = false;
        _numPiezasColor2--;
        EventHandler.CallAccionSeleccionadaEvent(false);
    }
    private void escondeOpcionesAccion()
    {
        FaderPartidaUI.transform.SetAsLastSibling();
        StartCoroutine(Fade(0f));
    }
    private void muestraOpcionesAccion(bool esJugador1)
    {
        StartCoroutine(AnimacionFade(1f, esJugador1));
    }
    private IEnumerator AnimacionFade(float finalAlpha, bool esJugador1)
    {
        yield return Fade(1f);
        FaderPartidaUI.transform.SetAsLastSibling();
        if (esJugador1)
        {
            GrupoPiezasColor1.transform.SetAsLastSibling();
        }
        else
        {
            GrupoPiezasColor2.transform.SetAsLastSibling();
        }
        GrupoDorsosUI.transform.SetAsLastSibling();
        GrupoDorsosUI.GetComponentInChildren<Button>().transform.SetAsLastSibling();
        yield return null;
    }
    private IEnumerator Fade(float finalAlpha)
    {
        //Seteamos a true para que no salte la corutine
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / Settings.FadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            //Movemos opacidad del canvas group que tapa todo en negro
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            //Espera un frame para mostrar el cambio en el fade y seguimos
            yield return null;
        }

        isFading = false;

        faderCanvasGroup.blocksRaycasts = true;
    }
    private void JugadaHechaEvent(bool esTurnoColor1)
    {
        List<ValorCasilla> casillasConPunto = PropiedadesCasillasManager.Instance.CheckPuntoEnTablero(!esTurnoColor1);
        if (casillasConPunto.Count > 0 && ((!esTurnoColor1 && _numPiezasColor1 > 0) || (esTurnoColor1 && _numPiezasColor2 > 0)))
        {
            muestraOpcionesAccion(!esTurnoColor1);

            //DEBUG
            Debug.Log("POSICIONES CON PUNTO");
            foreach(ValorCasilla casilla in casillasConPunto)
            {
                Debug.Log("X:" + casilla.x + "Y:" + casilla.y);
            }
        }else
        {
            //No hay decision, se juega carta
            EventHandler.CallAccionSeleccionadaEvent(true);
        }
    }

    private void DespuesFadeOutEvent()
    {
        if (_esTriggerAnimacionInicial)
        {
            StartCoroutine(instanciaPilaDorsos(Settings.PosicionFinalSpawnCartasUI, 17));
        }
    }

    private void EmpiezaFase1Event()
    {
        mainCamera = Camera.main;
        _camaraPosicionZ = mainCamera.transform.position.z;
        _esTriggerAnimacionInicial = true;
        
    }
    private IEnumerator instanciaPilaDorsos(Vector3 posicion, int barajaCount)
    {
        _listDorsos = new List<GameObject>();
        for (int i = 0; i < barajaCount; i++)
        {
            yield return new WaitForSeconds(0.15f);
            if (i == (barajaCount - 1))
            {
                //Ultima vuelta, la carta va al medio
                //yield return new WaitForSeconds(0.30f);
                //StartCoroutine(instanciaDorso(new Vector3(0, 0, -_camaraPosicionZ), true));
                StartCoroutine(instanciaDorso(new Vector3(posicion.x, posicion.y, -_camaraPosicionZ), true));
                posicion = new Vector3(posicion.x + 12f, posicion.y, 0);

            }
            else
            {
                //StartCoroutine(instanciaDorso(new Vector3(posicion.x + Random.Range(-0.01f, 0.01f), posicion.y + Random.Range(-0.1f, 0.1f), -_camaraPosicionZ)));
                StartCoroutine(instanciaDorso(new Vector3(posicion.x , posicion.y , -_camaraPosicionZ), false));
                posicion = new Vector3(posicion.x + 12f, posicion.y, 0);
            }

        }
        yield return null;
    }

    private IEnumerator instanciaDorso(Vector3 posicionFinal, bool esUltima)
    {
        Vector3 posicionInicial = new Vector3(Random.Range(-12, 21), 10, -_camaraPosicionZ);
        Vector3 porcionViaje = (posicionFinal - posicionInicial) / 100;
        GameObject dorso = Instantiate(DorsoPrefab, posicionInicial, Quaternion.identity, GrupoDorsosUI.transform);
        dorso.transform.localScale = new Vector3(3, 3, 1);
        _listDorsos.Add(dorso);
        for (int i = 0; i < 100; i++)//(dorso.transform.position != posicionFinal)
        {
            dorso.transform.position = dorso.transform.position + porcionViaje;
            yield return new WaitForSeconds(0.01f);
        }
        dorso.transform.rotation = Quaternion.identity;
        if (esUltima)
        {
            EventHandler.CallDespuesIntroFase1Event();
            Destroy(GetDorsoSiguiente());
            //StartCoroutine(VolteaCartaSiguiente());
        }

        yield return null;
    }

    public IEnumerator VolteaCartaSiguiente()
    {
        GameObject dorsoSiguiente = GetDorsoSiguiente();
        dorsoSiguiente.GetComponent<Dorso>().ToggleParticulasEsperar();
        Vector3 posicionFinal = _esTurnoJugador1 ? Settings.PosicionRobaCartaColor1 : Settings.PosicionRobaCartaColor2;
        Vector3 porcionViaje = (posicionFinal - dorsoSiguiente.transform.position) / 100;
        for (int i = 0; i < 100; i++)
        {
            dorsoSiguiente.transform.position = dorsoSiguiente.transform.position + porcionViaje;
            dorsoSiguiente.transform.localScale += new Vector3(0.03f, 0.03f, 0);
            dorsoSiguiente.GetComponent<Dorso>().ParticulasVoltear.gameObject.transform.localScale += new Vector3(0.03f, 0.03f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        dorsoSiguiente.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(2f);

        //Mostramos la carta
        dorsoSiguiente.GetComponent<Dorso>().ToggleDorsoCarta();
        //dorsoSiguiente.GetComponentInChildren<Carta>().ValorCuartosCarta = _baraja.GetSiguiente();
        dorsoSiguiente.GetComponentInChildren<Carta>().ValorCuartosCarta = "1111";
        Destroy(dorsoSiguiente);
        yield return null;
    }
    private GameObject GetDorsoSiguiente()
    {
        GameObject dorsoSiguiente = _listDorsos[_listDorsos.Count - 1];
        _listDorsos.RemoveAt(_listDorsos.Count - 1);
        return dorsoSiguiente;
    }
}
