using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GM : MonoBehaviour {
    public static GM instance;
    public static bool Active;

    Collider2D map;
    ObjectPool boats;
    ObjectPool enemies;

    public GameObject fort;
    public GameObject reticule;
    public GameObject GranTurretPrfb;
    public GameObject GunTurretPrfb;
    public GameObject AreaDecarmation;

    float timer;
    Rect guiArea;
    GameObject ReadiedTurret;

    enum MouseState
    {
        Blank,
        BuildGranTurret,
        BuildGunTurret,
        WaitForConfirmation
    }

    MouseState mouseState = MouseState.Blank;

    // Use this for initialization
    void Start ()
    {
        instance = this;
        map = GameObject.Find("Map").GetComponent<Collider2D>();
        boats = GameObject.Find("Boats").GetComponent<ObjectPool>();
        enemies = GameObject.Find("Enemies").GetComponent<ObjectPool>();

        Enemy.goal = fort;

        // Find gui area
        Vector3[] fourCornersArray = new Vector3[4];
        ((RectTransform)(GameObject.Find("PurchasePanel").transform)).GetWorldCorners(fourCornersArray);
        Vector2 position = fourCornersArray[0];
        Vector2 size = fourCornersArray[2] - fourCornersArray[0];
        guiArea = new Rect(position, size);

        StartNewGame();
    }

    public void StartNewGame()
    {
        UIControl.Reset();
        Active = true;
    }

    internal void EndGame()
    {
        Active = false;
        FindObjectsOfType<Enemy>().ToList().ForEach(e => e.Disable());
        FindObjectsOfType<Boat>().ToList().ForEach(b => b.gameObject.SetActive(false));
        FindObjectsOfType<Gun>().ToList().ForEach(g => { if (g.transform.parent== null) Destroy( g.gameObject); });
        FindObjectsOfType<Cannon>().ToList().ForEach(c => { if (c.gameObject.name != "Fort") Destroy(c.gameObject); });
    }

    // Update is called once per frame
    void Update () {
        if (Active)
        {
            // SPAWNING
            if (timer <= 0)
            {
                timer = 1.0f / ((((int)UIControl.GetTime() / 10) + 1)/2f);
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
                case MouseState.BuildGranTurret:
                    Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(pos));
                    RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
                    if (hits.Any(h => h.collider.name == "CollisionMap"))
                    {
                        if (UIControl.SpendCoins(20))
                            BuildGranadeTurret(pos);
                    }
                    break;
                case MouseState.BuildGunTurret:
                    Ray ray2 = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(pos));
                    RaycastHit2D[] hits2 = Physics2D.GetRayIntersectionAll(ray2, Mathf.Infinity);
                    if (hits2.Any(h => h.collider.name == "CollisionMap"))
                    {
                        if (UIControl.SpendCoins(20))
                            BuildGunTurret(pos);
                    }
                    break;
                case MouseState.WaitForConfirmation:
                    ConfirmTurret(pos);
                    break;
                }
            }


            // Other mouse Actions
            if (mouseState == MouseState.WaitForConfirmation)
            {
                if(ReadiedTurret == null)
                {
                    mouseState = MouseState.Blank;
                    return;
                }
                Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(mousPos, ReadiedTurret.transform.position) <= ReadiedTurret.GetComponent<Cannon>().maxRange)
                {
                    AreaDecarmation.transform.position = mousPos;
                }
                else
                {
                    AreaDecarmation.transform.position =
                        (mousPos - (Vector2)ReadiedTurret.transform.position).normalized * ReadiedTurret.GetComponent<Cannon>().maxRange
                        + (Vector2)ReadiedTurret.transform.position;
                }
            }
        }
	}

    void SpawnBoat()
    {
        // Set location
        float x; float y;
        float v = UnityEngine.Random.value;
        if (v < 0.64f)  // long side
        {
            x = UnityEngine.Random.Range(-10, 10);
            y = v > 0.32 ? 5 : -5;
        }
        else            // short side
        {
            y = UnityEngine.Random.Range(-5, 5);
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
    }

    private void BuildGranadeTurret(Vector2 pos)
    {
        GameObject nt = Instantiate(GranTurretPrfb);
        nt.transform.position = pos;
        AreaDecarmation.SetActive(true);
        mouseState = MouseState.WaitForConfirmation;
        ReadiedTurret = nt;
    }

    private void ConfirmTurret(Vector2 pos)
    {
        if (Vector2.Distance(pos, ReadiedTurret.transform.position) <= ReadiedTurret.GetComponent<Cannon>().maxRange)
        {
            ReadiedTurret.GetComponent<Cannon>().Go(pos);
        }
        else
        {
            ReadiedTurret.GetComponent<Cannon>().
                Go((pos - (Vector2)ReadiedTurret.transform.position).normalized * ReadiedTurret.GetComponent<Cannon>().maxRange);
        }
        AreaDecarmation.SetActive(false);
        mouseState = MouseState.Blank;
    }

    public void MousStateGunTurret()
    {
        if (mouseState != MouseState.BuildGunTurret)
            mouseState = MouseState.BuildGunTurret;
        else
            mouseState = MouseState.Blank;
    }

    private void BuildGunTurret(Vector2 pos)
    {
        GameObject nt = Instantiate(GunTurretPrfb);
        nt.transform.position = pos;
        mouseState = MouseState.Blank;
    }
}
