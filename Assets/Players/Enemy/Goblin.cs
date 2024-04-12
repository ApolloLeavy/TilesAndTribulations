using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hp = 20;
        speed = 3;
        acd = 1;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    public override void Attack()
    {
        base.Attack();
        GameObject o = MyCore.NetCreateObject(24, Owner, transform.position, Quaternion.identity);
        o.GetComponent<GobBomb>().lastInput = lastInput;
    }
}
