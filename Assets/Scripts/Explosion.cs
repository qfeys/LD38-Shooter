using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float maxSize;
    float Damage;
    float Suppression;

    float progression;

    static ObjectPool ExplosionPool;
    const float SPEED = 2f;



    public static void Go(Vector2 location, float size, float damage, float suppression)
    {
        if(ExplosionPool == null)
            ExplosionPool = GameObject.Find("Explosions").GetComponent<ObjectPool>();
        GameObject go = ExplosionPool.GetNextObject();
        Explosion ex = go.GetComponent<Explosion>();
        ex.maxSize = size; ex.Damage = damage; ex.Suppression = suppression;
        go.transform.position = location;
        ParticleSystem.ShapeModule shape = go.GetComponent<ParticleSystem>().shape;
        shape.radius = 0.1f;
        ex.progression = 0;
        go.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        ParticleSystem.ShapeModule shape = GetComponent<ParticleSystem>().shape;
        shape.radius += SPEED * Time.deltaTime;
        progression += SPEED / maxSize * Time.deltaTime;
        if(progression >= 1)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>()!= null)
        {
            Enemy e = collision.GetComponent<Enemy>();
            e.Hit(Damage * (1 - progression));
            e.Supress(Suppression * (1 - progression));
        }
    }
}
