using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyeball : Monster
{
    // Start is called before the first frame update
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hp = 15;
        speed = 2;
        acd = 2;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    public override void Attack()
    {
        base.Attack();
        GameObject o = MyCore.NetCreateObject(23, Owner, transform.position, Quaternion.identity);
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized *2;

    }
}
