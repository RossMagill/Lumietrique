using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, this will load the next scene in the Build Settings list.")]
    public bool loadNextLevel = false;

    [Tooltip("If Load Next Level is false, type the exact name of the scene you want to load here.")]
    public string sceneName;

    [Header("Trigger Options")]
    [Tooltip("The Tag of the object that can trigger the scene change (usually 'Player').")]
    public string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Ignore other triggers to prevent weird physics interactions
        if (other.isTrigger) return;

        // Check for specific component OR specific tag
        // (You can add your NewStackController check back here if needed)
        if (other.CompareTag(targetTag) || other.GetComponent("NewStackController")) 
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (loadNextLevel)
        {
            // 1. Load the next scene in the Build Settings list
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            // Check if the next scene exists to avoid errors
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No more scenes in Build Settings! Loading Main Menu (Index 0) or looping.");
                // Optional: Loop back to start
                // SceneManager.LoadScene(0); 
            }
        }
        else
        {
            // 2. Load a specific scene by name
            if (!string.IsNullOrEmpty(sceneName))
            {
                LevelLoader.Instance.LoadLevelByName(sceneName);
            }
            else
            {
                Debug.LogError("Scene Name is empty! Please type a scene name in the Inspector.");
            }
        }
    }
}