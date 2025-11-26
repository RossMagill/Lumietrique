using UnityEngine;
using System.Collections.Generic;

public class MultiPlateManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Drag all the ActivationPlate scripts here that affect this door.")]
    [SerializeField] private List<ActivationPlate> requiredPlates = new List<ActivationPlate>();

    [Tooltip("Drag the object you want to move (e.g., Door or Column)")]
    [SerializeField] private Transform targetObject;

    [Header("Movement Settings")]
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3, 0); // Where it goes
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private bool smoothMovement = true;

    // Internal State
    private Vector3 closedPos;
    private Vector3 openPos;
    private Vector3 currentTargetPos;
    private bool isLockedOpen = false;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError($"{name} is missing a Target Object to move!");
            enabled = false;
            return;
        }

        // Remember positions
        closedPos = targetObject.localPosition;
        openPos = closedPos + openOffset;
        currentTargetPos = closedPos;

        // Subscribe to every plate's UnityEvent
        foreach (var plate in requiredPlates)
        {
            if (plate != null)
            {
                // We listen for BOTH activate and deactivate to re-check the state
                plate.OnActivated.AddListener(CheckAllPlates);
                plate.OnDeactivated.AddListener(CheckAllPlates);
            }
        }
    }

    void OnDestroy()
    {
        // Cleanup listeners to prevent memory leaks
        foreach (var plate in requiredPlates)
        {
            if (plate != null)
            {
                plate.OnActivated.RemoveListener(CheckAllPlates);
                plate.OnDeactivated.RemoveListener(CheckAllPlates);
            }
        }
    }

    void Update()
    {
        if (targetObject == null) return;

        // Move the object
        float step = moveSpeed * Time.deltaTime;
        
        if (smoothMovement)
        {
            targetObject.localPosition = Vector3.Lerp(targetObject.localPosition, currentTargetPos, step);
        }
        else
        {
            targetObject.localPosition = Vector3.MoveTowards(targetObject.localPosition, currentTargetPos, step);
        }

        // Check for Lock condition (Reached the top)
        if (!isLockedOpen && currentTargetPos == openPos)
        {
            if (Vector3.Distance(targetObject.localPosition, openPos) < 0.01f)
            {
                targetObject.localPosition = openPos; // Snap to finish
                isLockedOpen = true;
                // Optional: Play "Locked" sound here
            }
        }
    }

    // This runs whenever ANY plate changes state
    private void CheckAllPlates()
    {
        if (isLockedOpen) return; // Don't close if we already finished

        bool allPressed = true;

        // Check every plate in the list
        foreach (var plate in requiredPlates)
        {
            // We access the public property IsActive on your new ActivationPlate script
            if (!plate.IsActive)
            {
                allPressed = false;
                break; // One is missing, so fail early
            }
        }

        if (allPressed)
        {
            currentTargetPos = openPos;
        }
        else
        {
            currentTargetPos = closedPos;
        }
    }
}