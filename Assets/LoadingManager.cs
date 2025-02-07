using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private TransitionType transitionType = TransitionType.Fade;
    
    public enum TransitionType
    {
        Fade,
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        ScaleUp,
        ScaleDown,
        RotateIn,
        RotateOut,
        DiagonalSlide,
        Curtain
    }
    
    private Canvas rootCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        rootCanvas = loadingPanel.GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            DontDestroyOnLoad(rootCanvas.gameObject);
            if (!transform.IsChildOf(rootCanvas.transform))
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        
        if (loadingPanel != null)
        {
            loadingPanel.alpha = 0;
            loadingPanel.blocksRaycasts = false;
            rootCanvas.enabled = false; // Disable the canvas initially
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(ShowTransition());
        
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }

        loadOperation.allowSceneActivation = true;
        yield return null;

        yield return StartCoroutine(HideTransition());
    }

    private IEnumerator ShowTransition()
    {
        RectTransform panelRect = loadingPanel.GetComponent<RectTransform>();
        float progress = 0f;
        loadingPanel.blocksRaycasts = true;

        // Enable the canvas before starting the transition
        rootCanvas.enabled = true;

        // Reset initial state
        panelRect.localScale = Vector3.one;
        panelRect.localRotation = Quaternion.identity;
        loadingPanel.alpha = 1;

        switch (transitionType)
        {
            case TransitionType.Fade:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    loadingPanel.alpha = progress;
                    yield return null;
                }
                break;

            case TransitionType.SlideLeft:
                Vector2 startPosLeft = new Vector2(2500, 0); // Start from far right
                Vector2 endPos = Vector2.zero;
                panelRect.anchoredPosition = startPosLeft;

                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(startPosLeft, endPos, progress);
                    yield return null;
                }
                break;

            case TransitionType.SlideRight:
                Vector2 startPosRight = new Vector2(-2500, 0); // Start from far left
                panelRect.anchoredPosition = startPosRight;

                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(startPosRight, Vector2.zero, progress);
                    yield return null;
                }
                break;

            case TransitionType.SlideUp:
                Vector2 startPosUp = new Vector2(0, -2500); // Start from far down
                panelRect.anchoredPosition = startPosUp;

                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(startPosUp, Vector2.zero, progress);
                    yield return null;
                }
                break;

            case TransitionType.SlideDown:
                Vector2 startPosDown = new Vector2(0, 2500); // Start from far up
                panelRect.anchoredPosition = startPosDown;

                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(startPosDown, Vector2.zero, progress);
                    yield return null;
                }
                break;

            case TransitionType.DiagonalSlide:
                Vector2 startPosDiagonal = new Vector2(2500, 2500); // Start from diagonal corner
                panelRect.anchoredPosition = startPosDiagonal;

                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(startPosDiagonal, Vector2.zero, progress);
                    yield return null;
                }
                break;

            case TransitionType.Curtain:
                panelRect.localScale = new Vector3(0, 1, 1);
                
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.localScale = new Vector3(progress, 1, 1);
                    yield return null;
                }
                break;
        }
    }

    private IEnumerator HideTransition()
    {
        RectTransform panelRect = loadingPanel.GetComponent<RectTransform>();
        float progress = 0f;

        Vector2 offScreenLeft = new Vector2(-Screen.width, 0);
        Vector2 offScreenRight = new Vector2(Screen.width, 0);
        Vector2 offScreenUp = new Vector2(0, Screen.height);
        Vector2 offScreenDown = new Vector2(0, -Screen.height);
        Vector2 offScreenDiagonal = new Vector2(Screen.width, Screen.height);

        switch (transitionType)
        {
            case TransitionType.Fade:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    loadingPanel.alpha = 1 - progress;
                    yield return null;
                }
                break;

            case TransitionType.SlideLeft:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(Vector2.zero, offScreenLeft, progress);
                    yield return null;
                }
                break;

            case TransitionType.SlideRight:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(Vector2.zero, offScreenRight, progress);
                    yield return null;
                }
                break;

            case TransitionType.SlideUp:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(Vector2.zero, offScreenUp, progress);
                    yield return null;
                }
                break;

            case TransitionType.SlideDown:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(Vector2.zero, offScreenDown, progress);
                    yield return null;
                }
                break;

            case TransitionType.DiagonalSlide:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.anchoredPosition = Vector2.Lerp(Vector2.zero, offScreenDiagonal, progress);
                    yield return null;
                }
                break;

            case TransitionType.ScaleUp:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.localScale = Vector3.one * (1 + progress); // Scale beyond 1 to simulate moving out
                    loadingPanel.alpha = 1 - progress;
                    yield return null;
                }
                break;

            case TransitionType.ScaleDown:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.localScale = Vector3.one * (1 + progress); // Move out instead of shrinking
                    loadingPanel.alpha = 1 - progress;
                    yield return null;
                }
                break;

            case TransitionType.Curtain:
                while (progress < 1)
                {
                    progress += Time.deltaTime * transitionSpeed;
                    panelRect.localScale = new Vector3(1 - progress, 1, 1);
                    yield return null;
                }
                break;
        }

        loadingPanel.blocksRaycasts = false;
        rootCanvas.enabled = false; // Disable the canvas after the transition
    }

    public void SetTransitionType(TransitionType newType)
    {
        transitionType = newType;
    }
}