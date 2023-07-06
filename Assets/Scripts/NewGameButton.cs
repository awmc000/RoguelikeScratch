using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    /**
     * Loads the game itself, closing the main menu.
     */
    public void OnPress()
    {
        SceneManager.LoadScene("SampleScene");
    }

    /**
     * Closes the game.
     */
    public void OnQuit()
    {
        Application.Quit();
    }

    public bool settingsOpen = true;

    /**
     * Toggles whether the full screen and set resolution buttons are visible.
     */
    public void OnToggleSettings()
    {
        settingsOpen = !settingsOpen;
        GameObject resolutionButtons = GameObject.Find("ResolutionButtons");
        if (settingsOpen)
        {
            resolutionButtons.transform.position -= new Vector3(1000, 1000, 0);
        }
        else
        {
            resolutionButtons.transform.position += new Vector3(1000, 1000, 0);
        }
    }
}
