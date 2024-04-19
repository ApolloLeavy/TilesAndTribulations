using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class GobBombExplosion : Projectile
{
    public AudioSource audioS;
    public AudioClip expS;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        audioS = GetComponent<AudioSource>();
        StartCoroutine(Timer());
    }

    public override IEnumerator SlowUpdate()
    {

        yield return new WaitForSeconds(.1f);

    }
    public IEnumerator Timer()
    {
        audioS.clip = expS;
        audioS.Play();
        yield return new WaitForSeconds(1);
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