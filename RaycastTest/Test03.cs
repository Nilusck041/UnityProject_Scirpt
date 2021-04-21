using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test03 : MonoBehaviour
{
    public LayerMask mask;

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;//2D跟3D一个不同点在于：3D射线射出的时候直接穿过源物体的box collider
                                                 //而2D不会穿过，这就导致射线从源物体内部射出，但被源box collider挡住
                                                 //因此在这个类的这个变量要设置为false
    }

    // Update is called once per frame
    void Update()
    {
       RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right,100,mask,0f,1f);
        if(hitInfo.collider != null)
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red, 100);
        }else
        {
            Debug.DrawLine(transform.position, transform.position + transform.right * 100, Color.green);
        }
    }
}
