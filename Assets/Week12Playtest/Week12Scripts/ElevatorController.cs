using UnityEngine;
using System.Collections.Generic;

public class ElevatorController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many individual robots (or stack height) must be in the elevator?")]
    [SerializeField] private int requiredRobots = 2;
    
    [SerializeField] private Vector3 moveOffset = new Vector3(0, 10, 0); // How high to go
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private bool oneWay = true; // If true, it stays up once raised

    // Internal State
    //private int currentRobotCount = 0;
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isRaised = false;
    private List<GameObject> objectsInElevator = new List<GameObject>();

    void Start()
    {
        startPos = transform.position;
        // Initially, target is start (Bottom)
        targetPos = startPos;
    }

    void Update()
    {
        // Smoothly move to target
        if (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
    }

    // --- LOGIC: COUNT ROBOTS ---

    private void CheckConditions()
    {
        // 1. Clean list (remove nulls if robots died)
        objectsInElevator.RemoveAll(x => x == null);

        // 2. Calculate Total Count
        // We need to be smart: A stack of 2 robots is ONE GameObject, 
        // but it counts as TWO robots.
        int totalCount = 0;

        foreach (GameObject obj in objectsInElevator)
        {
            // Check if it's a stack controller
            NewStackController stack = obj.GetComponent<NewStackController>();
            if (stack != null)
            {
                // Add the stack count (e.g., 1 player + 1 stacked = 2)
                totalCount += stack.CurrentStackCount; 
            }
            else if (obj.CompareTag("Player")) // Fallback for single non-stack player
            {
                totalCount += 1;
            }
        }

        // 3. Decide to Move
        if (totalCount >= requiredRobots)
        {
            RaiseElevator();
        }
        else if (!oneWay)
        {
            LowerElevator();
        }
    }

    private void RaiseElevator()
    {
        if (isRaised) return;
        targetPos = startPos + moveOffset;
        isRaised = true;
        // Optional: Play sound here
    }

    private void LowerElevator()
    {
        if (!isRaised) return;
        targetPos = startPos;
        isRaised = false;
    }

    // --- TRIGGER EVENTS ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        // Check if it's a valid robot/player
        if (other.GetComponent<NewStackController>() || other.CompareTag("Player"))
        {
            GameObject root = other.transform.root.gameObject;
            if (!objectsInElevator.Contains(root))
            {
                objectsInElevator.Add(root);
                CheckConditions();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        if (other.GetComponent<NewStackController>() || other.CompareTag("Player"))
        {
            GameObject root = other.transform.root.gameObject;
            if (objectsInElevator.Contains(root))
            {
                objectsInElevator.Remove(root);
                CheckConditions();
            }
        }
    }
}