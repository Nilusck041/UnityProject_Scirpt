using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test02 : MonoBehaviour
{
    public Transform objectToPlace;
    public Camera gameCamera;

    // Update is called once per frame
    void Update()
    {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition); //camera中一个非常有帮助的函数，存鼠标的实时位置
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo))
        {
            objectToPlace.position = hitInfo.point;
            objectToPlace.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);//hitInfo.normal可以计算cube的rotation
                                                                            //该代码目的是让cube在移动过程中，随着地形起伏而变化旋转角度
        }
    }
}
