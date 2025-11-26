using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        if (other.CompareTag("Player"))
        {
            RunnerMovement runner = other.GetComponent<RunnerMovement>();

            if (runner == null || !runner.IsDashing)
            {
                Debug.Log("ZAPPED! You must dash through lasers!");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("Passed through safely via Phase Shift/Dash.");
            }
        }
    }
}