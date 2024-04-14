using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


public enum EMovementState
{
    Walking,
    Running,
    Crouch,
    Air,
    WallRun
}

public class FPS_PlayerMovement : MonoBehaviour
{

    [Header("KeyBindings")]
    public KeyCode _jumpKey = KeyCode.Space;
    public KeyCode _sprintKey = KeyCode.LeftShift;
    public KeyCode _crouchKey = KeyCode.LeftControl;

    public EMovementState _movementState;

    [Header("Movement")]
    private float _moveSpeed;
    public float _walkSpeed;
    public float _runSpeed;

    [Header("Crouch")]
    public float _crouchSpeed;
    public float _crouchHeight;
    private float _walkHeight;

    [Header("SlopeHandling")]
    public float _maxWalableSlopeAngle;
    private RaycastHit _slopeHit;
    [SerializeField]
    private bool _bIsExitingSlope;


    public float _groundDrag;


    public float _jumpForce;
    public float _jumpCoolDown;
    public float _airMultiplier;
    bool _bReadyToJump;

    [Header("Ground Check")]
    public float _playerHeight;
    public LayerMask _groundLayer;
    [SerializeField]
    bool _bIsGrounded;

    public Transform _orientation;

    float HorizontalInput;
    float VerticalInput;

    Vector3 _moveDirection;

    Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _bReadyToJump = true;

        _walkHeight = transform.localScale.y;
    }
    private void Update()
    {
        
        //ground check
        _bIsGrounded = Physics.Raycast(transform.position, transform.up * -1, _playerHeight * 0.5f + 0.2f, _groundLayer);

        MyInput();
        StateHandler();
        SpeedControl();

        //Drag
        if(_bIsGrounded )
        {
            _rb.drag = _groundDrag;
        }
        else
        {
            _rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");



        if(Input.GetKey(_jumpKey) && _bReadyToJump && _bIsGrounded )
        {
            _bReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), _jumpCoolDown);
        }

        if(Input.GetKeyDown(_crouchKey))
        {
            transform.localScale =  new Vector3(transform.localScale.x , _crouchHeight, transform.localScale.z);
            _rb.AddForce(Vector3.down * 5f , ForceMode.Impulse);
        }

        if(Input.GetKeyUp(_crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, _walkHeight, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {
        _moveDirection = _orientation.forward * VerticalInput + _orientation.right * HorizontalInput;

        if(IsOnSlope() && !_bIsExitingSlope)
        {
            _rb.AddForce(GetSlopeMovementDirection() * _moveSpeed * 10, ForceMode.Force);

            if(_rb.velocity.y > 0)
            {
                _rb.AddForce(Vector3.down * 60f , ForceMode.Force);
            }
        }

        if(_bIsGrounded)
        {
            _rb.AddForce(_moveDirection * _moveSpeed * 10, ForceMode.Force);
        }
        else if(!_bIsGrounded)
        {
            _rb.AddForce(_moveDirection * _moveSpeed * 10 * _airMultiplier, ForceMode.Force);
        }

        _rb.useGravity = !IsOnSlope();
    }

    private void SpeedControl()
    {

        if(IsOnSlope() && !_bIsExitingSlope)
        {
            if(_rb.velocity.magnitude > _moveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 _currentVelocityGround = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            if (_currentVelocityGround.magnitude > _moveSpeed)
            {
                Vector3 _controlledVelocity = _currentVelocityGround.normalized * _moveSpeed;
                _rb.velocity = new Vector3(_controlledVelocity.x, _rb.velocity.y, _controlledVelocity.z);
            }
        }
        
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x , 0 ,_rb.velocity.z);
        _bIsExitingSlope = true;
        _rb.AddForce(transform.up * _jumpForce ,ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _bReadyToJump = true;
        _bIsExitingSlope = false;
    }


    private  void StateHandler()
    {
        //Check if player is grounded and is pressing the sprint key
        if(Input.GetKey(_crouchKey))
        {
            _movementState =EMovementState.Crouch;
            _moveSpeed = _crouchSpeed;
        }


        //Set Sprint
        else if(_bIsGrounded && Input.GetKey(_sprintKey))
        {
            _movementState = EMovementState.Running;
            _moveSpeed =_runSpeed;
        }


        //Set Walk
        else if(_bIsGrounded)
        {
             _movementState=EMovementState.Walking;
            _moveSpeed = _walkSpeed;
        }

        //Set InAir
        else
        {
            _movementState=EMovementState.Air;
        }
    }


    private bool IsOnSlope()
    {
        if(Physics.Raycast(transform.position ,Vector3.down , out _slopeHit , _playerHeight*0.5f + 0.3f))
        {
            float floorAngle = Vector3.Angle(Vector3.up , _slopeHit.normal);
            return (floorAngle < _maxWalableSlopeAngle && floorAngle != 0);
        }
        return false;
    }

    private Vector3 GetSlopeMovementDirection()
    {
        //_bIsGrounded = Physics.Raycast(transform.position, Vector3.ProjectOnPlane(Vector3.down , _slopeHit.normal).normalized, _playerHeight * 0.5f + 0.2f, _groundLayer);
        return Vector3.ProjectOnPlane(_moveDirection , _slopeHit.normal).normalized;
    }
}
