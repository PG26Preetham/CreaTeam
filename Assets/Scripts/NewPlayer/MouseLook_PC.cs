using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MouseLook_PC : MonoBehaviour
{
    // Start is called before the first frame update

    public float _mouseSens = 100f;
    private float xRotate = 0;
    public Transform _player;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSens * Time.deltaTime;


        xRotate -= mouseY;
        xRotate = Math.Clamp(xRotate, -89, 89);
        transform.localRotation = Quaternion.Euler(xRotate, 0, 0);


        _player.Rotate(Vector3.up * mouseX);
        
    }
}
