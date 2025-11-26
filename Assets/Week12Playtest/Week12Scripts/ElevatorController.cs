using UnityEngine;
using System.Collections.Generic;

public class ElevatorController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int requiredRobots = 2;
    [SerializeField] private Vector3 moveOffset = new Vector3(0, 10, 0); 
    [SerializeField] private float elevatorSpeed = 2.0f;
    [SerializeField] private bool oneWay = true; 

    [Header("References")]
    [Tooltip("The manager handling the door logic")]
    //[SerializeField] private ElevatorDoorManager doorManager; 

    // Internal State
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isElevatorActive = false; 

    private List<GameObject> objectsInElevator = new List<GameObject>();

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;
    }

    void Update()
    {
        // Only handles Elevator Movement now
        if (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, elevatorSpeed * Time.deltaTime);
        }
    }

    private void CheckConditions()
    {
        if (isElevatorActive) return; 

        objectsInElevator.RemoveAll(x => x == null);

        int totalCount = 0;
        foreach (GameObject obj in objectsInElevator)
        {
            NewStackController stack = obj.GetComponent<NewStackController>();
            if (stack != null) totalCount += stack.CurrentStackCount; 
            else if (obj.CompareTag("Player")) totalCount += 1;
        }

        if (totalCount >= requiredRobots)
        {
            isElevatorActive = true; 
            targetPos = startPos + moveOffset;
            
            // // Signal the door manager to lock up!
            // if (doorManager != null)
            // {
            //     doorManager.LockDoor();
            // }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

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
                // No need to re-check conditions on exit since we only launch on enter
            }
        }
    }
}