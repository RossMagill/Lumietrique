using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneResetManager : MonoBehaviour
{
    [Header("Global Inputs")]
    [SerializeField] private InputActionReference resetAction;

    void OnEnable()
    {
        resetAction?.action.Enable();
    }

    void OnDisable()
    {
        // This only runs when the SCENE closes, not when a robot dies.
        resetAction?.action.Disable();
    }

    void Update()
    {
        if (resetAction != null && resetAction.action.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}