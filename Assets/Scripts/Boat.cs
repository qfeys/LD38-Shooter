using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Boat : MonoBehaviour
{

    public float Speed = 0.7f;
    public float Rotation = 0.5f;
    float speed;
    bool OnLand = false;

    // Use this for initialization
    void Start()
    {
        // GO(new Vector2(7, 1), GameObject.Find("Map").GetComponent<Collider2D>());
    }

    public void GO(Vector2 StartingLoc, Collider2D map)
    {
        speed = Speed;
        OnLand = false;

        transform.position = StartingLoc;
        ColliderDistance2D distance = gameObject.GetComponent<Collider2D>().Distance(map);
        transform.rotation = Quaternion.FromToRotation(Vector2.up, -distance.normal);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        if (OnLand)
        {
            if(speed > 0)
                speed -= Speed * 1.5f * Time.deltaTime;
            else
            {
                speed = 0;
                Color c = gameObject.GetComponent<SpriteRenderer>().color;
                c.a -= 0.5f * Time.deltaTime;
                gameObject.GetComponent<SpriteRenderer>().color = c;
                if (c.a <= 0)
                {
                    gameObject.SetActive(false);
                    c.a = 1;
                    gameObject.GetComponent<SpriteRenderer>().color = c;
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Map")
        {
            OnLand = true;
            GetComponentsInChildren<Enemy>().ToList().ForEach(e => e.GO());
            transform.DetachChildren();
        }
    }
}
