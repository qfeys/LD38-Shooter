using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour {

    Collider2D map;
    ObjectPool boats;
    ObjectPool enemies;

    public GameObject fort;
    public GameObject reticule;

    float timer;
    Rect guiArea;

    enum MouseState
    {
        Blank,
        BuildGranTurret,
        BuildGunTurret
    }

    MouseState mouseState = MouseState.Blank;

    // Use this for initialization
    void Start ()
    {
        map = GameObject.Find("Map").GetComponent<Collider2D>();
        boats = GameObject.Find("Boats").GetComponent<ObjectPool>();
        enemies = GameObject.Find("Enemies").GetComponent<ObjectPool>();

        Enemy.goal = fort;

        UIControl.Reset();

        // Find gui area
        Vector3[] fourCornersArray = new Vector3[4];
        ((RectTransform)(GameObject.Find("PurchasePanel").transform)).GetWorldCorners(fourCornersArray);
        Debug.Log(""+fourCornersArray[0]+ fourCornersArray[1]+fourCornersArray[2]+fourCornersArray[3]);
        Vector2 position = fourCornersArray[0];
        Vector2 size = fourCornersArray[2] - fourCornersArray[0];
        guiArea = new Rect(position, size);
        Debug.Log(guiArea);
    }
	
	// Update is called once per frame
	void Update () {
        // SPAWNING
        if (timer <= 0)
        {
            timer = 1.0f / (((int)UIControl.GetTime() / 10) + 1); 
            SpawnBoat();
        }
        timer -= Time.deltaTime;

        // KEY INPUT
        if (Input.GetMouseButtonDown(0) && guiArea.Contains(Input.mousePosition) == false)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            switch (mouseState)
            {
            case MouseState.Blank:
                fort.GetComponent<Fort>().target = pos;
                reticule.transform.position = pos;
                break;
            }
        }

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

    public void MousStateGranadeTurret()
    {
        if (mouseState != MouseState.BuildGranTurret)
            mouseState = MouseState.BuildGranTurret;
        else
            mouseState = MouseState.Blank;

        Debug.Log(mouseState);
    }
}
