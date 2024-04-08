using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Wizard : Player
{
    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        hp = 10;
        speed = 1;
        qcd = 5;
        wcd = 5;
        ecd = 5;
        rcd = 5;
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }
    public void Update()
    {
        base.Update();
     }
}
