using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NETWORK_ENGINE;
public class Monster : NetworkComponent
{
    public int hp;
    public Player player;
    public NavMeshAgent agent;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
