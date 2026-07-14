using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_bullet_launcher : MonoBehaviour
{
    public GameObject bullet;
    public Camera cam;
    public float bullet_speed = 1;

    void Start()
    {
    }

    void Update()
    {
    if (Input.GetMouseButtonDown(0)) {
            GameObject newbullet =  Instantiate(bullet, transform.position, transform.rotation);
        }
    }
}


