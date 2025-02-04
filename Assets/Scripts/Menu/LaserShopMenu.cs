using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class LaserShopMenu : Panel
{
    [SerializeField] private RectTransform container2 = null;
    [SerializeField] private RectTransform container3 = null;

    [SerializeField] private Button BuyButton = null;
    [SerializeField] private Button UseButton = null;

    private List<RectTransform> containers = new List<RectTransform>();
    private int currentContainerIndex = 0;
    private int currentLaserColorIndex = 0;
    [SerializeField] private TextMeshProUGUI coinsCounter;

    public override void Awake()
    {

        Initialize();

        currentLaserColorIndex = GameManager.Instance.GetCurrentLaserColorIndex();
        if (GameManager.Instance.IsLaserColorUnlocked(currentContainerIndex))
        {
            BuyButton.gameObject.SetActive(false);
            UseButton.gameObject.SetActive(true);
            ButtonsSetting();
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        // Populate list with non-null containers
        if (container != null) containers.Add(container);
        if (container2 != null) containers.Add(container2);
        if (container3 != null) containers.Add(container3);

        // Ensure there's at least one valid container
        if (containers.Count == 0) return;

        // Check which container is currently active and update index
        for (int i = 0; i < containers.Count; i++)
        {
            if (containers[i].gameObject.activeSelf)
            {
                currentContainerIndex = i;
                break;
            }
        }
    }

    public override void Open()
    {
        if (!IsInitialized) { Initialize(); }
        if (containers.Count == 0) return;

        // Close the current container before opening the next one
        containers[currentContainerIndex].gameObject.SetActive(false);

        // Move to the next container cyclically
        currentContainerIndex = (currentContainerIndex + 1) % containers.Count;

        // Open the new container
        containers[currentContainerIndex].gameObject.SetActive(true);

        if (GameManager.Instance.IsLaserColorUnlocked(currentContainerIndex))
        {
            BuyButton.gameObject.SetActive(false);
            UseButton.gameObject.SetActive(true);
            ButtonsSetting();
        }
        else
        {
            BuyButton.gameObject.SetActive(true);
            UseButton.gameObject.SetActive(false);
        }
        isOpen = true;
    }

    public override void Close()
    {
        if (!IsInitialized) { Initialize(); }
        if (containers.Count == 0) return;

        // Close the current container
        containers[currentContainerIndex].gameObject.SetActive(false);

        // Move to the previous container cyclically
        currentContainerIndex = (currentContainerIndex - 1 + containers.Count) % containers.Count;

        // Open the previous container
        containers[currentContainerIndex].gameObject.SetActive(true);
        if (GameManager.Instance.IsLaserColorUnlocked(currentContainerIndex))
        {
            BuyButton.gameObject.SetActive(false);
            UseButton.gameObject.SetActive(true);
            ButtonsSetting();
        }
        else
        {
            BuyButton.gameObject.SetActive(true);
            UseButton.gameObject.SetActive(false);
        }
        isOpen = false;
    }

    public void BuyLaserColor()
    {
        if (!IsInitialized) { Initialize(); }

        // Find the text component in the current container
        TextMeshProUGUI costText = containers[currentContainerIndex].GetComponentInChildren<TextMeshProUGUI>();

        if (costText != null && int.TryParse(costText.text, out int laserCost))
        {
            if (GameManager.Instance.GetTotalCoinsCollected() < laserCost)
            {
                Debug.LogWarning("Not enough money to buy this laser color!");
                return;
            }
            coinsCounter.text = "     X " + (GameManager.Instance.GetTotalCoinsCollected() - laserCost);
            GameManager.Instance.ReduceTotalCoinsCollected(laserCost);

            GameManager.Instance.AddLaserColor(currentContainerIndex);
            BuyButton.gameObject.SetActive(false);
            UseButton.gameObject.SetActive(true);
            ButtonsSetting();
        }
    }

    public void UseLaserColor()
    {
        if (!IsInitialized) { Initialize(); }
        GameManager.Instance.SetCurrentLaserColorIndex(currentContainerIndex);
        currentLaserColorIndex = currentContainerIndex;
        ButtonsSetting();
    }

    public void ButtonsSetting()
    {
        TMP_Text buttonText = UseButton.GetComponentInChildren<TMP_Text>();
        RectTransform textRect = buttonText.GetComponent<RectTransform>();

        if (currentContainerIndex == currentLaserColorIndex)
        {
            UseButton.interactable = false;
            buttonText.text = "In Use!";

            // Move text position (X = 7)
            textRect.anchoredPosition = new Vector2(7f, textRect.anchoredPosition.y);

            // Make button transparent
            Image buttonImage = UseButton.GetComponent<Image>();
            Color tempColor = buttonImage.color;
            tempColor.a = 0f;
            buttonImage.color = tempColor;
        }
        else
        {
            UseButton.interactable = true;
            buttonText.text = "Use";

            // Restore text position (X = 15.5)
            textRect.anchoredPosition = new Vector2(15.5f, textRect.anchoredPosition.y);

            // Restore button visibility
            Image buttonImage = UseButton.GetComponent<Image>();
            Color tempColor = buttonImage.color;
            tempColor.a = 1f;
            buttonImage.color = tempColor;
        }


    }
}

