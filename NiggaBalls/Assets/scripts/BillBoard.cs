using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Billboard Settings")]
    [SerializeField] private bool keepUpright = true;
    [SerializeField] private bool flip180 = false;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            return;
        }

        Vector3 directionToCamera =
            targetCamera.transform.position - transform.position;

        // Prevent the picture from leaning forward or backward.
        if (keepUpright)
        {
            directionToCamera.y = 0f;
        }

        if (directionToCamera.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(
            directionToCamera,
            Vector3.up
        );

        if (flip180)
        {
            targetRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        transform.rotation = targetRotation;
    }
}