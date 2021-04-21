using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Playershoot:No camera reference");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.IsOn)
        {
            CancelInvoke("Shoot");
            return;
        }

        if(currentWeapon.bullets < currentWeapon.maxBullet)
        {
            if (Input.GetButtonDown("Reload"))
            {
                weaponManager.Reload();
                return;
            }
        }
        
        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    //called when a player shoot
    [Command]        //executed only on server
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //call on all client when we do a shoot effect
    [ClientRpc]     
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    //is called on the server when we hit something 
    //Takes in the hit point and the normal of the surface
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    //is called on all clients
    //here we can spawn in cool effect
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject) Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab,
                                                                  _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client]
    void Shoot()
    {
        if(!isLocalPlayer  ||  weaponManager.isReloading)
        {
            return;
        }

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullets--;

        //We are shooting, call the  OnShoot method on the server 
        CmdOnShoot();

        RaycastHit _hit;
        if(Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit, currentWeapon.range,mask))
        {
            if(_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage,transform.name);
            }

            //We hit something, call the OnHit  method on the server
            CmdOnHit(_hit.point, _hit.normal);
        }

        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }

    }

    [Command]
    void CmdPlayerShot(string _playerID,int _damage,string _sourseID)
    {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage,_sourseID);
    }
}
