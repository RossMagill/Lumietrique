using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Callbacks;

public class NewStackController : MonoBehaviour, IControllable
{
    [Header("Stack Base")]
    [SerializeField]
    private List<GameObject> stack = new();

    private GameObject stackBasePrefab;

    private BoxCollider box;
    private IMovement playerMovement;
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
        box = GetComponent<BoxCollider>();
        playerMovement = GetComponent<IMovement>();
        playerFocusManager = FindAnyObjectByType<PlayerFocusManager>();
        rb = GetComponent<Rigidbody>();
        flyerMovement = GetComponent<FlyerMovement>();
        jumperMovement = GetComponent<JumperMovement>();
        runnerMovement = GetComponent<RunnerMovement>();
    }

    void Start()
    {
        if (stack.Count > 0)
        {
            rb.useGravity = true;
            CalculateCollider();
            // ActivateMovementScript();
        }
    }

    // ---------------------- Collider Logic ----------------------

    private void CalculateCollider()
    {
        float height = GetStackHeight();
        float offset = stack[0].transform.localScale.y / 2f;
        box.center = new Vector3(0f, (height / 2f) - offset, 0);
        box.size = new Vector3(2f, height, 0.2f);
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

    // ---------------------- Stack Management Methods ----------------------

    public void HandleStackInputLogic()
    {
        // Search for stack, if none, pop off current
        PopStack();
    }

    public void PopStack()
    {
        if (stack.Count > 1)
        {
            GameObject topRobot = stack[^1];
            GameObject newStackBase = Instantiate(stackBasePrefab, topRobot.transform.position, Quaternion.identity);
            stack.RemoveAt(stack.Count - 1);
            CalculateCollider();
            NewStackController newStackController = newStackBase.GetComponent<NewStackController>();
            newStackController.RegisterToStack(topRobot);
            playerFocusManager.RegisterControllable(newStackBase);
        }
    }

    public void RegisterToStack(GameObject robot)
    {
        robot.transform.SetParent(this.transform);
        
        if (stack.Count == 0)
        {
            robot.transform.localPosition = Vector3.zero;        
        } 
        else
        {
            // Logic for positioning robot on top of stack
        }

        stack.Add(robot);
        CalculateCollider();
    }

    // ---------------------- IControllable Implementation ----------------------

    void IControllable.ActivateControl()
    {
        // playerMovement.EnableMovement();
        if (stack[0].name == "Jumper")
        {
            jumperMovement.enabled = true;
        } else if (stack[0].name == "Flyer")
        {
            flyerMovement.enabled = true;
        } else if (stack[0].name == "Runner")
        {
            runnerMovement.enabled = true;
        }
    }

    void IControllable.DeactivateControl()
    {
        // playerMovement.DisableMovement();
        jumperMovement.enabled = false;
        flyerMovement.enabled = false;
        runnerMovement.enabled = false;
    } 
}
