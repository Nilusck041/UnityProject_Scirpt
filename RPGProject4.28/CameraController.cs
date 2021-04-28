using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    #region  #####镜头设置变量   
    public Vector3 offset;
    public float zoomSpeed = 4f;/*相机缩放变量*/
    public float minZoom = 5f;
    public float maxZoom = 15f;

    public float pitch = 2f;

    public float yawSpeed = 100f;/*相机旋转变量*/

    private float currentZoom = 10f;
    private float currentYaw = 0f;
    #endregion

    private void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        currentYaw -=  Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
    }

    private void LateUpdate()
    {
        /*相机跟随*/
        transform.position = target.position - offset * currentZoom;
        transform.LookAt(target.position + Vector3.up * pitch);//target在玩家底部，玩家高2f，看向时加两个高度单位

        /*相机旋转*/
        transform.RotateAround(target.position, Vector3.up, currentYaw);
    }
}
