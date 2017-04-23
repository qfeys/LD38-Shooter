using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fort : MonoBehaviour
{

    public float Cooldown = 2;
    float cooldown;
    public Vector2 target;
    
    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            Granade.Go(transform.position, target, 4, 2, 2, 3);
            cooldown = Cooldown;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if( collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().Destroy();
            UIControl.LifeLost();
        }
    }
}
