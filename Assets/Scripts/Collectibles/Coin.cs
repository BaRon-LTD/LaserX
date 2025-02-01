using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private string coinID; // Unique ID for each coin

    public string CoinID => coinID;

    private void Awake()
    {
        // Generate a unique ID if coinID is not set
        if (string.IsNullOrEmpty(coinID))
        {
            coinID = System.Guid.NewGuid().ToString();
        }

        // During initialization, check if the coin has already been collected
        if (GameManager.Instance.SaveManager.IsCoinAlreadyCollected(coinID))
        {
            // Destroy the parent GameObject if the coin has already been collected
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject); // Destroy the parent object
            }
            else
            {
                // If no parent exists, just deactivate the coin itself
                gameObject.SetActive(false);
            }
        }
    }
    
    public void CollectCoin()
    {
        // Notify the GameManager about the coin collection
        GameManager.Instance.AddScore(1, coinID);

        // Optionally log the collection
        Debug.Log($"Coin {coinID} collected!");

        // Destroy the coin after notifying
        Destroy(gameObject);
    }
}
