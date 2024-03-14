using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class GameMaster : NetworkComponent
{
    public bool canPlay;
    public List<GameObject> players;
    public GameObject gameCanvas;
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
    public void StartGame()
    {
        gameCanvas.GetComponent<CanvasGroup>().alpha = 0;
        foreach(GameObject player in players)
        {
            player.GetComponent<Player>().SpawnCharacter();
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
