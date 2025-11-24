using UnityEngine;

public class SawBlade : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How far to move from the starting position (in Local Space)")]
    public Vector3 moveOffset = new Vector3(0, 0, 5f); 
    public float moveSpeed = 2f;
    public bool useSmoothMovement = true; // Sine wave vs Linear

    [Header("Rotation Settings")]
    [Tooltip("Which axis to spin around (1, 0, 0 is X-axis)")]
    public Vector3 rotationAxis = new Vector3(1, 0, 0); 
    public float rotationSpeed = 360f;

    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        // Remember where we started (in Local space so we can move the parent freely)
        startPos = transform.localPosition;
        endPos = startPos + moveOffset;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        // Simple continuous rotation
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    void HandleMovement()
    {
        // Calculate a value between 0 and 1 that pings back and forth
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);

        // Optional: Smooth out the movement at the ends (Sine Wave)
        if (useSmoothMovement)
        {
            t = Mathf.SmoothStep(0f, 1f, t);
        }

        // Linear Interpolation moves us between start and end based on 't'
        transform.localPosition = Vector3.Lerp(startPos, endPos, t);
    }

    // --- EDITOR VISUALS ---
    // This draws a line in the Scene view so you can see the path before hitting Play
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            // Calculate hypothetical end position based on current inspector values
            Vector3 editorStart = transform.position;
            // Note: TransformDirection converts our local offset to world alignment
            Vector3 editorEnd = editorStart + transform.TransformDirection(moveOffset);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(editorStart, editorEnd);
            Gizmos.DrawSphere(editorEnd, 0.1f);
        }
    }
}