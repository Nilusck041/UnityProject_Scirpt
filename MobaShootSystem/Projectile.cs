using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//该脚本为子弹或炮弹系统
public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public Color trailColor;
    float speed = 10;
    float damage = 1;

    float lifeTime = 3;
    float skinWidth = .1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollision = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollision.Length > 0)
        {
            OnHitObject(initialCollision[0], transform.position);
        }

        //GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);

    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

 

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageble damagebleObject = c.GetComponent<IDamageble>();
        if (damagebleObject != null)
        {
            damagebleObject.TakeHit(damage,hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }

}
