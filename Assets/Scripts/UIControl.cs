using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public static UIControl instance;

    public GameObject clock;
    public GameObject kills;
    public GameObject coins;
    public GameObject lifes;
    public GameObject EndPanel;

    static float time;
    static int kills_int;
    static int coins_int;
    static int lifes_int;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.Active)
        {
            time += Time.deltaTime;
            if (lifes_int <= 0)
            {
                GM.instance.EndGame();
                EndPanel.SetActive(true);
                EndPanel.transform.GetChild(1).GetComponent<Text>().text = "You lasted " + ((int)time / 60) + " minutes and " + ((int)time % 60) + " seconds";
                EndPanel.transform.GetChild(2).GetComponent<Text>().text = "You killed " + kills_int + " enemies";
            }
        }
    }

    public void OnGUI()
    {
        clock.GetComponent<Text>().text = "" + ((int)time / 60) + ":" + ((int)time % 60).ToString("00");
        kills.GetComponent<Text>().text = "" + kills_int.ToString("000");
        coins.GetComponent<Text>().text = "" + coins_int.ToString("000");
        lifes.GetComponent<Text>().text = "" + lifes_int.ToString("00");
    }

    public static void Reset()
    {
        time = 0;
        kills_int = 0;
        coins_int = 0;
        lifes_int = 15;
        instance.EndPanel.SetActive(false);
    }

    public static void AddKill()
    {
        kills_int++;
        coins_int++;
    }

    public static void LifeLost()
    {
        lifes_int--;
    }

    public static float GetTime()
    {
        return time;
    }

    public static bool SpendCoins(int amount)
    {
        if (coins_int < amount)
            return false;
        else
        {
            coins_int -= amount;
            return true;
        }
    }
}
