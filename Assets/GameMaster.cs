using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class GameMaster : NetworkComponent
{
    public bool canPlay;
    public List<GameObject> players;
    public GameObject gameCanvas;
    public List<int> classesTaken;
    public bool gameOver;
    public GameObject[] monsters;
    public GameObject scoreboard;
    public int timer;
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (!canPlay)
        {
            while (IsServer)
            {

                if (IsDirty)
                {

                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
            
        while (!gameOver)
        {
            while (IsServer)
            {
                if (IsDirty)
                {

                    IsDirty = false;
                }
                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(.1f);
        }
        while (gameOver)
        {
            while (IsServer)
            {

                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(.1f);
        }
    }
    
    public void ReadyCheck()
    {
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerNetworkManager>().isReady)
            {
                canPlay = true;
            }
            else
            {
                canPlay = false;
                break;
            }

        }
        if(canPlay)
        {
            StartGame();

        }
    }
    public void SelectClass(int i, int k)
    {
        classesTaken.Add(i);
        
        foreach(GameObject o in players)
        {
            if(k != -1)
            classesTaken.Remove(k);
            o.GetComponent<PlayerNetworkManager>().SendUpdate("CLSBUT", string.Join(',',classesTaken));
        }
    }
    public void StartGame()
    {
        foreach (GameObject o in players)
        {
            o.GetComponent<PlayerNetworkManager>().SpawnCharacter();
            
        }


    }
    public void EndGame()
    {
        int i = 0;
        foreach(GameObject o in players)
        {
            i++;
            GameObject t;
            t = GameObject.Find("P"+i+"Entry");
            t.transform.GetChild(0).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().playerName;
            t.transform.GetChild(1).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().character.kills.ToString();
            t.transform.GetChild(2).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().character.deaths.ToString();
            t.transform.GetChild(3).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().character.assists.ToString();
        }
        scoreboard.GetComponent<CanvasRenderer>().SetAlpha(1);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameCanvas = GameObject.Find("GameCanvas");
        scoreboard = GameObject.Find("Scoreboard");
        scoreboard.GetComponent<CanvasRenderer>().SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
