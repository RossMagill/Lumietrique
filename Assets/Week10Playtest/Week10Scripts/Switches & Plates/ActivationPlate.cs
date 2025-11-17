using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ActivationPlate : MonoBehaviour
{
    [Header("Visual Feedback Settings")]
    [SerializeField] private float depressedAmount = 0.1f; // How far down it lowers (in Unity units)
    [SerializeField] private float speed = 5f;             // Speed of the lowering/raising animation

    [Header("Events")]
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    // Internal State
    private int currentHolders = 0;
    private List<GameObject> objectsOnPlate = new List<GameObject>(); 
    private bool isCurrentlyActive = false;
    private const bool REQUIRE_ROBOT_CONTROLLER = true; 
    public bool IsActive => isCurrentlyActive;

    // Visual State
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition;
    }

    private void Update()
    {
        // Smoothly move the plate towards the target position (raised or lowered)
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }

    private void SetTargetPosition(bool depressed)
    {
        if (depressed)
        {
            // Calculate the lowered position
            targetPosition = initialPosition - new Vector3(0, depressedAmount, 0);
        }
        else
        {
            // Set the target back to the starting position (raised)
            targetPosition = initialPosition;
        }
    }

    private void CheckActivationState()
    {
        bool newActiveState = currentHolders > 0;
        
        if (newActiveState != isCurrentlyActive)
        {
            isCurrentlyActive = newActiveState;
            
            if (isCurrentlyActive)
            {
                Debug.Log($"Plate {gameObject.name} Activated!");
                OnActivated.Invoke();
                SetTargetPosition(true); // <--- LOWER THE PLATE
            }
            else
            {
                Debug.Log($"Plate {gameObject.name} Deactivated!");
                OnDeactivated.Invoke();
                SetTargetPosition(false); // <--- RAISE THE PLATE
            }
        }
    }
    
    // OnTriggerEnter and OnTriggerExit remain the same as before:

    private void OnTriggerEnter(Collider other)
    {
        if (REQUIRE_ROBOT_CONTROLLER && other.GetComponent<RobotController>() == null)
        {
            return;
        }
        GameObject rootObject = other.gameObject.GetComponentInParent<RobotController>()?.gameObject ?? other.gameObject;
        
        if (!objectsOnPlate.Contains(rootObject))
        {
            objectsOnPlate.Add(rootObject);
            currentHolders = objectsOnPlate.Count;
            CheckActivationState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (REQUIRE_ROBOT_CONTROLLER && other.GetComponent<RobotController>() == null)
        {
            return;
        }

        GameObject rootObject = other.gameObject.GetComponentInParent<RobotController>()?.gameObject ?? other.gameObject;

        if (objectsOnPlate.Contains(rootObject))
        {
            objectsOnPlate.Remove(rootObject);
            currentHolders = objectsOnPlate.Count;
            CheckActivationState();
        }
    }
}