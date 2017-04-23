using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{

    public GameObject clock;
    public GameObject kills;
    public GameObject lifes;

    static float time;
    static int kills_int;
    static int lifes_int;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    public void OnGUI()
    {
        clock.GetComponent<Text>().text = "" + ((int)time / 60) + ":" + ((int)time % 60).ToString("00");
        kills.GetComponent<Text>().text = "" + kills_int.ToString("000");
        lifes.GetComponent<Text>().text = "" + lifes_int.ToString("00");
    }

    public static void Reset()
    {
        time = 0;
        kills_int = 0;
        lifes_int = 15;
    }

    public static void AddKill()
    {
        kills_int++;
    }

    public static void LifeLost()
    {
        lifes_int--;
    }

    public static float GetTime()
    {
        return time;
    }
}
