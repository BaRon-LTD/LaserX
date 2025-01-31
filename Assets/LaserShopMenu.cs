using UnityEngine;
using System.Collections.Generic;

public class LaserShopMenu : Panel
{
    [SerializeField] private RectTransform container2 = null;
    [SerializeField] private RectTransform container3 = null;

    private List<RectTransform> containers = new List<RectTransform>();
    private int currentContainerIndex = 0;

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
        isOpen = false;
    }
}
