using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitToScreenScript : MonoBehaviour
{
    private Camera mainCamera;
    private CanvasScaler scaler;
    private Vector3 screenSize;
    private RectTransform rectTransform;

    private Vector2 lastRes;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();

        scaler = GetComponent<CanvasScaler>();
        mainCamera = Camera.main;

        AdjustCanvasScaler();
    }

    void FixedUpdate()
    {
        // This section is for testing resolution changes
        // Update every 30 frames to reduce frequency
        if (Time.frameCount % 30 != 0) return;

        Vector2 currentRes = new Vector2(Screen.width, Screen.height);

        // Check for resolution changes
        if (currentRes != lastRes)
        {
            AdjustCanvasScaler();
            lastRes = currentRes;
        }
    }

    private void AdjustCanvasScaler()
    {
        // Get screen dimensions and calculate aspect ratio
        screenSize = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane)) -
                     mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));

        float screenRatio = screenSize.x / screenSize.y;
        float referenceRatio = scaler.referenceResolution.x / scaler.referenceResolution.y;

        // Adjust scaling based on the comparison of screen and reference aspect ratios
        if (screenRatio > referenceRatio)
        {
            scaler.matchWidthOrHeight = 1; // Match width if screen is wider
        }
        else
        {
            scaler.matchWidthOrHeight = 0; // Match height if screen is taller
        }

        //Debug.Log("Adjusted scaler - matchWidthOrHeight: " + scaler.matchWidthOrHeight);
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
