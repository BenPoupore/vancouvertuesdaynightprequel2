using UnityEngine;
using UnityEngine.SceneManagement;

// Attach this to an empty GameObject in your scene (e.g. "GameManager" or "DeathScreenManager").
// Shows a death screen UI panel, pauses the game, and can restart the current level.
//
// Wiring it up (no code changes needed elsewhere):
//   1. Drag this script's ShowDeathScreen() into the Player's Health component's
//      "On Death ()" UnityEvent list in the Inspector - Health already calls that event
//      whenever the player dies, so this just needs to be plugged in.
//   2. Hook your UI Restart button's OnClick() to this script's RestartLevel().

public class DeathScreenManager : MonoBehaviour
{
    [Header("References")]
    public GameObject deathScreenPanel; // the UI panel (background + "You Died" text + button), inactive by default

    [Header("Options")]
    public bool pauseGameOnDeath = true;
    public bool unlockCursorOnDeath = true;

    void Awake()
    {
        // Make sure the death screen starts hidden
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(false);
        }
    }

    // Call this when the player dies (wire it to Health's "On Death ()" event in the Inspector)
    public void ShowDeathScreen()
    {
        if (deathScreenPanel != null)
        {
            deathScreenPanel.SetActive(true);
        }

        if (pauseGameOnDeath)
        {
            Time.timeScale = 0f;
        }

        if (unlockCursorOnDeath)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Wire this to your Restart button's OnClick() in the Inspector
    public void RestartLevel()
    {
        Time.timeScale = 1f; // always reset this before loading, or the new scene loads still paused
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Optional: wire this to a "Quit" button if you add one
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
