using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public abstract class Player : NetworkComponent
{
    public int hp;
    public int tileCount;
    public int maxTiles;
    public int tileGainSec;
    public bool isGenerating;
    public int qcd;
    public int wcd;
    public int ecd;
    public int rcd;
    public float acd;
    
    public Rigidbody myRig;
    public float speed;
    public Vector2 lastInput;
    public bool canPlace;
    public bool canAttack;
    public bool canQ;
    public bool canW;
    public bool canE;
    public bool canR;
    public List<Vector2[]> tiles;
    public List<Vector2[]> tileLibrary; 
    public int activeTile;
    public bool isInvincible;
    public int kills;
    public int deaths;
    public int assists;
    public bool isDead;
    public float deathTimer;
    public GameObject npm;
    public bool isFlipped;
    public Transform point;
    public GameObject previewBlock;
    public List<GameObject> indicatorList;
    public override void HandleMessage(string flag, string value)
    {
        
        if (flag == "MV")
        {
            if (IsServer && canPlace)
            {
                Debug.Log("Input");
                string[] tmp = value.Split(',');
                lastInput = new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1]));
                IsDirty = true;
                if ((lastInput.x > 0 || lastInput.x < 0) && (lastInput.y > 0 || lastInput.y < 0))
                {
                    lastInput.x = 0;
                    lastInput.y = 1;
                }
                SendUpdate("MV", value);
            }
            if (IsClient)
            {
                string[] tmp = value.Split(',');
                lastInput = new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1]));
                if ((lastInput.x > 0 || lastInput.x < 0) && (lastInput.y > 0 || lastInput.y < 0))
                {
                    lastInput.x = 0;
                    lastInput.y = 1;
                }
                PreviewMove(tiles[activeTile]);

            }
        }
        if(flag == "PLACE")
        {
            if(IsServer)
            {
                if(canPlace)
                {
                    Debug.Log("Place");
                    canPlace = false;
                    StartCoroutine(Move(tiles[activeTile]));
                    SendUpdate("PLACE", canPlace.ToString());
                }
                
            }
            if(IsLocalPlayer)
            {
                canPlace = bool.Parse(value);
                if(canPlace)
                    PreviewMove(tiles[activeTile]);

            }
        }
        if (flag == "ATTACK")
        {
            if (IsServer)
            {
                if (canAttack)
                {
                    Debug.Log("Attack");
                    StartCoroutine(Attack());
                    canAttack = false;
                    SendUpdate("ATTACK", canAttack.ToString());
                }

            }
            if (IsLocalPlayer)
            {
                canAttack = bool.Parse(value);
            }
        }
        if (flag == "FLIP")
        {
            if (IsServer)
            {
                isFlipped = !isFlipped;
                SendUpdate("FLIP", isFlipped.ToString());
            }
            if (IsLocalPlayer)
            {

                isFlipped = bool.Parse(value);
                PreviewMove(tiles[activeTile]);
            }
        }
       
    }
    public virtual IEnumerator Attack()
    {
        yield return new WaitForSeconds(acd);
        canAttack = true;
        SendUpdate("ATTACK", canAttack.ToString());
    }
    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        if (IsConnected)
        {
            while(IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("ATTACK", canAttack.ToString());
                    SendUpdate("MV", lastInput.x+","+lastInput.y);
                    SendUpdate("PLACE", canPlace.ToString());
                    SendUpdate("FLIP", isFlipped.ToString());
                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);

        }
    }
    public void OnAttack(InputAction.CallbackContext ev)
    {
        if (ev.started && IsLocalPlayer)
        {
            SendCommand("ATTACK", "");
        }
    }
    public void OnPlace(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer && ev.performed)
        {
            Debug.Log("Place");
            SendCommand("PLACE", "");
            
        }
    }
    public void OnFlip(InputAction.CallbackContext ev)
    {
        if (ev.started)
            SendCommand("FLIP","");
    }
    public void OnMove(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer)
        {
            if (ev.started)
            {
                Debug.Log("Input");
                Vector2 tempCmd = ev.ReadValue<Vector2>();
                SendCommand("MV", tempCmd.x + "," + tempCmd.y);

            }
            if (ev.canceled)
            {
                SendCommand("MV", lastInput.x + "," + lastInput.y);
            }
        }
    }
    public IEnumerator Move(Vector2[] dir)
    {
        if (IsServer)
        {
            Debug.Log("Move");
            for (int i = 0; i < dir.Length;i++)
            {
                
                if(lastInput.y <0)
                {
                    myRig.velocity = new Vector3(-dir[i].x, -dir[i].y, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(dir[i].x, -dir[i].y, 0);
                }
                else if (lastInput.x > 0)
                {
                    myRig.velocity = new Vector3(dir[i].y, -dir[i].x, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(dir[i].y, dir[i].x, 0);
                }
                else if(lastInput.x < 0)
                {
                    myRig.velocity = new Vector3(-dir[i].y, dir[i].x, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(-dir[i].y, -dir[i].x, 0);
                }
                else
                {
                    myRig.velocity = new Vector3(dir[i].x, dir[i].y, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(-dir[i].x, dir[i].y, 0);
                }
                myRig.velocity = myRig.velocity.normalized* speed;
                
                yield return new WaitForSecondsRealtime(1 / speed);
                
            }
            myRig.velocity = new Vector3(0, 0, 0);
            
            canPlace = true;
            
            SendUpdate("PLACE", canPlace.ToString());
        }
    }
    public void PreviewMove(Vector2[] dir)
    {

        foreach (GameObject o in indicatorList)
        {
            GameObject.Destroy(o);
        }
        indicatorList.Clear();
        for (int i = 0; i < dir.Length; i++)
        {

            if (lastInput.y < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].x * (dir[i].magnitude), -dir[i].y * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x > 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].y * (dir[i].magnitude), -dir[i].x * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].y * (dir[i].magnitude), dir[i].x * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].x * (dir[i].magnitude), dir[i].y * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            indicatorList.Add(GameObject.Instantiate(previewBlock, point.position, Quaternion.identity));

        }
        point.position = transform.position;


    }
    public void PreviewAbility(Vector2[] dir, int type)
    {
        for (int i = 0; i < dir.Length; i++)
        {
            if (lastInput.y < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].x * (dir[i].magnitude), -dir[i].y * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x > 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].y * (dir[i].magnitude), -dir[i].x * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].y * (dir[i].magnitude), dir[i].x * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].x * (dir[i].magnitude), dir[i].y * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            indicatorList.Add(MyCore.NetCreateObject(type, Owner, point.position, Quaternion.identity));
        }
        point.position = transform.position;
    }
    public void PreviewAbilityEnd(Vector2[] dir, int type)
    {
        for (int i = 0; i < dir.Length; i++)
        {
            if (lastInput.y < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].x * (dir[i].magnitude), -dir[i].y * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x > 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].y * (dir[i].magnitude), -dir[i].x * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].y * (dir[i].magnitude), dir[i].x * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].x * (dir[i].magnitude), dir[i].y * (dir[i].magnitude), 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
        }
        indicatorList.Add(MyCore.NetCreateObject(type, Owner, point.position, Quaternion.identity));
        point.position = transform.position;
    }
    public IEnumerator Q()
    {
        yield return new WaitForSeconds(qcd);
        canQ = true;
        SendUpdate("Q", canQ.ToString());
    }
    public IEnumerator W()
    {
        yield return new WaitForSeconds(wcd);
        canW = true;
        SendUpdate("W", canW.ToString());
    }
    public IEnumerator E()
    {
        yield return new WaitForSeconds(ecd);
        canE = true;
        SendUpdate("E", canE.ToString());
    }
    public IEnumerator R()
    {
        yield return new WaitForSeconds(rcd);
        canR = true;
        SendUpdate("R", canR.ToString());
    }
    public void OnQ(InputAction.CallbackContext ev)
    {
        if (ev.started && canQ)
            SendCommand("Q", "");
    }
    public void OnW(InputAction.CallbackContext ev)
    {
        if (ev.started && canW)
            SendCommand("W", "");
    }
    public void OnE(InputAction.CallbackContext ev)
    {
        if (ev.started && canQ)
            SendCommand("E", "");
    }
    public void OnR(InputAction.CallbackContext ev)
    {
        if (ev.started && canW)
            SendCommand("R", "");
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        myRig = GetComponent<Rigidbody>();
        maxTiles = 5;
        tileCount = 0;
        tileGainSec = 2;
        canAttack = true;
        canPlace = true;
        canQ = true;
        canW = true;
        canE = true;
        canR = true;
        tiles = new List<Vector2[]>();
        tiles.Add(new Vector2[] { new Vector2(0, 1),new Vector2(0, 1),new Vector2(0, 1),new Vector2(1, 0)});
        activeTile = 0;
        isFlipped = false;
        point = transform.GetChild(0);
        List<GameObject> indicatorList = new List<GameObject>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (IsLocalPlayer)
        {

            Camera.main.transform.position = Vector3.Lerp(transform.position + new Vector3(0,0, -9), myRig.position, speed / 2 * Time.deltaTime);

        }
    }
}
