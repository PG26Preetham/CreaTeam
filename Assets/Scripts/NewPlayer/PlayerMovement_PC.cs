using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_PC : MonoBehaviour
{
    private CharacterController _characterController;

    public float _speed = 12f;
    public float _airSpeedMultiply = 0.3f;
    public float _gravity = -9.8f;
    public Transform _groundCheck;
    public float _groundDistance = 0.4f;
    public LayerMask _groundLayer;

    public float _jumpHeight = 3f;

    private Vector3 _velocity;
    private bool _isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundLayer);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2;
        }

        float xValue = Input.GetAxis("Horizontal");
        float yValue = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xValue + transform.forward * yValue;
        if(_isGrounded)
        {
            _characterController.Move(move * _speed * Time.deltaTime);
        }
        else
        {
            _characterController.Move(move * _airSpeedMultiply * _speed * Time.deltaTime);
        }

       


        if(Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
