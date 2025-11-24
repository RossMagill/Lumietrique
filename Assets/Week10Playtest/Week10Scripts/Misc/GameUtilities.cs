using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameUtilities : MonoBehaviour
{
    [Header("Slice Lock")]
    public float sliceZ = 0f;

    [Header("Input")]
    [SerializeField] private InputActionReference resetAction;

    private Rigidbody rb;  

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        resetAction?.action.Enable();
    }

    void OnDisable()
    {
        resetAction?.action.Disable();
    }
    
    void FixedUpdate()
    {
        var p = rb.position;
        if (Mathf.Abs(p.z - sliceZ) > 0.0001f)
            rb.position = new Vector3(p.x, p.y, sliceZ);
    }

    void Update()
    {
        if (resetAction != null && resetAction.action.WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
