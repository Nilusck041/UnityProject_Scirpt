using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controllor;
    GunController gunController;

    protected override void Start()
    {
        base.Start();     
    }

    private void Awake()
    {
        controllor = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }
   
    void Update()
    {
        //玩家移动Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controllor.Move(moveVelocity);

        #region  #########鼠标光标系统(look input)
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);//鼠标光标移动，有点像瞄准系统记录你在看的位置
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);//射线ray投射到plane上
        float rayDistance;

        //获得鼠标光标位置
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point,Color.red); 
            controllor.lookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DectectTarget(ray);
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).magnitude > 1)
            {
                gunController.Aim(point);
            }
            
        };
        #endregion

        //Weapon Input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        //玩家掉出地图外，死亡
        if(transform.position.y < -10)
        {
            TakeDamage(health);
        }
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }

}
