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

    void Start()
    {
        center_on_player();
    }

    void Update()
    {
        player_cam_mode();
    }

    private void center_on_player()
    {
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
    }

    private void player_cam_mode()
    {
        float rotX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
        rotY += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        rotY = Mathf.Clamp(rotY, -89.5f, 89.5f);
        transform.localEulerAngles = new Vector3(-rotY, rotX, 0.0f);
        transform.position = player.transform.position;
    }
}
