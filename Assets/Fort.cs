using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fort : MonoBehaviour
{

    public float Cooldown = 2;
    float cooldown;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            Explosion.Go(new Vector2(2, 2), 2, 2, 3);
            cooldown = Cooldown;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if( collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().Destroy();
        }
    }
}
