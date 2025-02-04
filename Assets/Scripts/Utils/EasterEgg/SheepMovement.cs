using UnityEngine;
using System.Collections.Generic;

public class SheepController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpDuration = 1f;

    [Header("Jump Timing")]
    [SerializeField] private float jumpStartDistance = 1f;
    [SerializeField] private float jumpEndDistance = 1f;

    [Header("Layer Settings")]
    [SerializeField] private int defaultSortingOrder = 1;
    [SerializeField] private int behindFenceSortingOrder = 0;

    private float initialY;
    private float jumpProgress = 0f;
    private bool isJumping = false;
    private bool hasJumped = false;
    private float jumpStartX;
    private float jumpEndX;
    private Transform fenceTransform;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on sheep!");
        }
    }

    public void Initialize(Transform fence)
    {
        initialY = transform.position.y;
        fenceTransform = fence;
        hasJumped = false;
        jumpStartX = fenceTransform.position.x - jumpStartDistance;
        jumpEndX = fenceTransform.position.x + jumpEndDistance;

        // Set initial sorting order
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = defaultSortingOrder;
        }
    }

    private void Update()
    {
        Vector3 position = transform.position;
        position.x += walkSpeed * Time.deltaTime;

        // Check if we've passed the fence
        if (position.x > fenceTransform.position.x && spriteRenderer.sortingOrder != behindFenceSortingOrder)
        {
            spriteRenderer.sortingOrder = behindFenceSortingOrder;
        }

        if (!hasJumped && !isJumping && position.x >= jumpStartX)
        {
            StartJump();
        }

        if (isJumping)
        {
            float totalJumpDistance = jumpEndX - jumpStartX;
            float currentJumpDistance = position.x - jumpStartX;
            jumpProgress = currentJumpDistance / totalJumpDistance;

            if (jumpProgress >= 1f)
            {
                isJumping = false;
                position.y = initialY;
            }
            else
            {
                float normalizedHeight = Mathf.Sin(jumpProgress * Mathf.PI);
                position.y = initialY + (jumpHeight * normalizedHeight);
            }
        }

        transform.position = position;
    }

    public delegate void OnSheepJumpHandler();
    public event OnSheepJumpHandler OnSheepJump;

    private void StartJump()
    {
        isJumping = true;
        hasJumped = true;
        jumpProgress = 0f;

        // Trigger the jump event
        OnSheepJump?.Invoke();
    }

    public void ResetJump()
    {
        hasJumped = false;
        isJumping = false;
        jumpProgress = 0f;
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = defaultSortingOrder;
        }
    }
}