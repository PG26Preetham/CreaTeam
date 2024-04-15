using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public float _senseX;
    public float _senseY;

    public Transform _playerOrientation;
    public Transform _camHolder;

    float _xRotation;
    float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Getting mouse inputs
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _senseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _senseY;

        _yRotation += mouseX;
        _xRotation -= mouseY;

        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _camHolder.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        _playerOrientation.rotation = Quaternion.Euler(0 , _yRotation, 0);
    }

    public void DoFOV(float endFOV)
    {
        GetComponent<Camera>().DOFieldOfView(endFOV, 0.25f);
    }

    public void DoTilt(float endTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, endTilt), 0.25f);
    }
}
