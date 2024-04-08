using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Player : NetworkComponent
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
                    
                
            }
        }
        if(flag == "FLIP")
        {
            if (IsServer)
            {
                isFlipped = !isFlipped;
                SendUpdate("FLIP", isFlipped.ToString());
            }
            if (IsLocalPlayer)
            {
                isFlipped = bool.Parse(value);
            }
        }
       
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
                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);

        }
    }
    public void OnPlace(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer && ev.performed)
        {
            Debug.Log("Place");
            SendCommand("PLACE", canPlace.ToString());
            
        }
    }
    public void OnMove(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer)
        {
            if (ev.started || ev.performed)
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
    public void OnFlip(InputAction.CallbackContext ev)
    {
        if (ev.started)
            SendCommand("FLIP","");
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
                yield return new WaitForSeconds(dir[i].magnitude / speed);
                
            }
            myRig.velocity = new Vector3(0, 0, 0);
            canPlace = true;
            SendUpdate("PLACE", canPlace.ToString());
        }
    }
    public void PreviewMove(Vector2[] dir)
    {
        
            foreach(GameObject o in indicatorList)
            {
                GameObject.Destroy(o);
            }
            indicatorList.Clear();
            for (int i = 0; i < dir.Length; i++)
            {

                if (lastInput.y < 0)
                {
                    point.position += new Vector3(-dir[i].x * (dir[i].magnitude / speed), -dir[i].y * (dir[i].magnitude / speed), 0);
                    if (isFlipped)
                        point.position += new Vector3(dir[i].x * (dir[i].magnitude / speed), -dir[i].y * (dir[i].magnitude / speed), 0);
                }
                else if (lastInput.x > 0)
                {
                    point.position += new Vector3(dir[i].y * (dir[i].magnitude / speed), -dir[i].x * (dir[i].magnitude / speed), 0);
                    if (isFlipped)
                        point.position += new Vector3(dir[i].y * (dir[i].magnitude / speed), dir[i].x * (dir[i].magnitude / speed), 0);
                }
                else if (lastInput.x < 0)
                {
                    point.position += new Vector3(-dir[i].y * (dir[i].magnitude / speed), dir[i].x * (dir[i].magnitude / speed), 0);
                    if (isFlipped)
                        point.position += new Vector3(-dir[i].y * (dir[i].magnitude / speed), -dir[i].x * (dir[i].magnitude / speed), 0);
                }
                else
                {
                    point.position += new Vector3(dir[i].x* (dir[i].magnitude / speed), dir[i].y* (dir[i].magnitude / speed), 0);
                    if (isFlipped)
                        point.position += new Vector3(-dir[i].x * (dir[i].magnitude / speed), dir[i].y * (dir[i].magnitude / speed), 0);
                }
                indicatorList.Add(GameObject.Instantiate(previewBlock,point.position,Quaternion.identity));
                
            }
            point.position = transform.position;
            
        
    }

    // Start is called before the first frame update
    public void Start()
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
        tiles.Add(new Vector2[] { new Vector2(0, 1),new Vector2(0, 1),new Vector2(0, 1),new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0) });
        activeTile = 0;
        isFlipped = false;
        point = transform.GetChild(0);
        List<GameObject> indicatorList = new List<GameObject>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (IsLocalPlayer)
        {

            Camera.main.transform.position = Vector3.Lerp(transform.position + new Vector3(0,0, -9), myRig.position, speed / 2 * Time.deltaTime);

        }
    }
}
