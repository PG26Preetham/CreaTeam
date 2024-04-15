using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


enum EWallRunSide
{
    Right,
    Left,
    None
}
public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask _wallLayer;
    public LayerMask _groundLayer;
    public float _wallRunForce;
    public float _maxWallRunTime;
    private float _wallRunTimer;

    [Header("WallJump")]
    public float _wallJumpUpForce;
    public float _wallJumpSideForce;
    public KeyCode _wallJumpKey = KeyCode.Space;

    public bool _bIsExitingWall;
    public float _exitWallTime;
    private float _exitWallTimer;

    [Header("Input")]
    private float _horizontalInput;
    private float _verticalInput;

    [Header("Detection")]
    public float _wallCheckDistance;
    public float _minJumpHeight;

    private RaycastHit _leftWallHit;
    private RaycastHit _rightWallHit;

    private bool _isLeft;
    private bool _isRight;

    private EWallRunSide _wallRunSide;

    private Rigidbody _playerRB;
    public Transform _orientation;
    private FPS_PlayerMovement _movement;

    [Header("Camera")]
    public PlayerCam _playerCam;
    // Start is called before the first frame update
    void Start()
    {
        _bIsExitingWall = false;
        _playerRB = GetComponent<Rigidbody>();
        _movement = GetComponent<FPS_PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
       // WallCheck();
        StateMachine();
    }
    private void FixedUpdate()
    {
        if(_movement._bIsWallRunning)
        {
            WallRunMovement();
        }
    }

    private bool WallCheck()
    {
        if(!_movement._bIsWallRunning )
        {
            _isRight = Physics.Raycast(transform.position, _orientation.right, out _rightWallHit, _wallCheckDistance, _wallLayer);
            _isLeft = Physics.Raycast(transform.position, -_orientation.right, out _leftWallHit, _wallCheckDistance, _wallLayer);
            if (_isRight)
            {
                _wallRunSide = EWallRunSide.Right;
                return true;
            }

            else if (_isLeft)
            {
                _wallRunSide = EWallRunSide.Left;
                return true;
            }
            else
            {
                _wallRunSide = EWallRunSide.None;
                return false;
            }
        }
        else
        {
            if(_wallRunSide == EWallRunSide.Left)
            {
                _isLeft = Physics.Raycast(transform.position, -_orientation.right, out _leftWallHit, _wallCheckDistance, _wallLayer);
                if(_isLeft)
                {
                    return true;
                }
               return false;
            }
            else if (_wallRunSide == EWallRunSide.Right)
            {
                _isRight = Physics.Raycast(transform.position, _orientation.right, out _rightWallHit, _wallCheckDistance, _wallLayer);
                if(_isRight)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        
    }

    private bool IsAboveHeight()
    {
        return !Physics.Raycast(transform.position,Vector3.down,_minJumpHeight,_groundLayer);
    }
    private bool CheckIfKeyDown()
    {
        if(_wallRunSide == EWallRunSide.Left && _horizontalInput < 0)
        {
            return true;
        }
        else if(_wallRunSide == EWallRunSide.Right && _horizontalInput > 0)
        {
            return true;
        }
        return false;
        
    }

    private void StateMachine()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");



        if(IsAboveHeight() && WallCheck() && CheckIfKeyDown() && !_bIsExitingWall)
        {
            if(!_movement._bIsWallRunning)
            {
                StartWallRun();
            }
            if(Input.GetKeyDown(_wallJumpKey))
            {
                WallJump();
            }
        }
        else if(_bIsExitingWall)
        {
            if(_movement._bIsWallRunning)
            {
                StopWallRun();

               
            }
            if (_exitWallTimer > 0)
            {
                _exitWallTimer -= Time.deltaTime;
            }
            if (_exitWallTimer <= 0)
            {
                _bIsExitingWall = false;
            }
        }
        else
        {
            if (_movement._bIsWallRunning)
            {
            StopWallRun();                
            }
        }
    }

    private void StartWallRun()
    {
        _movement._bIsWallRunning = true;

        _playerCam.DoFOV(90f);
        if(_isLeft)
        {
            _playerCam.DoTilt(-15.0f);
        }
        else if( _isRight)
        {
            _playerCam.DoTilt(15.0f);
        }
    }
    private void WallRunMovement()
    {
        _playerRB.useGravity = false;
        _playerRB.velocity = new Vector3(_playerRB.velocity.x, 0, _playerRB.velocity.z);
        Vector3 wallNormal = _isRight ? _rightWallHit.normal : _leftWallHit.normal;

        Vector3 wallForward =Vector3.Cross(wallNormal , transform.up);

        if((_orientation.forward - wallForward).magnitude > (_orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        _playerRB.AddForce(wallForward*_wallRunForce , ForceMode.Force);

        if((_isLeft && _horizontalInput > 0 ) || (_isRight && _horizontalInput < 0 ))
        {
            StopWallRun();
        }
    }
    private void StopWallRun()
    {
        _playerRB.useGravity = true;
        _movement._bIsWallRunning = false;
        _playerCam.DoTilt(0);
        _playerCam.DoFOV(80);
    }

    private void WallJump()
    {
        _bIsExitingWall = true;
        _exitWallTimer = _exitWallTime;
        Vector3 wallNormal = _isRight ? _rightWallHit.normal : _leftWallHit.normal;

        Vector3 JumpDirectionForce= transform.up * _wallJumpUpForce + wallNormal * _wallJumpSideForce;

        _playerRB.velocity = new Vector3(_playerRB.velocity.x, 0, _playerRB.velocity.z);
        _playerRB.AddForce(JumpDirectionForce, ForceMode.Impulse);
    }
}
