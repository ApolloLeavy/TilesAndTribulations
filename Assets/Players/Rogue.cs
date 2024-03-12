using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Rogue : Player
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
        hp = 10;
        speed = 1;
        qcd = 5;
        wcd = 5;
        ecd = 5;
        rcd = 5;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
