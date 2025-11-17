using UnityEngine;

public abstract class UI_AbstractCloseable : MonoBehaviour
{
    protected Canvas canvas;

    protected void BaseStart(bool startOpen = false)
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = startOpen;

        if (canvas == null)
            Debug.LogError("Could not find Canvas: " + this.name);
    }

    public bool IsOpen()
    {
        return canvas != null && canvas.enabled;
    }

    public void Open()
    {
        if (canvas != null)
            canvas.enabled = true;
    }

    public void Close()
    {
        if (canvas != null)
            canvas.enabled = false;
    }
}