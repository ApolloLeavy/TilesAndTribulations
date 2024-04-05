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
    public int timer;
    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        if(IsConnected)
        {
            while(IsServer)
            {
                if(IsDirty)
                {
                   
                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            
            yield return new WaitForSeconds(MyId.UpdateFrequency);
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
    // Start is called before the first frame update
    void Start()
    {
        gameCanvas = GameObject.Find("GameCanvas");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
