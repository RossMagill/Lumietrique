using UnityEngine;
using System.Collections; // Required for Coroutines

public class LaunchPad : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The power of the launch. Higher numbers = higher jump.")]
    public float launchForce = 15f;

    [Tooltip("If true, launches along the pad's 'Up' direction (good for angled bounce pads). If false, always launches straight up world-space.")]
    public bool usePadRotation = false;

    [Header("Animation")]
    [Tooltip("How high the pad pops up when launching.")]
    public float popHeight = 0.5f;
    [Tooltip("How fast the pop animation plays.")]
    public float popDuration = 0.2f;

    [Header("Optional Effects")]
    public AudioSource audioSource;
    public ParticleSystem particleEffect;

    private Vector3 initialPosition;
    private bool isAnimating = false;
    private float lastLaunchTime = -1f; // Cooldown timer
    private Collider padCollider;

    private void Start()
    {
        // Remember where the pad started so we can return to it
        initialPosition = transform.position;
        // Get the collider on this object so we can ignore it later
        padCollider = GetComponent<Collider>();
    }

    // This function runs automatically when another collider touches this object
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the object colliding is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Cooldown check
            if (Time.time < lastLaunchTime + 0.1f) return;

            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Pass the specific collider that hit the pad
                Launch(playerRb, collision.collider);
            }
        }
    }

    private void Launch(Rigidbody rb, Collider playerCollider)
    {
        lastLaunchTime = Time.time;

        // 3. Calculate Direction
        Vector3 direction = usePadRotation ? transform.up : Vector3.up;

        // --- STUCK FIX: IGNORE COLLISION ---
        // We tell the physics engine to ignore collisions between the player 
        // and the pad for a moment. This prevents friction/drag as the pad moves up.
        if (padCollider != null && playerCollider != null)
        {
            StartCoroutine(TemporarilyIgnoreCollision(padCollider, playerCollider));
        }
        
        // 4. Set Velocity Directly (Snappier than AddForce)
        // Instead of adding force, we override the velocity.
        // This guarantees the same launch height every time, regardless of gravity/falling speed.
        Vector3 currentVelocity = rb.linearVelocity;
        
        // Zero out velocity in the launch direction so we get a fresh launch
        if (usePadRotation)
        {
            // For angled pads, we set velocity directly
            rb.linearVelocity = direction * launchForce;
        }
        else
        {
            // For vertical pads, keep horizontal momentum, override vertical
            currentVelocity.y = launchForce;
            rb.linearVelocity = currentVelocity;
        }

        // 5. Effects
        if (audioSource != null) audioSource.Play();
        if (particleEffect != null) particleEffect.Play();

        // 6. Play Animation
        if (!isAnimating)
        {
            StartCoroutine(PopUpAnimation());
        }
    }

    private IEnumerator TemporarilyIgnoreCollision(Collider padCol, Collider playerCol)
    {
        // Disable collision so player can pass through the moving pad freely
        Physics.IgnoreCollision(padCol, playerCol, true);
        
        // Wait for ~0.5 seconds (plenty of time to clear the pad)
        yield return new WaitForSeconds(0.5f);
        
        // Re-enable collision so they can land on it again later
        // (Check if objects still exist to avoid errors if scene changed)
        if (padCol != null && playerCol != null)
        {
            Physics.IgnoreCollision(padCol, playerCol, false);
        }
    }

    private IEnumerator PopUpAnimation()
    {
        isAnimating = true;
        float elapsed = 0f;
        
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / popDuration;
            
            // Go up then down (Sine wave)
            float height = Mathf.Sin(percent * Mathf.PI) * popHeight;
            
            transform.position = initialPosition + (transform.up * height);
            
            yield return null;
        }

        transform.position = initialPosition;
        isAnimating = false;
    }
}