using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_behaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float despawn_radius = 50f;
    [SerializeField] private float despawn_time = 50f;
    [SerializeField] private float precision = 0f;

    [SerializeField] private bool enable_precision = false;
    public int bullet_damage = 10;

    private GameObject player;
    private Vector3 initialposition;
    private float distance_from_spawn;
    private float bullet_epoch;
   // private Collider playershell = player.GetComponent<Collider>();

    void Start()
    {

        player = GameObject.Find("player 1");
        bullet_epoch = Time.time;
        initialposition =  transform.position;
        
        if (enable_precision) { transform.localRotation = Quaternion.Euler(Random.Range(-precision, precision), Random.Range(-precision, precision), Random.Range(-precision, precision)); }
        if (rb != null) { rb.linearVelocity = transform.forward * speed; }
    }

    void Update()
    {
        distance_from_spawn = Vector3.Distance(initialposition, transform.position);
        if (distance_from_spawn > despawn_radius)
        {
            Destroy(gameObject);
        }
        if (bullet_epoch + despawn_time < Time.time)
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

