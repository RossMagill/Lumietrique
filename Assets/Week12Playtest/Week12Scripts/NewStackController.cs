using UnityEngine;
using System.Collections.Generic;

public class NewStackController : MonoBehaviour, IControllable
{
    [Header("Stack Base")]
    [SerializeField]
    private List<GameObject> stack = new();

    [Header("Player Control Indicator")]
    [SerializeField]
    private GameObject thunder;

    [Header("Visual Effects")]
    [SerializeField] private GameObject stackConnectVFX; 
    [SerializeField] private GameObject stackPopVFX;

    private GameObject stackBasePrefab;

    private BoxCollider physicsCollider;
    private BoxCollider triggerCollider;
    private const float TRIGGER_PADDING = 1.5f;
    private List<NewStackController> potentialTargets = new();
    private PlayerFocusManager playerFocusManager;
    private Rigidbody rb; 
    private FlyerMovement flyerMovement;
    private JumperMovement jumperMovement;
    private RunnerMovement runnerMovement;

    // ---------------------- Init Methods ----------------------

    private void Awake()
    {
        stackBasePrefab = Resources.Load<GameObject>("StackBase");
        if (stackBasePrefab == null)
        {
            Debug.LogError("StackBasePrefab not found");
        }

        BoxCollider[] colliders = GetComponents<BoxCollider>();

        foreach (BoxCollider collider in colliders)
        {
            if (collider.isTrigger)
            {
                triggerCollider = collider;
            }
            else
            {
                physicsCollider = collider;
            }
        }

        playerFocusManager = FindAnyObjectByType<PlayerFocusManager>();
        rb = GetComponent<Rigidbody>();
        flyerMovement = GetComponent<FlyerMovement>();
        jumperMovement = GetComponent<JumperMovement>();
        runnerMovement = GetComponent<RunnerMovement>();

        if (thunder != null) thunder.SetActive(false);
    }

    void Start()
    {
        if (stack.Count > 0)
        {
            rb.useGravity = true;
            CalculateCollider();
            CalculateTriggerCollider();
            SetThunderPosition();
        }
    }

    // ---------------------- Collider Logic ----------------------

    private void CalculateCollider()
    {
        float height = GetStackHeight();
        float offset = stack[0].transform.localScale.y / 2f;
        physicsCollider.center = new Vector3(0f, (height / 2f) - offset, 0);
        physicsCollider.size = new Vector3(2f, height, 0.2f);
    }

    private void CalculateTriggerCollider()
    {
        float height = GetStackHeight();
        float width = 2f;

        float offset = stack[0].transform.localScale.y / 2f;
        triggerCollider.center = new Vector3(0f, (height / 2f) - offset, 0);

        float paddedHeight = height + (TRIGGER_PADDING * 2f);
        float paddedWidth = width + (TRIGGER_PADDING * 2f);

        triggerCollider.size = new Vector3(paddedWidth, paddedHeight, 0.2f);
    }

    private float GetStackHeight()
    {
            float height = 0;
            foreach (GameObject robot in stack)
            {
                height += (float)robot.transform.localScale.y;
            }
            return height;
    }

