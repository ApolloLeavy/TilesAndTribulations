using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class PlayerNetworkManager : NetworkComponent
{
    public bool isReady;
    public string playerName = "";
    public InputField NameField;
    public Toggle ReadyButton;
    public GameMaster gameMaster;
    public List<GameObject> classButtons;
    public int classIndex = 0;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "NAME" && !(value.Equals("")))
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
                    if (playerName != "")
                        ReadyButton.interactable = true;
                    else
                        ReadyButton.interactable = false;
                }
            }
        }
        if (flag == "RDY")
        {
            if (IsServer)
            {

                isReady = bool.Parse(value);
                gameMaster.GetComponent<GameMaster>().ReadyCheck();
                ReadyButton.isOn = isReady;
                SendUpdate("RDY", value);
            }
            if (IsClient)
            {
                isReady = bool.Parse(value);
                ReadyButton.isOn = isReady;

            }
        }
        if (flag == "CLASS")
        {
            
            int i = int.Parse(value);
            if (IsServer)
            {
                IsDirty = true;
                gameMaster.classesTaken.Remove(classIndex);
                gameMaster.classesTaken.Add(i);
                classIndex = i;
                SendUpdate("CLASS", value);
                SendUpdate("CLASSUP", "");
            }
            if (IsClient)
            {
                
                if (classIndex != -1)
                {
                    
                    gameMaster.classesTaken.Remove(classIndex);
                    classButtons[classIndex].GetComponent<Image>().color = Color.white;
                }
                gameMaster.classesTaken.Add(i);
                classButtons[i].GetComponent<Image>().color = Color.green;
                classIndex = i;
                
            }
            if (IsLocalPlayer)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (gameMaster.classesTaken.Contains(j))
                        classButtons[j].GetComponent<Button>().interactable = false;
                    else
                        classButtons[j].GetComponent<Button>().interactable = true;
                }

            }
        }

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
    public void Name(string value)
    {
        IsDirty = true;
        playerName = value;
        SendUpdate("NAME", value);
    }
    public void Ready(bool value)
    {
        IsDirty = true;
        isReady = value;
        NameField.interactable = false;
        SendUpdate("RDY", value.ToString());
    }
    public void ClassSelect(int i)
    {
        IsDirty = true;
        SendCommand("CLASS", i.ToString());
    }
    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {

            while (IsServer)
            {

                if (IsDirty)
                {
                    SendUpdate("NAME", playerName);
                    SendUpdate("RDY", isReady.ToString());
                    SendUpdate("CLASS", classIndex.ToString());
                    IsDirty = false;
                }

                yield return new WaitForSeconds(.1f);

            }

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
        gameMaster.IsDirty = true;
        ReadyButton.interactable = false;


    }

    // Update is called once per frame
    void Update()
    {

    }
}
