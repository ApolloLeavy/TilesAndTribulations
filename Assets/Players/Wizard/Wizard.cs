using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Wizard : Player
{
    
    public bool necklace;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        ring = false;
        necklace = false;
        hp = 25;
        hpM = 25;
        speed = 4;
        acd = 2.5f;
        qcd = 8;
        wcd = 10;
        ecd = 8;
        rcd = 15;
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
                    if (necklace)
                    {
                        StartCoroutine(NecklaceTimer());
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 15);
                        lastInput *= -1;
                    }
                    if (!ring)
                        canQ = false;
                    else
                        StartCoroutine(RingTimer());

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
                    if (necklace)
                    {
                        StartCoroutine(NecklaceTimer());
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 16);
                        lastInput *= -1;
                    }
                    if (!ring)
                        canW = false;
                    else
                        StartCoroutine(RingTimer());
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
                    if (necklace)
                    {
                        StartCoroutine(NecklaceTimer());
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 17);
                        lastInput *= -1;
                    }

                    if (!ring)
                        canE = false;
                    else
                        StartCoroutine(RingTimer());
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
                    if(necklace)
                    {
                        StartCoroutine(NecklaceTimer());
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 18);
                        lastInput *= -1;
                    }
                    if (!ring)
                        canR = false;
                    else
                        StartCoroutine(RingTimer());
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
        GameObject o = MyCore.NetCreateObject(33,Owner,myRig.position+new Vector3(lastInput.x,lastInput.y,0), Quaternion.Euler(0, 0, lastInput.y * 90));
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
    public IEnumerator RingTimer()
    {
        ring = false;
        yield return new WaitForSeconds(7);
        ring= true;    
    }
    public IEnumerator NecklaceTimer()
    {
        necklace = false;
        yield return new WaitForSeconds(7);
        necklace = true;
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (IsServer)
        {
            switch (other.tag)
            {
                case "Ring":
                    {
                        ring = true;
                        break;
                    }
                case "Necklace":
                    {
                        necklace = false;

                        break;
                    }
            }
        }
    }
}
