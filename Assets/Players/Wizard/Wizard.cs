using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Wizard : Player
{
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hp = 10;
        speed = 4;
        acd = .75f;
        qcd = 5;
        wcd = 5;
        ecd = 5;
        rcd = 5;
        tileLibrary.Add(new Vector2[] { new Vector2(1, -1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(-1, 0), new Vector2(-1, 1) });

        tileLibrary.Add(new Vector2[] { new Vector2(1, 1), new Vector2(1, 1), new Vector2(-1, 1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, 1) });
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
                    PreviewAbility(tileLibrary[tiles[activeTile]], 15);
                    canQ = false;

                    StartCoroutine(Q());

                    SendUpdate("Q", canQ.ToString());
                }
            }
            if(IsLocalPlayer)
            {
                canQ = bool.Parse(value);
                if (!canQ)
                {
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
            }
        }
        if(flag == "W")
        {
            if(IsServer)
            {
                if(canW)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 16);
                    canW = false;
                    StartCoroutine(W());
                    SendUpdate("W", canW.ToString());
                }
            }
            if(IsLocalPlayer)
            {
                canW = bool.Parse(value);
                if (!canW)
                {
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
            }
        }
        if (flag == "E")
        {
            if (IsServer)
            {
                if (canE)
                {
                    PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 17);
                    
                    canE = false;
                    StartCoroutine(E());
                    SendUpdate("E", canE.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canE = bool.Parse(value);
            }
        }
        if (flag == "R")
        {
            if (IsServer)
            {
                if (canR)
                {

                    PreviewAbility(tileLibrary[tiles[activeTile]], 18);
                    canR = false;
                    StartCoroutine(R());
                    SendUpdate("R", canR.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canR = bool.Parse(value);
                if (!canR)
                {
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
            }
        }
    }

    public override void Attack2()
    {
        GameObject o = MyCore.NetCreateObject(33,Owner,myRig.position+new Vector3(lastInput.x,lastInput.y,0));
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y,0).normalized*3.5f;
        
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
        myRig.position += new Vector3(1, 0, 0);
    }
    public override void Update()
    {
        base.Update();
     }
    
}
