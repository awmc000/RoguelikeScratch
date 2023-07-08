using UnityEngine;

public class ResolutionButton : MonoBehaviour
{
    public int width;
    public int height;

    /**
     * Sets the screen resolution to `width` and `height`.
     */
    public void SetResolution()
    {
        Screen.SetResolution(width, height, false);
    }

    /**
     * Toggles fullscreen.
     */
    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
