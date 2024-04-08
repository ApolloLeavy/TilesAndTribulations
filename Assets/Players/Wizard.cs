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
    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }
    public override void Update()
    {
        base.Update();
     }
}
