using UnityEngine;

public class ResolutionButton : MonoBehaviour
{
    public int width;
    public int height;

    public void SetResolution()
    {
        Screen.SetResolution(width, height, false);
    }
}
