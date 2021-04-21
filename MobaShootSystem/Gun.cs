using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region 枪支属性
    public enum FireMode{Auto,Burst,Single};
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = .3f;
    #endregion

    #region  后坐力参数
    [Header("##Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3,5);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    #endregion

    #region  枪支特效
    [Header("##Effect")]
    public Transform shell;
    public Transform shellEjection;

    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    #endregion

    bool triggerReleasedSinceLastShot;//点击是否释放
    int shotsRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

   
    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    private void LateUpdate()
    {
        //animate recoil   后坐力动画
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles = Vector3.left * recoilAngle;

        //reload
        if(!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    void Shoot() //射击System
    {
        if(!isReloading && Time.time>nextShotTime && projectilesRemainingInMag > 0)
        {
            if(fireMode == FireMode.Burst)//三连发模式
            {
                if (shotsRemainingInBurst == 0) return;
                shotsRemainingInBurst--;
            }else if(fireMode == FireMode.Single)//单发模式
            {
                if (!triggerReleasedSinceLastShot) return;
            }//默认else为全自动模式

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if(projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }  
    }

    #region  #####装弹System设置和动画
    public void Reload()
    {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }
    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1 / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }


        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }
    #endregion

    public void Aim(Vector3 aimPoint)
    {
        transform.LookAt(aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;//鼠标点下去了
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;//鼠标没有点下去
        shotsRemainingInBurst = burstCount;
    }
}
