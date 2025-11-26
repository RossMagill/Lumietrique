using UnityEngine;

[RequireComponent(typeof(UIFlasher))] 
public class TutorialPromptController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Only show this prompt if the stack size is exactly this number (1 = solo).")]
    [SerializeField] private int triggerStackCount = 1;
    
    [Tooltip("How close to a target before showing?")]
    [SerializeField] private float detectionRadius = 5.0f;

    // No more PlayerPrefs key needed!

    private UIFlasher flasher;
    private CanvasGroup canvasGroup;
    private bool hasShown = false; // Resets to false every time scene loads
    private NewStackController playerController;
    private float timeSinceLastValid = 0f;

    void Awake()
    {
        flasher = GetComponent<UIFlasher>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Start invisible
        SetVisible(false);
    }

    void Update()
    {
        // If we already showed AND completed it this session, stop.
        if (hasShown) return;

        if (playerController == null)
        {
            FindPlayer();
            return;
        }

        // Safety check for initialization
        if (playerController.CurrentStackCount == 0) return;

        bool conditionsMet = CheckConditions();

        if (conditionsMet) timeSinceLastValid = Time.time;
        bool show = (Time.time - timeSinceLastValid) < 0.2f;

        SetVisible(show);

        // If player succeeds (stack grows), mark complete for this session
        if (playerController.CurrentStackCount > triggerStackCount) 
        {
            CompleteTutorial();
        }
    }

    private void SetVisible(bool visible)
    {
        if (flasher != null) flasher.enabled = visible;

        if (!visible)
        {
            canvasGroup.alpha = 0f;
        }
        
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }

    private void FindPlayer()
    {
        var allControllers = FindObjectsByType<NewStackController>(FindObjectsSortMode.None);
        foreach (var controller in allControllers)
        {
            if (controller.enabled && controller.gameObject.activeInHierarchy) 
            {
                playerController = controller;
                break;
            }
        }
    }

    private bool CheckConditions()
    {
        if (playerController.CurrentStackCount != triggerStackCount) return false;

        Collider[] hits = Physics.OverlapSphere(playerController.transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            NewStackController target = hit.GetComponentInParent<NewStackController>();
            if (target != null && target != playerController)
            {
                return true;
            }
        }
        return false;
    }

    private void CompleteTutorial()
    {
        hasShown = true; // Mark done for this session
        gameObject.SetActive(false); // Hide until scene reload
    }
    
    void OnDrawGizmosSelected()
    {
        if (playerController != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerController.transform.position, detectionRadius);
        }
    }
}