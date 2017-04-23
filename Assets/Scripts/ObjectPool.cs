using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public GameObject prefab;

    List<GameObject> pool;

    // Use this for initialization
    void Awake()
    {
        pool = new List<GameObject>();
    }

    public GameObject GetNextObject()
    {
        foreach (GameObject go in pool)
        {
            if (go.activeSelf == false)
            {
                go.SetActive(true);
                return go;
            }
        }
        GameObject newGO = Instantiate(prefab);
        newGO.transform.SetParent(transform);
        pool.Add(newGO);
        return newGO;
    }
}
