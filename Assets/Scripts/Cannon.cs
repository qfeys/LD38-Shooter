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
    bool IsReady = false;

    // Use this for initialization
    void Start () {
		
	}

    public void Go(Vector2 target)
    {
        this.target = target;
        IsReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.Active)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0 && IsReady)
            {
                Granade.Go(transform.position, target + Random.insideUnitCircle * spread, 5, ExplSize, damage, Suppression);
                cooldown = Cooldown;
            }
        }
    }
}
