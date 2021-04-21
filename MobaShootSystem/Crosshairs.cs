using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHightlightColor;
    Color originalDotColor;

    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    //瞄准到敌人时，瞄点自动变红
    public void DectectTarget(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHightlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
