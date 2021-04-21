using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }


    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]//evert time value change, push out to all of the clients
             //每次改变所有玩家都能看
    private int currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    [SyncVar]
    public string username = "Loading...";

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameobjectOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool isFirstSetup = true;

    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            //Switch Cameras
            if (isLocalPlayer)
            {
                GameManager.instance.SetSceneCameraActive(false);
                GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
            }
        }

        

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            isFirstSetup = false;
        }
  
        SetDefaults();
    }

    //void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(9999);
    //    }
    //}

    [ClientRpc]
    public void RpcTakeDamage(int _amount,string _sourceID)
    {
        if (isDead) return;

        currentHealth -= _amount; //SyncVar实时更新玩家health

        Debug.Log(transform.name + " now has " + currentHealth + "health");

        if(currentHealth<=0)
        {
            Die(_sourceID);
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallBack.Invoke(username, sourcePlayer.username);
        }

        deaths++;

        //Disable component
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //Disable gameobject
        for (int i = 0; i < disableGameobjectOnDeath.Length; i++)
        {
            disableGameobjectOnDeath[i].SetActive(false);
        }

        //Disable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //Spawn a death effect
         GameObject _gfxIns = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        //Switch Cameras
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + "is dead!");

        //Call RESPWAN METHOD
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSetting.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer(); 

        Debug.Log(transform.name + " Respawn!");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        //Set component active
        for (int i= 0;i<disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable the component
        for (int i = 0; i < disableGameobjectOnDeath.Length; i++)
        {
            disableGameobjectOnDeath[i].SetActive(true);
        }

        //Enable the Collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        //Create Spawn Effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

    }
 
}
