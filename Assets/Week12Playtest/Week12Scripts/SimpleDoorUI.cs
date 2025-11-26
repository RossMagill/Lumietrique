using UnityEngine;
using System.Collections.Generic;

public class SimpleDoorUI : MonoBehaviour
{
    [Header("UI Images")]
    [Tooltip("Drag your images here in order: 0/3, 1/3, 2/3, 3/3")]
    [SerializeField] private List<GameObject> countImages;
    
    [Tooltip("The 'Bring to Elevator' message")]
    [SerializeField] private GameObject bringToElevator;

    [Tooltip("How long to show the final 'Complete' image before switching to Elevator msg?")]
    [SerializeField] private float completeHideDelay = 3.0f;

    private void Start()
    {
        // Hide everything at start (including the elevator message)
        HideAll();
        
        // Optional: If you want to show 0/3 at start, call UpdateCount(0) manually here.
        // UpdateCount(0); 
    }

    public void UpdateCount(int currentCount)
    {
        if (currentCount < 0 || currentCount >= countImages.Count) return;

        // 1. Reset everything
        HideAll();
        CancelInvoke(nameof(ShowElevatorMessage)); // Cancel pending messages if count dropped

        // 2. Show the current progress image
        if (countImages[currentCount] != null)
        {
            countImages[currentCount].SetActive(true);
        }

        // 3. Check for Completion
        if (currentCount == countImages.Count - 1)
        {
            // Success! Wait 3 seconds, then hide the "3/3" image and show "Go to Elevator"
            Invoke(nameof(ShowElevatorMessage), completeHideDelay);
        }
    }

    private void ShowElevatorMessage()
    {
        // Hide the "3/3" image
        foreach (var img in countImages) if (img != null) img.SetActive(false);

        // Show the "Go to Elevator" message
        if (bringToElevator != null) bringToElevator.SetActive(true);
    }

    private void HideAll()
    {
        foreach (var img in countImages)
        {
            if (img != null) img.SetActive(false);
        }

        if (bringToElevator != null) bringToElevator.SetActive(false);
    }
}