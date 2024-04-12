using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Chestplate : Projectile
{
    public Player player;
    public Animator myAnim;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
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
    void Start()
    {
        myAnim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
