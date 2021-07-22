using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotate : MonoBehaviour
{
    private Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        transform.LookAt(mainCam.transform);
    }
}
