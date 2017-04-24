using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

    float speed = 5;
    float timeTilInpact;

    float maxSize;
    float Damage;
    float Suppression;

    static ObjectPool GranadePool;

    public static void Go(Vector2 Start, Vector2 target, float speed, float size, float damage, float suppression)
    {
        if (GranadePool == null)
            GranadePool = GameObject.Find("Granades").GetComponent<ObjectPool>();
        GameObject go = GranadePool.GetNextObject();
        Granade gr = go.GetComponent<Granade>();
        gr.speed = speed; gr.maxSize = size; gr.Damage = damage; gr.Suppression = suppression;
        go.transform.position = Start;
        go.transform.rotation = Quaternion.FromToRotation(Vector2.up, target - Start);
        gr.timeTilInpact = Vector2.Distance(Start, target) / speed;
        go.SetActive(true);
        SoundBoard.instance.PlayLaunch();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.up * speed * Time.deltaTime;
        timeTilInpact -= Time.deltaTime;
        if(timeTilInpact <= 0)
        {
            Explosion.Go(transform.position, maxSize, Damage, Suppression);
            gameObject.SetActive(false);
        }
	}
}
