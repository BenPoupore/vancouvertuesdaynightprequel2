using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour{

    public Rigidbody RB;
    public float speed;
    private float velx;
    private float vely;

    void Start(){
        
    }

    void Update(){

        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        RB.linearVelocity = new Vector2(velx, vely).normalized * speed;
    }
}
