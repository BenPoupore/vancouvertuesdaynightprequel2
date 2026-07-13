using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour
{

        public GameObject player;
    public float speed;
    public float mouseSensitivityX = 5.0f;
    public float mouseSensitivityY = 5.0f;

    private float rotY = 0.0f;

    public Camera cam;
    public Rigidbody RB;

    void Start()
    {

    }

    void Update()
    {

        Vector3 trans = new Vector3(0, 0, 0);
        // player look
        float rotX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
        rotY += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        rotY = Mathf.Clamp(rotY, -89.5f, 89.5f);
        transform.localEulerAngles = new Vector3(-rotY, rotX, 0.0f);

        //Stransform.position += gameObject.transform.localRotation * trans;

        // player translation

        if (Input.GetKey(KeyCode.W)) { trans += Vector3.ProjectOnPlane(transform.forward, Vector3.up); }
        if (Input.GetKey(KeyCode.A)) { trans += -transform.right; }
        if (Input.GetKey(KeyCode.S)) { trans += -Vector3.ProjectOnPlane(transform.forward, Vector3.up); }
        if (Input.GetKey(KeyCode.D)) { trans += transform.right; }
        trans.Normalize();

        gameObject.transform.position += trans*speed/100;
    }
}
