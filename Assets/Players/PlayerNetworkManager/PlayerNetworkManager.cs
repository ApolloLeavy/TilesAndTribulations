using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class PlayerNetworkManager : NetworkComponent
{
    public bool isReady;
    public bool gameStarted;
    public string playerName = "";
    public InputField NameField;
    public Toggle ReadyButton;
    public GameMaster gameMaster;
    public List<GameObject> classButtons;
    public GameObject gameCanvas;
    public GameObject disconnect;
    public int classIndex;
    public int kills;
    public int deaths;
    public int assists;
    

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "NAME")
        {
            if (IsServer)
            {

                playerName = value;
                NameField.text = value;
                SendUpdate("NAME", value);

            }
            if (IsClient)
            {
                playerName = value;
                NameField.text = value;

                if (IsLocalPlayer)
                {
                    if (playerName != "" && classIndex != -1)
                        ReadyButton.interactable = true;
                    else
                        ReadyButton.interactable = false;
                }
            }
        }
        if(flag == "START")
        {
            if(IsServer)
            {
               
            }
            
            gameStarted = true;
            
        }
        if (flag == "RDY")
        {
            if(playerName != "" && classIndex != -1)
            {
                isReady = bool.Parse(value);
                ReadyButton.isOn = isReady;
                if (IsServer)
                {
                    gameMaster.GetComponent<GameMaster>().ReadyCheck();
                    SendUpdate("RDY", value);
                }
                if (IsLocalPlayer)
                {
                    if (isReady)
                    {
                        NameField.interactable = false;
                        foreach (GameObject b in classButtons)
                        {
                            b.GetComponent<Button>().interactable = false;
                        }
                    }
                    else
                    {
                        NameField.interactable = true;
                    }
                }
            }
            
        }
        if (flag == "CLASS")
        {
            
            int i = int.Parse(value);
            if (IsServer)
            {
                
                if (!gameMaster.classesTaken.Contains(i))
                {
                    gameMaster.SelectClass(i, classIndex);
                    
                    classIndex = i;
                    SendUpdate("CLASS", value);
                    SendUpdate("CLSBUT", string.Join(',', gameMaster.classesTaken));
                    gameCanvas.GetComponent<CanvasGroup>().alpha = 0;
                }
                
                    
            }
            
            if (IsClient)
            {
                classIndex = i;
                if (IsLocalPlayer)
                {
                    if (playerName != "" && classIndex != -1)
                        ReadyButton.interactable = true;
                    else
                        ReadyButton.interactable = false;
                }
            }
            
        }
        if(flag == "CLSBUT")
        {
            string[] i = value.Split(',');
            gameMaster.classesTaken.Clear();
            if(IsClient)
            {
                foreach (string s in i)
                {
                    gameMaster.classesTaken.Add(int.Parse(s));
                    
                }
                

            }
            
        }
        if(flag == "CANCEL")
        {
            if(IsServer)
            {
                if(classIndex != -1)
                {
                    SendUpdate("CANCEL", classIndex.ToString());
                    gameMaster.classesTaken.Remove(classIndex);
                    classIndex = -1;
                    
                    IsDirty = true;
                }
            }
            if(IsClient)
            {
                gameMaster.classesTaken.Remove(int.Parse(value));
                classIndex = -1;
            }
        }
    }
    public void SpawnCharacter()
    {
        gameStarted = true;
        SendUpdate("START", "");
        MyCore.NetCreateObject(classIndex, Owner, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
    public override void NetworkedStart()
    {
        if (IsClient && !IsLocalPlayer)
        {
            ReadyButton.interactable = false;
            NameField.interactable = false;
            foreach (GameObject cb in classButtons)
            {
                cb.GetComponent<Button>().interactable = false;
            }
        }
    }
    private void OnDestroy()
    {
        gameMaster.classesTaken.Remove(classIndex);
        gameMaster.players.Remove(gameObject);
    }
    public void Name(string value)
    {
        IsDirty = true;
        playerName = value;
        SendCommand("NAME", value);
    }
    public void Ready(bool value)
    {
        IsDirty = true;
        SendCommand("RDY", value.ToString());
    }
    public void ClassSelect(int i)
    {
        IsDirty = true;
        if (i != -1)
            SendCommand("CLASS", i.ToString());
        else
            SendCommand("CANCEL", "");
    }
    public override IEnumerator SlowUpdate()
    {
            while(!gameStarted)
            {
                    if (IsDirty)
                    {
                        SendUpdate("NAME", playerName);
                        SendUpdate("RDY", isReady.ToString());
                        SendUpdate("CLASS", classIndex.ToString());
                        SendUpdate("CLSBUT", string.Join(',', gameMaster.classesTaken));
                        IsDirty = false;
                    }
                if (IsLocalPlayer && !isReady)
                {
                    for (int o = 0; o < 4; o++)
                    {
                        if (gameMaster.classesTaken.Contains(o+3))
                            classButtons[o].GetComponent<Button>().interactable = false;
                        else
                            classButtons[o].GetComponent<Button>().interactable = true;
                    }
                }
                yield return new WaitForSeconds(.1f);
            }
            if(IsLocalPlayer)
            {
                NameField.gameObject.SetActive(false);
                ReadyButton.gameObject.SetActive(false);
            }
            while (!gameMaster.gameOver)
            {
                    if (IsDirty)
                    {
                        IsDirty = false;
                    }
                yield return new WaitForSeconds(.1f);
            }
            while(gameMaster.gameOver)
            {

                yield return new WaitForSeconds(.1f);
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("Players").transform);
        isReady = false;
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        gameMaster.players.Add(this.gameObject);
        ReadyButton.interactable = false;
        gameCanvas = GameObject.Find("GameCanvas");
        classIndex = -1;
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
