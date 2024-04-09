using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Wizard : Player
{
    public GameObject fireball;
    public GameObject icestorm;
    public GameObject teleport;
    public GameObject haste;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hp = 10;
        speed = 1;
        qcd = 5;
        wcd = 5;
        ecd = 5;
        rcd = 5;
    }
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if(flag == "Q")
        {
            if(IsServer)
            {
                if(canQ)
                {
                    PreviewAbility(tiles[activeTile], 7);
                    canQ = false;
                    StartCoroutine(Q());
                    SendUpdate("Q", canQ.ToString());
                }
            }
            if(IsLocalPlayer)
            {
                canQ = bool.Parse(value);
            }
        }
        if(flag == "W")
        {
            if(IsServer)
            {
                if(canQ)
                {
                    PreviewAbility(tiles[activeTile], 8);
                    canW = false;
                    StartCoroutine(W());
                    SendUpdate("W", canW.ToString());
                }
            }
            if(IsLocalPlayer)
            {
                canW = bool.Parse(value);
            }
        }
    }
    public IEnumerator Q()
    {
        yield return new WaitForSeconds(qcd);
        canQ = true;
        SendUpdate("Q", canQ.ToString());
    }
    public IEnumerator W()
    {
        yield return new WaitForSeconds(wcd);
        canW = true;
        SendUpdate("W", canW.ToString());
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }
    public override void Update()
    {
        base.Update();
     }
    public void OnQ(InputAction.CallbackContext ev)
    {
        if (ev.started && canQ)
            SendCommand("Q", "");
    }
    public void OnW(InputAction.CallbackContext ev)
    {
        if (ev.started && canW)
            SendCommand("W", "");
    }
    public void OnE(InputAction.CallbackContext ev)
    {
        if (ev.started && canQ)
            SendCommand("E", "");
    }
    public void OnR(InputAction.CallbackContext ev)
    {
        if (ev.started && canW)
            SendCommand("R", "");
    }
}
