using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Teleport : Projectile
{

    public Player p;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        p = GameObject.FindGameObjectWithTag("Wizard").GetComponent<Player>();
        p.myRig.position = transform.position;
        StartCoroutine(Timer());
        StartCoroutine(prevUp());
    }

    public override IEnumerator SlowUpdate()
    {
        
        yield return new WaitForSeconds(.1f);

    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        MyCore.NetDestroyObject(NetId);
    }
    public IEnumerator prevUp()
    {
        yield return new WaitForSeconds(.2f);
        if(p.activeTile != -1)
            p.PreviewMove(p.tileLibrary[p.tiles[p.activeTile]]);
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
