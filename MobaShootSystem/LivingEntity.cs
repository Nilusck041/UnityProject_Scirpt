using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageble
{
    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

     protected virtual void Start()//virtual
    {
        health = startingHealth;
    }

    public virtual void TakeHit(float damage,Vector3 hitPoint, Vector3 hitDirection)
    {
        //Do some staff here with hit variable
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}
