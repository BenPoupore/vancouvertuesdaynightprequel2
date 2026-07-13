using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emeny_bullet_launcher : MonoBehaviour
{
    public GameObject bullet;
    public float bullet_speed = 1;
    public float bullets_per_second = 1f;

    IEnumerator shotperiod()
    {
        while (true) {
            yield return new WaitForSeconds(bullets_per_second);
            spawnbullet();
        }
    }

    void OnEnable() {
        StartCoroutine(shotperiod());
    }

    void spawnbullet()
    {
        GameObject newbullet = Instantiate(bullet, transform.position, transform.rotation);
    }
}
