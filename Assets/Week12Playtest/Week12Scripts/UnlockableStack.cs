using UnityEngine;

[RequireComponent(typeof(NewStackController))]
public class UnlockableStack : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How close does the player need to get to unlock this robot?")]
    [SerializeField] private float unlockRadius = 5.0f;
    
    [Tooltip("Optional: Play a sound/particle when unlocked")]
    [SerializeField] private GameObject unlockVFX;

    private bool isUnlocked = false;
    private NewStackController myController;
    private PlayerFocusManager focusManager;

    void Awake()
    {
        myController = GetComponent<NewStackController>();
        focusManager = FindAnyObjectByType<PlayerFocusManager>();
    }

    void Start()
    {
        // IMPORTANT: Ensure this robot is NOT in the Focus Manager's list at the start.
        // You can either remove it manually in the Inspector, or code it here.
        // For safety, let's just assume you removed it from the inspector list.
    }

    void Update()
    {
        if (isUnlocked) return;

        // Check for player proximity
        Collider[] hits = Physics.OverlapSphere(transform.position, unlockRadius);
        foreach (var hit in hits)
        {
            // We look for an active stack controller that IS the player
            NewStackController potentialPlayer = hit.GetComponentInParent<NewStackController>();
            
            // Check if it's the player (you might check tag, or if it's currently controllable)
            // A simple check is: Is it a stack controller, is it NOT me, and is it enabled?
            if (potentialPlayer != null && potentialPlayer != myController && potentialPlayer.enabled)
            {
                Unlock();
                break;
            }
        }
    }

    private void Unlock()
    {
        isUnlocked = true;
        
        // Register with the manager so we can switch to it!
        if (focusManager != null)
        {
            focusManager.RegisterControllable(this.gameObject);
            
            // Optional: Switch focus immediately?
            // focusManager.SetFocus(this.gameObject); 
        }

        // Visual Feedback
        if (unlockVFX != null) Instantiate(unlockVFX, transform.position, Quaternion.identity);
        
        // Disable this script so we don't waste CPU
        this.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, unlockRadius);
    }
}