using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 7;
    public event System.Action OnPlayerDeath;

    float screenHalfWidthInWordUnits;

    void Start()
    {
        float halfPlayerWidth = transform.localScale.x / 2f;
        screenHalfWidthInWordUnits = Camera.main.aspect * Camera.main.orthographicSize + halfPlayerWidth;//自动计算当前屏幕大小,加上玩家自身的半径
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float velocity = inputX * speed;
        transform.Translate(Vector2.right * velocity * Time.deltaTime);

        if(transform.position.x < -screenHalfWidthInWordUnits)
        {
            transform.position = new Vector2(screenHalfWidthInWordUnits, transform.position.y);
        }
        if (transform.position.x > screenHalfWidthInWordUnits)
        {
            transform.position = new Vector2(-screenHalfWidthInWordUnits, transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D triggerCollider)
    {
        if(triggerCollider.tag == "Falling Block")
        {
            if(OnPlayerDeath != null)
            {
                OnPlayerDeath();
            }
            Destroy(gameObject);
        }
    }
}
