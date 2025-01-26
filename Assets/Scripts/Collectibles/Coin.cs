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
