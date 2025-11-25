using UnityEngine;

public enum RobotType
{
    Runner,
    Jumper,
    Flyer,
    None
}

public class PlayerUIManager : MonoBehaviour
{
    [Header("Control Panels (Movement)")]
    [SerializeField] private GameObject runnerUI;
    [SerializeField] private GameObject jumperUI;
    [SerializeField] private GameObject flyerUI;

    [Header("Action Prompts")]
    [Tooltip("The UI element that says 'Press E to Stack'")]
    [SerializeField] private GameObject stackActionPrompt;
    
    [Tooltip("The UI element that says 'Press E to Unstack'")]
    [SerializeField] private GameObject unstackActionPrompt;

    public static PlayerUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowMovementUI(RobotType type)
    {
        if (runnerUI) runnerUI.SetActive(false);
        if (jumperUI) jumperUI.SetActive(false);
        if (flyerUI) flyerUI.SetActive(false);

        switch (type)
        {
            case RobotType.Runner: if (runnerUI) runnerUI.SetActive(true); break;
            case RobotType.Jumper: if (jumperUI) jumperUI.SetActive(true); break;
            case RobotType.Flyer: if (flyerUI) flyerUI.SetActive(true); break;
        }
    }

    // --- NEW METHOD ---
    public void UpdateActionUI(bool canStack, bool canUnstack)
    {
        // 1. Reset
        if (stackActionPrompt) stackActionPrompt.SetActive(false);
        if (unstackActionPrompt) unstackActionPrompt.SetActive(false);

        // 2. Prioritize Stacking over Unstacking
        if (canStack)
        {
            if (stackActionPrompt) stackActionPrompt.SetActive(true);
        }
        else if (canUnstack)
        {
            if (unstackActionPrompt) unstackActionPrompt.SetActive(true);
        }
    }
}