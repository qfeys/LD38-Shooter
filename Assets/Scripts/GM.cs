using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {

    Collider2D map;
    ObjectPool boats;
    ObjectPool enemies;

    public GameObject fort;

    float timer;

	// Use this for initialization
	void Start ()
    {
        map = GameObject.Find("Map").GetComponent<Collider2D>();
        boats = GameObject.Find("Boats").GetComponent<ObjectPool>();
        enemies = GameObject.Find("Enemies").GetComponent<ObjectPool>();

        Enemy.goal = fort;
    }
	
	// Update is called once per frame
	void Update () {
        if (timer <= 0)
        {
            timer = 0.5f;
            SpawnBoat();
        }
        timer -= Time.deltaTime;
	}

    void SpawnBoat()
    {
        // Set location
        float x; float y;
        float v = Random.value;
        if (v < 0.64f)  // long side
        {
            x = Random.Range(-10, 10);
            y = v > 0.32 ? 5 : -5;
        }
        else            // short side
        {

            y = Random.Range(-5, 5);
            x = v > 0.83 ? 10 : -10;
        }
        GameObject b = boats.GetNextObject();
        b.GetComponent<Boat>().GO(new Vector2(x, y), map);

        // Spawn enemies
        for (int i = 0; i < 6; i++)
        {
            GameObject en = enemies.GetNextObject();
            en.transform.parent = b.transform;
            en.transform.localPosition = Vector3.zero;
            en.transform.localRotation = Quaternion.identity;
        }
    }
}
