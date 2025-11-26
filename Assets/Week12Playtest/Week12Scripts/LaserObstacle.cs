using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserObstacle : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip zapSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        if (other.CompareTag("Player"))
        {
            RunnerMovement runner = other.GetComponent<RunnerMovement>();

            if (runner == null || !runner.IsDashing)
            {
                Debug.Log("ZAPPED! You must dash through lasers!");

                if (zapSound != null)
                {
                    AudioSource.PlayClipAtPoint(zapSound, transform.position, soundVolume);
                }

                LevelLoader.Instance.ReloadLevel();
            }
            else
            {
                Debug.Log("Passed through safely via Phase Shift/Dash.");
            }
        }
    }
}