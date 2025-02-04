using UnityEngine;

public class MapMenu : Panel
{
    [SerializeField] private Transform levelsContainer;
    private GameManager gameManager;

    public override void Awake()
    {
        base.Awake();
        gameManager = GameManager.Instance;
    }

    // Override the Open method if additional logic is needed for enabling the levels and coins
    public override void Open()
    {
        base.Open(); // Call the base class Open method to show the container
        Debug.Log("MapMenu opened!");

        // Iterate through all levels (level1 to level8) and update coins
        for (int i = 1; i <= 8; i++)
        {
            string levelName = "level" + i; // Construct level name
            Transform level = levelsContainer.Find(levelName);  // Find the level by its name

            if (level != null)
            {
                // Retrieve the coins for this level: leftcoin, middlecoin, rightcoin
                Transform leftCoin = level.Find("leftcoin");
                Transform middleCoin = level.Find("middlecoin");
                Transform rightCoin = level.Find("rightcoin");

                // Get the number of coins collected for the current scene (level)
                int coinsCollected = gameManager.GetCoinsCollectedInScene(levelName);

                // Update visibility based on coins collected for the level
                UpdateCoinVisibility(leftCoin, 1, coinsCollected);
                UpdateCoinVisibility(middleCoin, 2, coinsCollected);
                UpdateCoinVisibility(rightCoin, 3, coinsCollected);
            }
        }
    }

    // Method to update visibility of coins based on coins collected
    private void UpdateCoinVisibility(Transform coinTransform, int coinPosition, int coinsCollected)
    {
        if (coinTransform != null)
        {
            // Enable or disable the coin based on the number of coins collected
            // Example: If coinsCollected is greater than or equal to the coin position, enable the coin
            if (coinsCollected >= coinPosition)
            {
                coinTransform.gameObject.SetActive(true); // Enable the coin
            }
            else
            {
                coinTransform.gameObject.SetActive(false); // Disable the coin
            }
        }
    }

    // Override the Close method if additional logic is needed
    public override void Close()
    {
        base.Close(); // Call the base class Close method to hide the container
        Debug.Log("MapMenu closed!");
    }
}

