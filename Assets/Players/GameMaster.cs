 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class GameMaster : NetworkComponent
{
    public bool canPlay;
    public bool stall;
    public List<GameObject> players;
    public GameObject gameCanvas;
    public List<int> classesTaken;
    public float spawnIntensity;
    public bool gameOver;
    public bool timerOver;
    public List<GameObject> monsters;
    public List<int> items;
    public GameObject Wscoreboard;
    public GameObject Lscoreboard;
    public int timer;
    public bool isWin;
    public bool dead;
    public override void HandleMessage(string flag, string value)
    {
        if(flag == "PLAY")
        {
            if(IsClient)
            {
                canPlay = bool.Parse(value);
            }
        }
        if (flag == "OVER")
        {
            if (IsClient)
            {
                gameOver = bool.Parse(value);
            }
        }
        
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while (!canPlay)
        {
                if (IsDirty)
                {
                    IsDirty = false;
                }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
        if (IsServer)
        {
            
        }
        StartCoroutine(Stall());
        while (stall)
        {
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }

        timer = 0;
        StartCoroutine(Delay());
        
        while (!gameOver)
        {
            if(IsServer)
            {
                foreach(GameObject p in players)
                {
                    if (!p.GetComponent<PlayerNetworkManager>().player.isDead)
                    {
                        dead = false;
                        break;
                    }
                    else
                    {
                        dead = true;
                    }
                }
                if (dead)
                {
                    gameOver = true;
                    IsDirty = true;
                }
            }
            if (IsDirty)
            {
                SendUpdate("OVER", gameOver.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }


        StartCoroutine(EndGame());
        while (gameOver)
        {
            

            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }
    public IEnumerator Stall()
    {
        if(IsServer)
            StartCoroutine(StallTimer());
        gameCanvas.GetComponent<Canvas>().enabled = false;
        GameObject.Find("Disconnect").SetActive(false);
        yield return new WaitForSeconds(5);
        stall = false;
    }
    public IEnumerator StallTimer()
    {
        yield return new WaitForSeconds(1);
        timer++;
        if (timer == 60)
            timer = 0;
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerNetworkManager>().player.SendUpdate("TIME","0:"+timer);
        }
        if(stall)
            StartCoroutine(StallTimer());
    }

    public IEnumerator SpawnMonster()
    {
        monsters.Add(MyCore.NetCreateObject(Random.Range(28,32), Owner, new Vector3(20, 2, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28,32), Owner, new Vector3(-20, 2, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28, 32), Owner, new Vector3(2, 15, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28, 32), Owner, new Vector3(2, -15, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28, 32), Owner, new Vector3(20, -2, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28, 32), Owner, new Vector3(-20, -2, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28, 32), Owner, new Vector3(-2, 15, 0), Quaternion.identity));
        monsters.Add(MyCore.NetCreateObject(Random.Range(28, 32), Owner, new Vector3(-2, -15, 0), Quaternion.identity));
        yield return new WaitForSeconds(20 / spawnIntensity);
        StartCoroutine(SpawnMonster());
    }
    public IEnumerator Delay()
    {
        
        if(IsServer)
        {
            StartCoroutine(SpawnIntensity());
            StartCoroutine(SpawnMonster());
            StartCoroutine(Time());
        }
        
        yield return new WaitForSeconds(180);
        timerOver = true;
        
        StartCoroutine(TriggerEnd());
    }
    public IEnumerator SpawnIntensity()
    {
        yield return new WaitForSeconds(60);
        spawnIntensity++;
        StartCoroutine(SpawnIntensity());
    }
    public IEnumerator Time()
    {
        yield return new WaitForSeconds(1);
        timer++;
        if (timer >= 60)
            timer -= 60;
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerNetworkManager>().player.SendUpdate("TIME", spawnIntensity + ":" + timer);
        }
            StartCoroutine(Time());
    }
    public IEnumerator TriggerEnd()
    {
        if(monsters.Count == 0)
        {
            gameOver = true;
        }
        yield return new WaitForSeconds(1);
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
            IsDirty = true;
            SendUpdate("PLAY", canPlay.ToString());
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
        
        MyCore.IsListening = false;
        foreach (GameObject o in players)
        {
            o.GetComponent<PlayerNetworkManager>().SpawnCharacter();
            
        }

        
    }
    public IEnumerator EndGame()
    {
        gameCanvas.GetComponent<Canvas>().enabled = true;
        if (isWin)
            Wscoreboard = GameObject.Instantiate(Wscoreboard,gameCanvas.transform);
        else
        {
            Lscoreboard = GameObject.Instantiate(Lscoreboard, gameCanvas.transform);
        }
        int i = 0;
        foreach(GameObject o in players)
        {
            i++;
            GameObject t;
            t = GameObject.Find("P"+i+"Entry");
            t.transform.GetChild(0).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().playerName;
            t.transform.GetChild(1).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().kills.ToString();
            t.transform.GetChild(2).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().deaths.ToString();
            t.transform.GetChild(3).GetComponent<Text>().text = o.GetComponent<PlayerNetworkManager>().assists.ToString();
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(10);
        MyCore.UI_Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        stall = true;
        gameCanvas = GameObject.Find("GameCanvas");
        gameOver = false;
        canPlay = false;
        isWin = false;
        dead = false;
        timer = 0;
        timerOver = false;
        spawnIntensity = 1;
        items = new List<int>();
        for (int i = 38; i <= 45; i++)
            items.Add(i);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
