using UnityEngine;

public class CursorLock : MonoBehaviour
{
    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        // Press Escape to release the mouse.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Left-click to lock it again.
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}