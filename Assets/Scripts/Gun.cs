using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float range = 1;
    public float heatCap = 5;
    public float fireRate = 2;
    public float cooldownRate = 0.5f;
    LineRenderer line;
    CircleCollider2D coll;
    UnityEngine.UI.Slider slider;

    bool graphicActive = false;
    bool operational = true;
    float timeUntilShot = 0;
    float _heat;
    float heat { get { return _heat; }set { _heat = value; slider.value = heat / heatCap; } }

    const float FADE_OUT_TIME = 0.15f;

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        line.SetPosition(0, transform.position);
        coll = GetComponent<CircleCollider2D>();
        coll.radius = range;
        operational = true;
        slider = GetComponentInChildren<UnityEngine.UI.Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (graphicActive)
        {
            if (timeUntilShot < 1 / fireRate - FADE_OUT_TIME)
            {
                graphicActive = false;
                line.enabled = false;
            }
        }
        if(timeUntilShot <= 0 && operational)
        {
            Collider2D[] entries = new Collider2D[10];
            int n = coll.OverlapCollider(new ContactFilter2D(), entries);
            for(int i = 0; i < n; i++)
            {
                if (entries[i].GetComponent<Enemy>() != null)
                {
                    Shoot(entries[i].transform.position);
                    entries[i].GetComponent<Enemy>().Hit(1);
                    break;
                }
            }
        }
        if(timeUntilShot > 0)
        {
            timeUntilShot -= Time.deltaTime;
        }
        if (heat > 0)
        {
            heat -= cooldownRate * Time.deltaTime;
        }
        if(heat > heatCap)
        {
            operational = false;
        }
        if(operational == false && heat <= 0)
        {
            operational = true;
        }
	}

    void Shoot(Vector2 loc)
    {
        heat++;
        timeUntilShot = 1 / fireRate;
        graphicActive = true;
        line.enabled = true;
        line.SetPosition(1, loc);
        SoundBoard.instance.PlayGun();
    }
}
