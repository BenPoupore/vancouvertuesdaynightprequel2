using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_camera : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;

    private float rotY = 0.0f;

    void Update()
    {

        player.transform.position = transform.position;

        float rotX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
        rotY += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        rotY = Mathf.Clamp(rotY, -89.5f, 89.5f);
        transform.localEulerAngles = new Vector3(-rotY, rotX, 0.0f);

        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        // move forwards/backwards

        Vector3 trans = new Vector3(-strafe * speed * Time.deltaTime, 0.0f, -forward * speed * Time.deltaTime);
        transform.position += gameObject.transform.localRotation * trans;
    }
}
