using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private float fadeSpeed = 2f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // Get the root Canvas parent of the loading panel
            Canvas rootCanvas = loadingPanel.GetComponentInParent<Canvas>();
            if (rootCanvas != null)
            {
                // Mark both the LoadingManager and the Canvas hierarchy for preservation
                DontDestroyOnLoad(rootCanvas.gameObject);
                
                // If LoadingManager isn't a child of the Canvas, preserve it separately
                if (!transform.IsChildOf(rootCanvas.transform))
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Debug.LogError("LoadingManager: No Canvas found in parent hierarchy of loading panel!");
            }
            
            // Make sure panel starts hidden
            if (loadingPanel != null)
            {
                loadingPanel.alpha = 0;
                loadingPanel.blocksRaycasts = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Show loading screen
        yield return StartCoroutine(FadeLoadingScreen(true));

        // Start loading the new scene
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;

        // Wait until the scene is loaded
        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }

        // Give a short delay to prevent visual "pop"
        yield return new WaitForSeconds(0.2f);
        
        // Activate the scene
        loadOperation.allowSceneActivation = true;

        // Wait one frame to ensure scene is fully active
        yield return null;

        // Hide loading screen
        yield return StartCoroutine(FadeLoadingScreen(false));
    }

    private IEnumerator FadeLoadingScreen(bool fadeIn)
    {
        if (loadingPanel == null) yield break;

        float targetAlpha = fadeIn ? 1f : 0f;
        loadingPanel.blocksRaycasts = fadeIn;

        while (!Mathf.Approximately(loadingPanel.alpha, targetAlpha))
        {
            loadingPanel.alpha = Mathf.MoveTowards(loadingPanel.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }
}