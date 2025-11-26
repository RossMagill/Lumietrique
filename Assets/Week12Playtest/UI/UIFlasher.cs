using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFlasher : MonoBehaviour
{
    [Header("Flash Settings")]
    public float flashSpeed = 1.5f;  
    public float minAlpha = 0.3f;     
    public float maxAlpha = 1.0f;     

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * flashSpeed, 1.0f);

        t = Mathf.SmoothStep(0f, 1f, t);

        canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
    }
}