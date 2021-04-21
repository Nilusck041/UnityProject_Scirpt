using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    //敌人的状态
    public enum State{Idle,Chasing,Attacking};
    State currentState;

    public ParticleSystem deathEffect;
    public static event System.Action OnDeathStatic;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttack = 1;
    float damage = 1; 

    float nextAttackTime;
    float myCollisionRadius;//玩家角色半径
    float targetCollisionRadius;//敌人半径

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start()
    {
        base.Start();
        

        if(hasTarget)
        {
            currentState = State.Chasing;

            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;

        if(hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        deathEffect.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if(damage >= health)
        {
            if(OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject
                ,deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath()//定义玩家死亡后AI会发生什么的函数
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if(hasTarget)
        {
            //每一帧都要计算攻击cd时间
            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttack;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }   

    }

    IEnumerator Attack()//攻击
    {
        //我们不想要攻击和寻路共同进行，所以分几个状态separate开来，当攻击时不寻路
        currentState = State.Attacking;
        pathfinder.enabled = false;

        //敌人攻击方式 做成扑过来咬，需要敌我的角色半径作为参数，用v3存
        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);
        

        float attackSpeed = 3;
        float percent = 0;//判断攻击cd的中间参数

        skinMaterial.color = Color.red;//敌人咬的时候变色
        bool hasAppliedDamage = false;

        while(percent <= 1)
        {
            if(percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;//咬完变回来

        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()//AI寻路
    {
        float refreshRate = .25f;

        while(hasTarget)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }           
            yield return new WaitForSeconds(refreshRate);

        }
    }
}
