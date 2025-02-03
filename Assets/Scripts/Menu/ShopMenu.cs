using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ShopMenu : Panel
{
    [SerializeField] private TextMeshProUGUI coinsCounter;
    // Update coins counter when opening the panel
    public override void Open()
    {
        coinsCounter.text = "     X " + GameManager.Instance.GetTotalCoinsCollected();
        base.Open();
    }
}      
