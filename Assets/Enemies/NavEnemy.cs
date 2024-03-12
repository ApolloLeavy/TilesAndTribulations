using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NETWORK_ENGINE;
using UnityEngine.AI;

public class NavEnemy : NetworkComponent
{
    public NavMeshAgent agent;
    public Transform flagA;
    public Transform flagB;
    public Rigidbody myRig;
    public GameObject player;
    public bool isA = true;
    public float speed;

    public override void HandleMessage(string flag, string value)
    {
        if (IsServer)
        {
            if (flag == "MV")
            {

            }

        }
        if (IsClient)
        {


        }
    }

    public override void NetworkedStart()
    {
        speed = 8f;
        
    }

    public override IEnumerator SlowUpdate()
    {
        if (IsConnected)
        {
            while (IsServer)
            {
                if (IsDirty)
                {
                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        if(IsServer)
        {
            agent.destination = flagA.position;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (IsServer)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        if (isA)
                        {
                            IsDirty = true;
                            isA = false;
                            agent.destination = flagB.position;
                        }
                        else 
                        {
                            IsDirty = true;
                            isA = true;
                            agent.destination = flagA.position;
                        }
                    }
                }
            }
            player = GameObject.FindGameObjectWithTag("Player");
            if((player.transform.position-transform.position).magnitude <= 2f)
            {
                IsDirty = true;
                agent.destination = player.transform.position;
            }
            else
            {
                if (isA)
                    agent.destination = flagA.position;
                else
                    agent.destination = flagB.position;
            }
        }

    }
}

