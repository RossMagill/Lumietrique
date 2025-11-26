using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPlaytest : MonoBehaviour
{
    [SerializeField] private InputActionReference resetAction;

    void OnEnable()
    {
        resetAction?.action.Enable();
    }

    void OnDisable()
    {
        resetAction?.action.Disable();
    }

    void Update()
    {
        if (resetAction != null && resetAction.action.WasPressedThisFrame())
        {
            LevelLoader.Instance.LoadLevelByName("Sandbox");
        }
    }
}
