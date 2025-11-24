using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class RunnerMovement : MonoBehaviour, IMovement
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference dashAction;

    [Header("Movement")]
    public float moveSpeed = 100f;
    public float acceleration = 50f;
    public float deceleration = 60f;
    public float maxHorizontalSpeed = 30f;

    [Header("Dash")]
    public float dashSpeed = 60f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    public bool canAirDash = true;

    // --- NEW SECTION ---
    [Header("VFX")]
    [Tooltip("Particle effect for ground dashing (e.g., dust kickup)")]
    [SerializeField] private GameObject dashDustVFX;
    [Tooltip("Slight upward offset so dust doesn't clip into the floor")]
    [SerializeField] private float vfxYOffset = 0.1f;
    // --------------------

    [Header("Grounding / Rays")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.1f;

    [Header("Gravity (better-fall feel)")]
    public float ascendGravityMultiplier = 1.8f;
    public float fallGravityMultiplier = 3.2f;

    [Header("Slice Lock")]
    public float sliceZ = 0f;

    private Rigidbody rb;
    private BoxCollider box;

    // Dash State
    private bool isDashing = false;
    private float lastDashTime;
    private float facingDirection = 1f; // 1 for right, -1 for left

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();

        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        dashAction?.action.Enable();
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        dashAction?.action.Disable();
    }

    void Update()
    {
        if (dashAction != null && dashAction.action.WasPressedThisFrame())
        {
            AttemptDash();
        }
    }

    void FixedUpdate()
    {
        // 1. Z-Lock
        var p = rb.position;
        if (Mathf.Abs(p.z - sliceZ) > 0.0001f)
            rb.position = new Vector3(p.x, p.y, sliceZ);

        // 2. IF DASHING: Override physics
        if (isDashing)
        {
            rb.linearVelocity = new Vector3(facingDirection * dashSpeed, 0f, 0f);
            return; 
        }

        // 3. Normal Movement Logic
        float inputX = moveAction ? moveAction.action.ReadValue<float>() : 0f;

        if (Mathf.Abs(inputX) > 0.01f)
        {
            facingDirection = Mathf.Sign(inputX);
        }

        float targetX = inputX * moveSpeed;
        float rate = (Mathf.Abs(inputX) > 0.01f) ? acceleration : deceleration;

        Vector3 v = rb.linearVelocity;
        float newX = Mathf.MoveTowards(v.x, targetX, rate * Time.fixedDeltaTime);
        v.x = Mathf.Clamp(newX, -maxHorizontalSpeed, maxHorizontalSpeed);

        // 4. Gravity Logic
        float g = Physics.gravity.y;
        bool rising  = v.y >  0.01f;
        bool falling = v.y < -0.01f;

        if (falling) v += Vector3.up * (g * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime);
        else if (rising) v += Vector3.up * (g * (ascendGravityMultiplier - 1f) * Time.fixedDeltaTime);

        // 5. Wall Logic
        float wallDir = IsWalled();
        if (wallDir != 0 && !IsGrounded() && v.y < 0f)
        {
            if (inputX > 0 && wallDir == 1)      v.y = -5f; 
            else if (inputX < 0 && wallDir == -1) v.y = -5f; 
        }

        rb.linearVelocity = v;
    }

    // ---------------------- Dash Logic ----------------------

    private void AttemptDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        if (!canAirDash && !IsGrounded()) return;

        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        lastDashTime = Time.time;
        
        // --- TRIGGER VFX ---
        SpawnDashVFX();
        // -------------------

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    // --- NEW HELPER FOR VFX ---
    private void SpawnDashVFX()
    {
        // 1. Safety checks: Must have prefab, must be on ground for "dust"
        if (dashDustVFX == null || !IsGrounded()) return;

        // 2. Calculate Feet Position
        // box.bounds.min.y gives the absolute bottom of the collider
        Vector3 bottomCenter = new Vector3(box.bounds.center.x, box.bounds.min.y, box.bounds.center.z);
        Vector3 spawnPos = bottomCenter + new Vector3(0, vfxYOffset, 0);

        // 3. Calculate Rotation (The Fix)
        // Assuming the prefab looks correct for facing RIGHT by default.
        // If facing left (< 0), rotate 180 degrees on Y axis. Else, zero rotation.
        Quaternion rotation = (facingDirection < 0) ? Quaternion.Euler(0, 180f, 0) : Quaternion.identity;

        // 4. Spawn and cleanup
        GameObject vfx = Instantiate(dashDustVFX, spawnPos, rotation);
        Destroy(vfx, 2.0f); // Adjust time based on your particle duration
    }


    // ---------------------- Helpers ----------------------

    bool IsGrounded()
    {
        var b = box.bounds;
        float y = b.min.y + 0.01f;
        float d = groundCheckDistance;
        float inset = 0.02f;

        Vector3[] offsets =
        {
            new( 0f, 0f,  0f), 
            new( b.extents.x - inset, 0f,  0f),
            new(-b.extents.x + inset, 0f,  0f),
            new( 0f, 0f,  b.extents.z - inset),
            new( 0f, 0f, -b.extents.z + inset),
            new( b.extents.x - inset, 0f,  b.extents.z - inset),
            new( b.extents.x - inset, 0f, -b.extents.z + inset),
            new(-b.extents.x + inset, 0f,  b.extents.z - inset),
            new(-b.extents.x + inset, 0f, -b.extents.z + inset),
        };

        foreach (var o in offsets)
        {
            Vector3 origin = new(b.center.x + o.x, y, b.center.z + o.z);
            if (Physics.Raycast(origin, Vector3.down, d, groundMask, QueryTriggerInteraction.Ignore))
                return true;
        }
        return false;
    }

    float IsWalled()
    {
        Vector3 origin = box.bounds.center;
        float distance = box.bounds.extents.x + groundCheckDistance;

        bool leftWall  = Physics.Raycast(origin, Vector3.left,  distance, groundMask, QueryTriggerInteraction.Ignore);
        bool rightWall = Physics.Raycast(origin, Vector3.right, distance, groundMask, QueryTriggerInteraction.Ignore);

        if (leftWall && !rightWall)   return -1f;
        if (rightWall && !leftWall)   return  1f;
        return 0f;
    }

    public void EnableMovement()  { enabled = true;  }
    public void DisableMovement() { enabled = false; }
}