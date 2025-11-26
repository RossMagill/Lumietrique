using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum PlateOrientation
{
    Floor,      // Standard: Moves Down (Local -Y)
    Ceiling,    // Moves Up (Local +Y)
    Wall_Front, // Moves Backward (Local -Z)
    Wall_Back,  // Moves Forward (Local +Z)
    Wall_Left,  // Moves Right (Local +X)
    Wall_Right  // Moves Left (Local -X)
}

public class ActivationPlate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlateOrientation orientation = PlateOrientation.Wall_Left; // Dropdown!
    [SerializeField] private float depressionDistance = 0.15f; 
    [SerializeField] private float moveSpeed = 5f;

    [Header("Events")]
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    // State
    private int objectsOnPlateCount = 0;
    private bool isActive = false;
    public bool IsActive => isActive;

    // Positions
    private Vector3 restingLocalPos;
    private Vector3 targetLocalPos;

    void Start()
    {
        restingLocalPos = transform.localPosition;
        targetLocalPos = restingLocalPos;
    }

    void Update()
    {
        // Smooth movement
        if (Vector3.Distance(transform.localPosition, targetLocalPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * moveSpeed);
        }
    }

    private void UpdateState()
    {
        bool shouldBeActive = objectsOnPlateCount > 0;

        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
            if (isActive)
            {
                OnActivated.Invoke();
                targetLocalPos = GetPressedPosition();
            }
            else
            {
                OnDeactivated.Invoke();
                targetLocalPos = restingLocalPos;
            }
        }
    }

    private Vector3 GetPressedPosition()
    {
        Vector3 offset = Vector3.zero;

        // Determine which way is "In" based on the dropdown
        switch (orientation)
        {
            case PlateOrientation.Floor:      offset = new Vector3(0, -depressionDistance, 0); break;
            case PlateOrientation.Ceiling:    offset = new Vector3(0, depressionDistance, 0); break;
            
            // For walls, you might need to experiment once to see which axis matches your art
            // Usually:
            case PlateOrientation.Wall_Left:  offset = new Vector3(depressionDistance, 0, 0); break;  // Moves Right (+X)
            case PlateOrientation.Wall_Right: offset = new Vector3(-depressionDistance, 0, 0); break; // Moves Left (-X)
            case PlateOrientation.Wall_Front: offset = new Vector3(0, 0, -depressionDistance); break; // Moves Back (-Z)
            case PlateOrientation.Wall_Back:  offset = new Vector3(0, 0, depressionDistance); break;  // Moves Forward (+Z)
        }

        return restingLocalPos + offset;
    }

    // ---------------------- TRIGGER LOGIC ----------------------

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlateCount++;
            UpdateState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlateCount--;
            if (objectsOnPlateCount < 0) objectsOnPlateCount = 0;
            UpdateState();
        }
    }

    private bool IsValidObject(Collider other)
    {
        if (other.isTrigger) return false;
        return other.CompareTag("Player") || other.GetComponentInParent<NewStackController>();
    }
}