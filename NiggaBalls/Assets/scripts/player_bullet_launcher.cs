using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_bullet_launcher : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera cam;
    [SerializeField] private float firerate = 1;
    [SerializeField] private bool fullauto = false;

    private float nextfiretime = 0;

    void Start(){
    }


    void Update(){

        if (fullauto && Input.GetMouseButton(0) && Time.time >= nextfiretime)
        {
            spawnbullet();
            nextfiretime = Time.time + 1/firerate;
        }
        else if (!fullauto && Input.GetMouseButtonDown(0)) {spawnbullet();}
    }

    void spawnbullet() {
        GameObject newbullet = Instantiate(bullet, transform.position, transform.rotation);
    }
}


