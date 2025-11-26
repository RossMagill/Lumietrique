using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Static variable to hold the one true instance of the MusicPlayer
    public static MusicPlayer Instance;

    private void Awake()
    {
        // If an instance already exists...
        if (Instance != null && Instance != this)
        {
            // ...destroy THIS new object immediately. 
            // This prevents the music from restarting or playing twice.
            Destroy(gameObject);
            return;
        }

        // If no instance exists (first time launching), this is it!
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}