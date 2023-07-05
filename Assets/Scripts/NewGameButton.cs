using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnPress()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public bool settingsOpen = true;

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
