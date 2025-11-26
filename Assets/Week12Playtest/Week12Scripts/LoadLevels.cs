using UnityEngine;
using UnityEngine.InputSystem;

public class LoadLevels : MonoBehaviour
{
    [SerializeField] private InputActionReference loadAction;

    void OnEnable()
    {
        loadAction?.action.Enable();
    }

    void OnDisable()
    {
        loadAction?.action.Disable();
    }

    void Update()
    {
        if (loadAction != null && loadAction.action.WasPressedThisFrame())
        {
            LevelLoader.Instance.LoadLevelByName("Level1");
        }
    }
}
