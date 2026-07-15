using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_2_behaviour : MonoBehaviour
{

    public int health = 100;
    public float move_speed = 1;
    public float rotate_speed = 10;
    public float attack_radius = 30;

    public GameObject player;

    void Start()
    {

    }

    void Update()
    {
        Vector3 diatance = player.transform.position - transform.position;

       // Debug.Log(diatance.magnitude);
        //;if (diatance.magnitude > attack_radius) { bullet_launcher.SetActive(false); }
        //else { bullet_launcher.SetActive(true); }

        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotate_speed * Time.deltaTime);

        transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up) * move_speed / 100;

        if (health == 0) { Destroy(gameObject); }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("projectile"))
        {
            health -= 20;
        }
    }
}
