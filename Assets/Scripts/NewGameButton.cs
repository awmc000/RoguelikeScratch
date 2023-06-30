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
}
