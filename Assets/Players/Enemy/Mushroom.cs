using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Monster
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
        hp = 25;
        speed = 3;
        acd = 1.5f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    public override void Attack()
    {
        base.Attack();
        GameObject o = MyCore.NetCreateObject(25, Owner, transform.position+new Vector3(lastInput.x,lastInput.x,0), Quaternion.identity);
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized * 3;
        o = MyCore.NetCreateObject(25, Owner, transform.position + new Vector3(lastInput.y, lastInput.y, 0), Quaternion.identity);
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized * 3;


    }
}
