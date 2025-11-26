using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip deathSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        
        if (other.CompareTag("Player"))
        {
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);
            }

            LevelLoader.Instance.ReloadLevel();
            Debug.Log("Player Died");
        }
    }
}
