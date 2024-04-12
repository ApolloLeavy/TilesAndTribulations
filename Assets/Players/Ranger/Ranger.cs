using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Ranger : Player
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
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, 1) });

        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 1) });
    }
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if (flag == "Q")
        {
            if (IsServer)
            {
                if (canQ)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 11);
                    isFlipped = !isFlipped;
                    PreviewAbility(tileLibrary[tiles[activeTile]], 11);
                    isFlipped = !isFlipped;
                    canQ = false;
                    StartCoroutine(Q());
                    SendUpdate("Q", canQ.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canQ = bool.Parse(value);
            }
        }
        if (flag == "W")
        {
            if (IsServer)
            {
                if (canQ)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 12);
                    isFlipped = !isFlipped;
                    PreviewAbility(tileLibrary[tiles[activeTile]], 12);
                    isFlipped = !isFlipped;
                    canW = false;
                    StartCoroutine(W());
                    SendUpdate("W", canW.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canW = bool.Parse(value);
            }
        }
        if (flag == "E")
        {
            if (IsServer)
            {
                if (canE)
                {
                    PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 13);
                    isFlipped = !isFlipped;
                    PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 13);
                    isFlipped = !isFlipped;

                    canE = false;
                    StartCoroutine(E());
                    SendUpdate("E", canE.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canW = bool.Parse(value);
            }
        }
        if (flag == "R")
        {
            if (IsServer)
            {
                if (canR)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 14);
                    isFlipped = !isFlipped;
                    PreviewAbility(tileLibrary[tiles[activeTile]], 14);
                    isFlipped = !isFlipped;
                    canR = false;
                    StartCoroutine(R());
                    SendUpdate("R", canR.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canR = bool.Parse(value);
            }
        }
    }
    public override void Attack2()
    {
        GameObject o = MyCore.NetCreateObject(8, Owner, myRig.position + new Vector3(lastInput.x, lastInput.y, 0));
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized * speed;
        
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
        myRig.position += new Vector3(-1, 0, 0);
    }
    public override void Update()
    {
        base.Update();
    }

}