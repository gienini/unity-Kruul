using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    public bool isFading;
    public bool esCambiandoEscena;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    [SerializeField] private GameObject MenuPrincipalCanvas = null;
    [SerializeField] private GameObject MenuPausaCanvas = null;

    [SerializeField] private GameObject AccionesCanvas = null;
    [SerializeField] private List<GameObject> PartidaCanvas = null;
    [SerializeField] private GridCursorFase1 gridCursorFase1 = null;
    [SerializeField] private GridCursorFase2 gridCursorFase2 = null;
    [SerializeField] private GameObject FinalPartidaCanvas = null;
    public NombresEscena startingSceneName;
    private string _escenaActual;
    private TextMeshProUGUI _textoFader;
    public string faseActual;
    public string EscenaActual { get => _escenaActual; set => _escenaActual = value; }


    private void OnEnable()
    {
        EventHandler.AcabaFase1Event += AcabaFaseEvent;
        EventHandler.AcabaFase2Event += AcabaFaseEvent;
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event += EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
    }
    private void OnDisable()
    {
        EventHandler.AcabaFase1Event -= AcabaFaseEvent;
        EventHandler.AcabaFase2Event -= AcabaFaseEvent;
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.EmpiezaFase2Event -= EmpiezaFase2Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    private void DespuesFadeOutEvent()
    {
    }

    private void EmpiezaFase2Event()
    {
        gridCursorFase2.gameObject.SetActive(true);
        gridCursorFase1.gameObject.SetActive(false);
        faseActual = "fase2";
    }

    private void EmpiezaFase1Event()
    {
        gridCursorFase1.gameObject.SetActive(true);
        gridCursorFase2.gameObject.SetActive(false);
        AccionesCanvas.gameObject.SetActive(false);
        faseActual = "fase1";
    }

    private void AcabaFaseEvent()
    {
        gridCursorFase1.gameObject.SetActive(false);
        gridCursorFase2.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));
        gridCursorFase1.gameObject.SetActive(false);
        gridCursorFase2.gameObject.SetActive(false);
        StartCoroutine(Fade(0f));
        EventHandler.CallMenuPrincipalEvent();
    }

    public void ToggleMenuPausa()
    {
        MenuPausaCanvas.SetActive(!MenuPausaCanvas.activeSelf);
        foreach (GameObject go in PartidaCanvas)
        {
            go.SetActive(!MenuPausaCanvas.activeSelf);
        }
    }
    public void FadeAndLoadSceneCargar(string sceneName)
    {
        if (!isFading)
        {
            MenuPausaCanvas.SetActive(false);
            StartCoroutine(FadeAndSwitchScene(sceneName, true));
        }
    }
    public void FadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            MenuPausaCanvas.SetActive(false);
            StartCoroutine(FadeAndSwitchScene(sceneName));
        }
    }
    public void FadeAndKeepScene(string textToShow, bool esFinal)
    {
        if (!isFading)
        {

            StartCoroutine(FadeAndKeepRoutine(textToShow, esFinal));
        }
    }
    public void FadeAndKeepScene(string textToShow)
    {
        FadeAndKeepScene(textToShow, false);
    }
    private IEnumerator FadeAndKeepRoutine(string textToShow, bool esFinal)
    {
        _textoFader = faderCanvasGroup.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        
        EventHandler.CallAntesFadeOutEvent();
        EventHandler.CallFadeOutEvent();
        //Fade in
        
        if (!esFinal)
        {
            yield return StartCoroutine(Fade(1f));
            _textoFader.text = textToShow;
            //Fade out
            yield return StartCoroutine(Fade(0f));
            EventHandler.CallDespuesFadeOutEvent();
            _textoFader.text = null;
        }else
        {
            yield return StartCoroutine(Fade(0.5f));
            _textoFader.text = textToShow;
            FinalPartidaCanvas.SetActive(true);
        }
    }
    private IEnumerator FadeAndSwitchScene(string sceneName)
    {
        return FadeAndSwitchScene(sceneName, false) ;
    }
    private IEnumerator FadeAndSwitchScene(string sceneName, bool esCargar)

    {
        
        EventHandler.CallAntesFadeOutEvent();
        EventHandler.CallFadeOutEvent();
        //yield return de corutina, se pasa la ejecucion al metodo Fade y se para aqui
        yield return StartCoroutine(Fade(1f));
        if (EscenaActual != NombresEscena.none.ToString())
        {
            //Operacion builtin para desmontar una escena
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        EscenaActual = sceneName;
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
        yield return StartCoroutine(Fade(0f));

        EventHandler.CallDespuesFadeOutEvent();
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        //Caso de menu principal
        if (sceneName != NombresEscena.none.ToString())
        {
            //Operacion builtin para montar una escena. Additive = se añade a la escena existente (en este momento esta cargada la persistent unicamente)
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            MenuPrincipalCanvas.SetActive(false);
            foreach (GameObject go in PartidaCanvas)
            {
                go.SetActive(true);
            }
        } else
        {
            MenuPrincipalCanvas.SetActive(true);
            foreach (GameObject go in PartidaCanvas)
            {
                go.SetActive(false);
            }
        }

        //Buscamos la escena que se acaba de cargar asincronamente
        Scene escenaNueva = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        //Seteamos la nueva escena como la activa
        SceneManager.SetActiveScene(escenaNueva);
        foreach (GameObject go in escenaNueva.GetRootGameObjects())
        {
            if (go.name == "PartidaManager")
            {
                go.GetComponent<PartidaManager>().EmpiezaPartida();
                break;
            }
        }
    }

    public IEnumerator Fade(float finalAlpha)
    {
        if (_textoFader == null)
        {
            _textoFader = faderCanvasGroup.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (finalAlpha == 1f)
        {
            FinalPartidaCanvas.SetActive(false);
            _textoFader.text = "";
        }
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

        faderCanvasGroup.blocksRaycasts = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleAcciones()
    {
        AccionesCanvas.gameObject.SetActive(!AccionesCanvas.activeSelf);
    }

}
