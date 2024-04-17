using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Monster
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
        hp = 30;
        speed = 2;
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
        Vector2[] piece = tileLibrary[Random.Range(0, (tileLibrary.Count ))];
        PreviewAbility(piece, 26);

    }
}
