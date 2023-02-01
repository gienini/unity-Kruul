using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    [SerializeField] private GameObject MenuPrincipalCanvas = null;
    [SerializeField] private List<GameObject> PartidaCanvas = null;
    public NombresEscena startingSceneName;
    private string _escenaActual;

    private IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        StartCoroutine(Fade(0f));
        EventHandler.CallMenuPrincipalEvent();
    }

    public void FadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScene(sceneName));
        }
    }

    private IEnumerator FadeAndSwitchScene(string sceneName)
    {
        EventHandler.CallAntesFadeOutEvent();
        EventHandler.CallFadeOutEvent();
        //yield return de corutina, se pasa la ejecucion al metodo Fade y se para aqui
        yield return StartCoroutine(Fade(1f));
        if (_escenaActual != NombresEscena.none.ToString())
        {
            //Operacion builtin para desmontar una escena
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
        yield return StartCoroutine(Fade(0f));

        EventHandler.CallDespuesFadeOutEvent();

        
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        _escenaActual = sceneName;
        //Caso de menu principal
        if (sceneName != NombresEscena.none.ToString())
        {
            //Operacion builtin para montar una escena. Additive = se añade a la escena existente (en este momento esta cargada la persistent unicamente)
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            MenuPrincipalCanvas.SetActive(false);
            foreach(GameObject go in PartidaCanvas)
            {
                go.SetActive(true);
            }
        }else
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
        foreach(GameObject go in escenaNueva.GetRootGameObjects())
        {
            if (go.name == "PartidaManager")
            {
                go.GetComponent<PartidaManager>().EmpiezaPartida();
                break;
            }
        }
        


    }

    private IEnumerator Fade(float finalAlpha)
    {
        //Seteamos a true para que no salte la corutine
        isFading = true;

        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

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
}
