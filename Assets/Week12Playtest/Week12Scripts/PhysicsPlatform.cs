using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveOffset = new Vector3(0, 0, 5f); 
    public float moveSpeed = 2f;
    public bool useSmoothMovement = true;

    // --- NEW: Public Property for the Player to read ---
    public Vector3 CurrentVelocity { get; private set; }
    // --------------------------------------------------

    private Vector3 startLocalPos;
    private Vector3 endLocalPos;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void Start()
    {
        startLocalPos = transform.localPosition;
        endLocalPos = startLocalPos + moveOffset;
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * moveSpeed, 1f);

        if (useSmoothMovement)
            t = Mathf.SmoothStep(0f, 1f, t);

        Vector3 nextLocalPos = Vector3.Lerp(startLocalPos, endLocalPos, t);
        
        Vector3 nextWorldPos;
        if (transform.parent != null) nextWorldPos = transform.parent.TransformPoint(nextLocalPos);
        else nextWorldPos = nextLocalPos;

        // --- NEW: Calculate Velocity Manualy ---
        // (Destination - Current) / Time = Speed
        CurrentVelocity = (nextWorldPos - rb.position) / Time.fixedDeltaTime;
        // ---------------------------------------

        rb.MovePosition(nextWorldPos);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Vector3 editorStart = transform.position;
            Vector3 editorEnd = editorStart + transform.TransformDirection(moveOffset);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(editorStart, editorEnd);
            Gizmos.DrawSphere(editorEnd, 0.1f);
        }
    }
}