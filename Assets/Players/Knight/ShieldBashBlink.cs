using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class ShieldBashBlink : Projectile
{
    public Player p;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

        p = GameObject.FindGameObjectWithTag("Knight").GetComponent<Player>();
        p.myRig.position = transform.position;
        StartCoroutine(Timer());

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(1.5f);
        MyCore.NetDestroyObject(NetId);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
