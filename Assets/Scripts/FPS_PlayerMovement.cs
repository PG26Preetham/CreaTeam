using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


public enum EMovementState
{
    Walking,
    Running,
    Crouch,
    Sliding,
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
    public float _slideSpeed;
    public float _wallRunSpeed;
    public float _speedIncreaseMultiplier;
    public float _slopeIncreaseMultiplier;

    public bool _bIsSliding;
    public bool _bIsWallRunning;

    private float _requiredMoveSpeed;
    private float _latRequiredMoveSpeed;

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
            _rb.AddForce(GetSlopeMovementDirection(_moveDirection) * _moveSpeed * 10, ForceMode.Force);

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
        if(_bIsWallRunning)
        {
            _movementState = EMovementState.WallRun;
            _requiredMoveSpeed = _wallRunSpeed;
        }

        else if(_bIsSliding)
        {
            _movementState = EMovementState.Sliding;

            if(IsOnSlope() && _rb.velocity.y <0.1f)
            {
                _requiredMoveSpeed = _slideSpeed;
            }
            else
            {
                _requiredMoveSpeed = _runSpeed;
            }
        }
        //Check if player is grounded and is pressing the sprint key
        else if(Input.GetKey(_crouchKey))
        {
            _movementState =EMovementState.Crouch;
            _requiredMoveSpeed = _crouchSpeed;
        }
        //Set Sprint
        else if(_bIsGrounded && Input.GetKey(_sprintKey))
        {
            _movementState = EMovementState.Running;
            _requiredMoveSpeed = _runSpeed;
        }


        //Set Walk
        else if(_bIsGrounded)
        {
             _movementState=EMovementState.Walking;
             _requiredMoveSpeed = _walkSpeed;
        }

        //Set InAir
        else
        {
            _movementState=EMovementState.Air;
        }


        if(Mathf.Abs(_requiredMoveSpeed - _latRequiredMoveSpeed) > 0.4f)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            _moveSpeed = _requiredMoveSpeed;
        }
        _latRequiredMoveSpeed = _requiredMoveSpeed;
    }


    public bool IsOnSlope()
    {
        if(Physics.Raycast(transform.position ,Vector3.down , out _slopeHit , _playerHeight*0.5f + 0.3f))
        {
            float floorAngle = Vector3.Angle(Vector3.up , _slopeHit.normal);
            return (floorAngle < _maxWalableSlopeAngle && floorAngle != 0);
        }
        return false;
    }

    public Vector3 GetSlopeMovementDirection(Vector3 Direction)
    {
        //_bIsGrounded = Physics.Raycast(transform.position, Vector3.ProjectOnPlane(Vector3.down , _slopeHit.normal).normalized, _playerHeight * 0.5f + 0.2f, _groundLayer);
        return Vector3.ProjectOnPlane(Direction , _slopeHit.normal).normalized;
    }
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(_requiredMoveSpeed - _moveSpeed);
        float startValue = _moveSpeed;

        while (time < difference)
        {
            _moveSpeed = Mathf.Lerp(startValue, _requiredMoveSpeed, time / difference);

            if (IsOnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * _speedIncreaseMultiplier * _slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * _speedIncreaseMultiplier;

            yield return null;
        }

        _moveSpeed = _requiredMoveSpeed;
    }
}
