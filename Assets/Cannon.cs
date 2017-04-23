using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public float Cooldown = 2;
    float cooldown;
    public float maxRange = 3;
    public float spread = 1;
    public float damage = 1;
    public float ExplSize = 3;
    public float Suppression = 3;
    Vector2 target;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            Granade.Go(transform.position, target + Random.insideUnitCircle * spread, 5, ExplSize, damage, Suppression);
            cooldown = Cooldown;
        }
    }
}
