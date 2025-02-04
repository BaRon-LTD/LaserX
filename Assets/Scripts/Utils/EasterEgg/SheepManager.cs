using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SheepManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private int numberOfSheep = 3;
    [SerializeField] private float spacingBetweenSheep = 2f;
    [SerializeField] private Transform fence;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI jumpCounterText;

    [Header("Screen Bounds")]
    [SerializeField] private float leftBound = -10f;
    [SerializeField] private float rightBound = 10f;

    private List<GameObject> sheepList = new List<GameObject>();
    private int totalJumps = 0;

    private void Start()
    {
        if (fence == null)
        {
            Debug.LogError("Please assign a fence object in the inspector!");
            return;
        }

        if (jumpCounterText == null)
        {
            Debug.LogError("Please assign a TextMeshPro UI text component in the inspector!");
            return;
        }

        SpawnSheep();
        UpdateJumpCounter();
    }

    private void SpawnSheep()
    {
        for (int i = 0; i < numberOfSheep; i++)
        {
            Vector3 spawnPosition = new Vector3(
                leftBound - (i * spacingBetweenSheep),
                0f,
                0f
            );

            GameObject sheep = Instantiate(sheepPrefab, spawnPosition, Quaternion.identity);
            SheepController controller = sheep.GetComponent<SheepController>();
            controller.Initialize(fence);

            // Subscribe to the jump event
            controller.OnSheepJump += IncrementJumpCounter;

            sheepList.Add(sheep);
        }
    }

    private void IncrementJumpCounter()
    {
        totalJumps++;
        UpdateJumpCounter();
    }

    private void UpdateJumpCounter()
    {
        jumpCounterText.text = $"{totalJumps}";
    }

    private void Update()
    {
        foreach (GameObject sheep in sheepList)
        {
            if (sheep.transform.position.x > rightBound)
            {
                sheep.transform.position = new Vector3(
                    leftBound,
                    0f,
                    sheep.transform.position.z
                );
                sheep.GetComponent<SheepController>().ResetJump();
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when the manager is destroyed
        foreach (GameObject sheep in sheepList)
        {
            if (sheep != null)
            {
                var controller = sheep.GetComponent<SheepController>();
                if (controller != null)
                {
                    controller.OnSheepJump -= IncrementJumpCounter;
                }
            }
        }
    }
}