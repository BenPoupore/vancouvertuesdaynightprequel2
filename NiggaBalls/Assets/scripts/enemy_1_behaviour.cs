using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_1_behaviour : MonoBehaviour
{

    public int health = 100;
    public float move_speed = 1;
    public float rotate_speed = 10;
    public float attack_radius = 30;
    public float strafe_radius = 20;
    public float public_strafe_speed = 1;
    public float public_charge_speed = 1;

    public GameObject player;
    public GameObject hitboxbody;
    public GameObject hitboxhead;
    public GameObject bullet_launcher;

    private float private_strafe_speed = 1;
    private float charge_speed = 1;

    void Start()
    {

    }

    void Update()
    {
        Vector3 diatance = player.transform.position - transform.position;

//Debug.Log(diatance.magnitude);
        if (diatance.magnitude > attack_radius){bullet_launcher.SetActive(false);}
        else { bullet_launcher.SetActive(true); }
        Debug.Log(diatance.magnitude);
        if (diatance.magnitude < strafe_radius)
        {
            private_strafe_speed = Mathf.PerlinNoise(Time.time, transform.position.y);
            private_strafe_speed -= 0.5f;
            charge_speed = 0;
        }
        else { 

            charge_speed = 1;
            private_strafe_speed = 0;
        }

        Debug.Log((private_strafe_speed-0.5f));

        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotate_speed * Time.deltaTime);

        transform.position += (transform.forward * charge_speed * public_charge_speed/10 + transform.right * public_strafe_speed * private_strafe_speed)  *move_speed;

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
