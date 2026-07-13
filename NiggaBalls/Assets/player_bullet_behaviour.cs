using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_bullet_behaviour : MonoBehaviour
{
    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrane"))
        {
           // Destroy(gameObject);
        }

    }

    


}
