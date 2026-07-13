using UnityEngine;

// Attach this to the Quad (or any flat sprite object) that displays your enemy's 2D image.
// Makes it always rotate to face the camera - the classic "billboard" trick DOOM used
// to make 2D sprites look like they're part of the 3D world from any angle.

public class Billboard : MonoBehaviour
{
    [Tooltip("True = classic DOOM style, only rotates left/right, stays upright. False = fully faces camera on all axes (can tilt up/down too).")]
    public bool lockUpright = true;

    private Transform cam;

    void Start()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main != null) cam = Camera.main.transform;
            else return;
        }

        if (lockUpright)
        {
            // Only rotate around the vertical axis, so sprites never tilt up/down -
            // this is exactly how DOOM/Duke Nukem 3D enemies behaved.
            Vector3 direction = cam.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            // Full billboard - always exactly matches the camera's facing, including pitch
            transform.rotation = cam.rotation;
        }
    }
}
