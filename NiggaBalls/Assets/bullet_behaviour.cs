using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_behaviour : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 1;
    public float despawn_radius = 50;
    public float precision = 0;
    public bool enable_precision = false;
  

    private Vector3 initialposition;
    private float distance_from_spawn;

void Start()
    {
        initialposition =  transform.position;

        if (enable_precision) { transform.localRotation = Quaternion.Euler(Random.Range(-precision, precision), Random.Range(-precision, precision), Random.Range(-precision, precision)); }
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        distance_from_spawn = Vector3.Distance(initialposition, transform.position);
        if (distance_from_spawn > despawn_radius)
        {
            Destroy(gameObject);
        } 

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrain") || collision.gameObject.CompareTag("player"))
        {
            Destroy(gameObject);
        }
   }
}

