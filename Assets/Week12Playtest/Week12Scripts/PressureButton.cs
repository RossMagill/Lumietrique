using UnityEngine;
using System; // Needed for Actions

public class PressureButton : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform buttonMesh;
    [SerializeField] private float pressDepth = 0.2f;
    [SerializeField] private Material pressedMaterial;
    [SerializeField] private Material defaultMaterial;

    // Public read-only property for the Manager to check
    public bool IsPressed { get; private set; } = false;

    // Event that Manager can listen to
    public event Action OnStateChanged;

    private Vector3 upPos;
    private Vector3 downPos;
    private int objectsOnButton = 0;
    private Renderer meshRenderer;

    void Start()
    {
        if (buttonMesh)
        {
            upPos = buttonMesh.localPosition;
            downPos = upPos - new Vector3(0, pressDepth, 0);
            meshRenderer = buttonMesh.GetComponent<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add your tag checks here (Player, NewStackController, etc)
        if (other.CompareTag("Player") || other.GetComponentInParent<NewStackController>())
        {
            if (objectsOnButton == 0)
            {
                SetState(true);
            }
            objectsOnButton++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<NewStackController>())
        {
            objectsOnButton--;
            if (objectsOnButton <= 0)
            {
                objectsOnButton = 0;
                SetState(false);
            }
        }
    }

    private void SetState(bool pressed)
    {
        IsPressed = pressed;

        // Visuals
        if (buttonMesh) buttonMesh.localPosition = pressed ? downPos : upPos;
        if (meshRenderer) meshRenderer.material = pressed ? pressedMaterial : defaultMaterial;

        // Notify the Manager!
        OnStateChanged?.Invoke();
    }
}