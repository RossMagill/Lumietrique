using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    
    // Reference the PhysicsPlatform script (on the parent) so we can get its velocity
    [SerializeField] PhysicsPlatform platformScript; 

    private void OnTriggerEnter(Collider other)
    {
        // Ignore the player's interaction triggers, only grab their feet (Physics collider)
        if (other.isTrigger) return;

        if (other.CompareTag(playerTag))
        {
            // Check for Runner
            // RunnerMovement runner = other.GetComponent<RunnerMovement>();
            // if (runner != null)
            // {
            //     StartCoroutine(SyncRunnerVelocity(runner));
            //     return;
            // }

            // Check for Jumper
            JumperMovement jumper = other.GetComponent<JumperMovement>();
            if (jumper != null)
            {
                StartCoroutine(SyncJumperVelocity(jumper));
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag(playerTag))
        {
            StopAllCoroutines();

            // Reset Runner
            // RunnerMovement runner = other.GetComponent<RunnerMovement>();
            // if (runner != null) runner.platformVelocity = Vector3.zero;

            // Reset Jumper
            JumperMovement jumper = other.GetComponent<JumperMovement>();
            if (jumper != null) jumper.platformVelocity = Vector3.zero;
        }
    }

    // --- SYNC COROUTINES ---

    // System.Collections.IEnumerator SyncRunnerVelocity(RunnerMovement player)
    // {
    //     while (player != null)
    //     {
    //         player.platformVelocity = platformScript.CurrentVelocity;
    //         yield return new WaitForFixedUpdate();
    //     }
    // }

    System.Collections.IEnumerator SyncJumperVelocity(JumperMovement player)
    {
        while (player != null)
        {
            player.platformVelocity = platformScript.CurrentVelocity;
            yield return new WaitForFixedUpdate();
        }
    }
}