using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void PlayGame(){
        gameManager.LoadScene("Tutorial_1");
    }
}
