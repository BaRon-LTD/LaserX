using UnityEngine;
using UnityEngine.Scripting;

public class MenuManager : MonoBehaviour
{
    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    [Preserve]
    public void PlayGame()
    {
        gameManager.LoadScene("tutorial_1");
    }
}