    void OnTriggerEnter(Collider other)
    {
        NewStackController targetStack = other.GetComponentInParent<NewStackController>();

        if (targetStack != null && targetStack != this)
        {
            if (!potentialTargets.Contains(targetStack))
            {
                potentialTargets.Add(targetStack);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        NewStackController targetStack = other.GetComponentInParent<NewStackController>();

        if (targetStack != null && potentialTargets.Contains(targetStack))
        {
            potentialTargets.Remove(targetStack);
        }
    }

    // ---------------------- Stack Management Methods ----------------------

    public void TryRejoinOrPop()
    {
        potentialTargets.RemoveAll(target => target == null || target.gameObject == null);

        if (potentialTargets.Count > 0)
        {
            NewStackController targetController = potentialTargets[0];

            if (targetController != null && targetController.gameObject != null)
            {
                RejoinTargetStack(targetController.gameObject);
                return;
            }
        }
        else
        {
            PopStack();
        }
    }

    public void PopStack()
    {
        if (stack.Count > 1)
        {
            GameObject topRobot = stack[^1];
            float halfHeight = topRobot.transform.localScale.y / 2f;
            Vector3 feetPosition = topRobot.transform.position - (topRobot.transform.up * halfHeight);
            
            SpawnVFX(stackPopVFX, feetPosition, this.transform);
            GameObject newStackBase = Instantiate(stackBasePrefab, topRobot.transform.position, Quaternion.identity);
            stack.RemoveAt(stack.Count - 1);
            CalculateCollider();
            CalculateTriggerCollider();
            SetThunderPosition();
            NewStackController newStackController = newStackBase.GetComponent<NewStackController>();
            newStackController.RegisterRobotToStack(topRobot);
            playerFocusManager.RegisterControllable(newStackBase);
        }
    }

    public void RejoinTargetStack(GameObject targetStack)    
    {
        NewStackController targetStackController = targetStack.GetComponent<NewStackController>();

        List<GameObject> modelsToTransfer = new List<GameObject>(this.stack);

        foreach (GameObject robotModel in modelsToTransfer)
        {
            targetStackController.RegisterRobotToStack(robotModel); 
        }

        //targetStackController.SetThunderPosition();

        this.stack.Clear();
        
        playerFocusManager.DeregisterControllable(this.gameObject, targetStack);

        Destroy(this.gameObject);
    }

    public void RegisterRobotToStack(GameObject robot)
    {
        // float baseHeight = GetStackHeight();
        //float robotHeight = robot.transform.localScale.y;

        robot.transform.SetParent(this.transform);
        
        if (stack.Count == 0)
        {
            robot.transform.localPosition = Vector3.zero;        
        } else
        {
            GameObject previousRobot = stack[stack.Count - 1];

            float prevY = previousRobot.transform.localPosition.y;
            float prevHalfHeight = previousRobot.transform.localScale.y / 2f;
            float newHalfHeight = robot.transform.localScale.y / 2f;

            robot.transform.localPosition = new Vector3(0, prevY + prevHalfHeight + newHalfHeight, 0);

            Vector3 contactPoint = robot.transform.position - (robot.transform.up * newHalfHeight);
            SpawnVFX(stackConnectVFX, contactPoint, this.transform);
        }

        stack.Add(robot);
        CalculateCollider();
        CalculateTriggerCollider();
        SetThunderPosition();
    }

    // ---------------------- SFX, VFX, and Player Indicator ----------------------

    private void SetThunderPosition()
    {
        Debug.Log("Setting Thunder");
        float height = GetStackHeight();
        Debug.Log(height);
        thunder.transform.localPosition = new Vector3(0, height + 0.5f, 0);
        Debug.Log(thunder.transform.localPosition);
        #if UNITY_EDITOR
        UnityEditor.EditorGUIUtility.PingObject(thunder);
#endif
        Debug.Log($"Moving Thunder named '{thunder.name}' on Parent '{transform.name}'");
    }

    private void SpawnVFX(GameObject prefab, Vector3 position, Transform parent)
    {
        if (prefab == null) return;

        GameObject vfxInstance = Instantiate(prefab, position, Quaternion.identity, parent);

        Destroy(vfxInstance, 0.5f);
    }

    // ---------------------- IControllable Implementation ----------------------

    void IControllable.ActivateControl()
    {
        jumperMovement.enabled = false;
        flyerMovement.enabled = false;
        runnerMovement.enabled = false;

        if (jumperMovement != null && stack[0].name.Contains("Jumper"))
        {
            jumperMovement.enabled = true;
        } 
        else if (flyerMovement != null && stack[0].name.Contains("Flyer"))
        {
            flyerMovement.enabled = true;
        } 
        else if (runnerMovement != null && stack[0].name.Contains("Runner"))
        {
            runnerMovement.enabled = true;
        }

        SetThunderPosition();
        if(thunder != null) thunder.SetActive(true);
    }

    void IControllable.DeactivateControl()
    {
        jumperMovement.enabled = false;
        flyerMovement.enabled = false;
        runnerMovement.enabled = false;
        if(thunder != null) thunder.SetActive(false);
    } 
}
