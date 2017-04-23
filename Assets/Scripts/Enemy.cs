using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {


    public float speed = 0.5f;
    public float rotSpeed = 0.5f;
    public int HP = 1;
    static public GameObject goal;

    bool IsActive = false;
    float suppressionTimer = 0f;
    BreadCrumb ActiveBC;

    public static Grid grid;

	// Use this for initialization
	void Start ()
    {
    }

    public void GO()
    {
        IsActive = true;
        HP = 1;
        ActiveBC = PathFinder.FindPath(grid, grid.WorldToGrid(transform.position), grid.WorldToGrid(goal.transform.position));
    }
    
    public void Hit(float damage)
    {
        if(damage >= 1)
        {
            Destroy();
        }else
        {
            if (Random.value < damage)
                Destroy();
        }
    }

    public void Supress(float suppression)
    {
        if(suppressionTimer<suppression)
            suppressionTimer = suppression;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        transform.position = new Vector2(100, 100);
        IsActive = false;
        suppressionTimer = 0f;
        UIControl.AddKill();
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsActive && suppressionTimer <= 0 && ActiveBC != null)
        {
            Vector3 targetPos;
            if (ActiveBC.next != null)
                targetPos = grid.GridToWorld(ActiveBC.next.position);
            else
                targetPos = grid.GridToWorld(ActiveBC.position);
            Quaternion targetRot = Quaternion.FromToRotation(Vector2.up, targetPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            transform.position += transform.up * speed * Time.deltaTime;

            if ((targetPos - transform.position).magnitude < Grid.UnitSize / 2) // To close
            {
                if (ActiveBC.next != null)
                    ActiveBC = ActiveBC.next;
            }
            if ((targetPos - transform.position).magnitude > Grid.UnitSize * 2)
            {
                ActiveBC = PathFinder.FindPath(grid, grid.WorldToGrid(transform.position), grid.WorldToGrid(goal.transform.position));
                //Debug.LogError("Recalculate path");
            }
        }else if(suppressionTimer > 0)
        {
            suppressionTimer -= Time.deltaTime;
        }
    }
}
