using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private float speed;
    [SerializeField] private float mouseSensitivityX = 5.0f;
    [SerializeField] private float mouseSensitivityY = 5.0f;
    [SerializeField] private float jumpieness = 1f;

    private float rotY = 0.0f;
    private bool can_jump = true;

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

        transform.position += trans*speed/100;

        if (Input.GetKeyDown(KeyCode.Space) && can_jump) { 
            RB.AddForce(Vector3.up * jumpieness, ForceMode.Impulse);
            can_jump = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            can_jump = true;
        }
    }

}
