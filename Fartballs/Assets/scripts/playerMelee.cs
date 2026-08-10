using UnityEngine;
using UnityEngine.InputSystem;
public class playerMelee : MonoBehaviour
{
    [SerializeField] private GameObject kick_projectile;
    [SerializeField] private float kick_time_length = 0.25f;
    [SerializeField] private float cooldown = 0.25f;
    private float savetime;
    public float cooldown_temp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kick_projectile.SetActive(false);
        cooldown_temp = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(cooldown_temp);
        handle_kick();
    }
    void handle_kick() {

        if (Keyboard.current.fKey.isPressed && cooldown_temp <= 0f) {
            savetime = Time.time;
            kick_projectile.SetActive(true);
            cooldown_temp = cooldown;
        }
        if (savetime + kick_time_length <= Time.time)
        {
            kick_projectile.SetActive(false);
        }
        if (cooldown_temp >= 0f)
        {
            cooldown_temp -= Time.deltaTime;
        }
    }
}
