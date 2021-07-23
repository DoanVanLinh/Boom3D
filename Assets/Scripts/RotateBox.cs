using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBox : MonoBehaviour
{
    private float smooth = GameManager.Instance.SmoothRotateBox;
    private void Update()
    {
        Rotate();
    }
    private void Rotate()
    {
        if (Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * smooth * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * smooth * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up, -rotX,Space.World);
            transform.Rotate(Vector3.right, rotY,Space.World);
        }
    }

}
