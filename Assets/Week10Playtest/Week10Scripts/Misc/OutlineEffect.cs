using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    // Assign your actual Outline Component or Material/Renderer here
    [SerializeField]
    private Renderer targetRenderer; // e.g., the Mesh Renderer of the robot/stack

    [SerializeField]
    private Material outlineMaterial; // Assign a material that looks like an outline

    private Material originalMaterial;

    void Awake()
    {
        // Store the original material
        if (targetRenderer != null)
        {
            originalMaterial = targetRenderer.material;
        }
    }

    public void EnableOutline()
    {
        if (targetRenderer != null && outlineMaterial != null)
        {
            targetRenderer.material = outlineMaterial;
        }
    }

    public void DisableOutline()
    {
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }
    }
}