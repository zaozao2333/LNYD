using System.Collections;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target; // 拖拽目标物体到此处
    public Vector3 offset = new Vector3(0, 0, -10); 

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}