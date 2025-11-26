using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("Settings")]
    public Animator transitionAnimator;
    public float transitionTime = 1f;

    private const string TRANSITION_TRIGGER = "Start";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        // Loop back to start if at the end
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            nextSceneIndex = 0;

        StartCoroutine(LoadLevelSequence(nextSceneIndex));
    }

    public void LoadLevelByName(string levelName)
    {
        StartCoroutine(LoadLevelSequence(levelName));
    }

    // NEW: Call this to restart the current scene
    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadLevelSequence(currentSceneIndex));
    }

    IEnumerator LoadLevelSequence(object levelIdentifier)
    {
        // 1. Play Fade Out animation
        transitionAnimator.SetTrigger(TRANSITION_TRIGGER);

        // 2. Wait
        yield return new WaitForSeconds(transitionTime);

        // 3. Load Scene
        if (levelIdentifier is int index)
        {
            SceneManager.LoadScene(index);
        }
        else if (levelIdentifier is string name)
        {
            SceneManager.LoadScene(name);
        }
        
        // 4. Force the Fade In animation (Fixes "stuck on black" issue)
        transitionAnimator.Play("Crossfade_End");
    }
}