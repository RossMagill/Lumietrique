using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [Header("Targeting")]
    [Tooltip("Assign the Player Stack here (or find it automatically)")]
    public Transform playerTransform;
    
    [Header("Tracking Settings")]
    public float horizontalSpeed = 10f;  
    public float verticalSpeed = 2f;     
    public float verticalDeadzone = 2.0f; 

    [Header("Offsets")]
    public Vector3 offset = new Vector3(0, 2, -10);

    void LateUpdate()
    {
        if (playerTransform == null) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = playerTransform.position + offset;

        float newX = Mathf.Lerp(currentPos.x, targetPos.x, Time.deltaTime * horizontalSpeed);
        float newZ = targetPos.z; 

        float heightDiff = targetPos.y - currentPos.y;
        float newY = currentPos.y;

        if (Mathf.Abs(heightDiff) > verticalDeadzone)
        {
            float targetY = targetPos.y - (Mathf.Sign(heightDiff) * verticalDeadzone);
            newY = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * verticalSpeed);
        }

        transform.position = new Vector3(newX, newY, newZ);
    }

    // Call this when the player respawns or changes stack
    public void SetTarget(Transform newTarget)
    {
        playerTransform = newTarget;
    }
}