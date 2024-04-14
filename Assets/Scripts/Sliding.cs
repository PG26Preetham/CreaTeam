using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("reference")]
    public Transform _orientation;
    public Transform _playerObject;
    private Rigidbody _playerRB;
    private FPS_PlayerMovement _playerMovement;


    [Header("Sliding")]
    public float _maxTimeToSlide;
    public float _slideForce;
    private float _slideTimer;

    public float _slideScale;
    private float _NormalScale;

  //  private bool _bIsSliding;

    [Header("Input")]
    public KeyCode _slideKey;
    private float _HorizontalInput;
    private float _VerticalInput;
    // Start is called before the first frame update
    void Start()
    {
        _playerRB = GetComponent<Rigidbody>();
        _playerMovement = GetComponent<FPS_PlayerMovement>();

        _playerMovement._bIsSliding = false;

        _NormalScale = _playerObject.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        _HorizontalInput = Input.GetAxisRaw("Horizontal");
        _VerticalInput = Input.GetAxisRaw("Vertical");


        if(Input.GetKeyDown(_slideKey) && (_HorizontalInput != 0  ||  _VerticalInput != 0) )
        {
            StartSlide();
        }
        if(Input.GetKeyUp(_slideKey) && _playerMovement._bIsSliding)
        {
            StopSlide();
        }
    }
    private void FixedUpdate()
    {
        if(_playerMovement._bIsSliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        Debug.Log("Sliding");
        _playerMovement._bIsSliding = true;

        _playerObject.localScale = new Vector3(_playerObject.localScale.x , _slideScale, _playerObject.localScale.z);
        _playerRB.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = _maxTimeToSlide;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = _orientation.forward * _VerticalInput + _orientation.right * _HorizontalInput;

        //slide not on slode
       if(! _playerMovement.IsOnSlope() )
        {

        _playerRB.AddForce(inputDirection.normalized * _slideForce, ForceMode.Force);
        _slideTimer -= Time.deltaTime;
        }
       else
        {
            _playerRB.AddForce(_playerMovement.GetSlopeMovementDirection(inputDirection) * _slideForce, ForceMode.Force);
        }

        if(_slideTimer < 0 )
        {
            StopSlide() ;
        }

    }

    private void StopSlide()
    {
        _playerMovement._bIsSliding = false;

        _playerObject.localScale = new Vector3(_playerObject.localScale.x,_NormalScale, _playerObject.localScale.z);
        
    }
}
