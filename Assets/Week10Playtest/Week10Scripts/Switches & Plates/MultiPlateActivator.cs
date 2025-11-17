using UnityEngine;
using UnityEngine.Events;

public class MultiPlateActivator : MonoBehaviour
{
    public ActivationPlate plate1;
    public ActivationPlate plate2;
    public ActivationPlate plate3;
    
    public GameObject columnToRaise;
    public float raiseSpeed = 1.0f;
    
    private Vector3 initialPosition;
    public Vector3 raisedPosition; 
    
    private Vector3 currentTargetPosition; 
    
    // 💡 NEW: Flag to prevent the column from lowering once raised
    private bool isLockedInRaisedPosition = false;

    private void Start()
    {
        if (columnToRaise != null)
        {
            initialPosition = columnToRaise.transform.position;
            // Set the initial target to the starting position
            currentTargetPosition = initialPosition; 
        }
    }

    private void Update()
    {
        // Perform the smooth movement every frame
        if (columnToRaise.transform.position != currentTargetPosition)
        {
            columnToRaise.transform.position = Vector3.MoveTowards(
                columnToRaise.transform.position, 
                currentTargetPosition, 
                Time.deltaTime * raiseSpeed
            );
        }
        
        // 💡 FIX: Use Vector3.Distance to check if the column has reached the raised position
        if (currentTargetPosition == raisedPosition && Vector3.Distance(columnToRaise.transform.position, raisedPosition) < 0.001f)
        {
            // 💡 OPTIONAL: Snap it to the exact position to clean up any remaining error
            columnToRaise.transform.position = raisedPosition; 
            isLockedInRaisedPosition = true;
        }
    }

    // Call this method whenever a plate's state changes
    public void CheckAllPlates()
    {
        // If the column is already locked up, ignore all plate changes
        if (isLockedInRaisedPosition) 
        {
            return;
        }
        
        bool allActive = plate1.IsActive && plate2.IsActive; 
        
        if (allActive)
        {
            // Column starts raising and will be locked in Update()
            currentTargetPosition = raisedPosition;
        }
        else
        {
            // Only lower the column if it hasn't been successfully raised yet
            currentTargetPosition = initialPosition;
        }
    }
}