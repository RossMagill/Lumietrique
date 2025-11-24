using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GameUtilities : MonoBehaviour
{
    [Header("Slice Lock")]
    public float sliceZ = 0f;

    private Rigidbody rb;  

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        // Purely physics. No Input. Safe to destroy.
        if (Mathf.Abs(rb.position.z - sliceZ) > 0.0001f)
        {
            rb.position = new Vector3(rb.position.x, rb.position.y, sliceZ);
        }
    }
}