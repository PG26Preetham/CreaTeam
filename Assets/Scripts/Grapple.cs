using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Header("Reference")]
    private FPS_PlayerMovement _playerMovement;
    public Transform _playerCam;
    public Transform _grappleStartLocation;
    public LayerMask _grappleLayer;

    public LineRenderer _GrappleLineRender;


    [Header("Grappling")]
    public float _maxGrappleDistance;
    public float _grappleDelayTime;

    public float _overShootYAxis;


    private Vector3 _grapplePoint;

    private bool _bIsGrappled;

    [Header("CoolDown")]
    public float _coolDown;
    private float _cooldownTimer;

    [Header("Input")]
    public KeyCode _grappleKey = KeyCode.Mouse1;

    private bool _bIsGrappling;
    // Start is called before the first frame update
    void Start()
    {
        _playerMovement = GetComponent<FPS_PlayerMovement>();
        _bIsGrappled =false;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(_grappleKey) && !_bIsGrappled)
        {
            StartGrapple();
        }
        else if(Input.GetKeyDown(_grappleKey) && _bIsGrappling) 
        {
            GrappleMovement();
        }
        if(_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }
    private void LateUpdate()
    {
        if (_bIsGrappling)
            _GrappleLineRender.SetPosition(0, _grappleStartLocation.position);
    }

    private void StartGrapple()
    {
        if (_cooldownTimer > 0) return;
        if (_bIsGrappled) return;

        _bIsGrappling = true;

        RaycastHit hit;
        if(Physics.Raycast(_playerCam.position, _playerCam.forward, out hit,_maxGrappleDistance,_grappleLayer))
        {
            _grapplePoint = hit.point;
            _playerMovement._bIsFrozen = true;
            Invoke(nameof(PrimeGrappplePoint), _grappleDelayTime);
        }
        else
        {
            _grapplePoint = _playerCam.position+ _playerCam.forward * _maxGrappleDistance;

            Invoke(nameof(StopGrapple), _grappleDelayTime);
        }
        _GrappleLineRender.enabled = true;
        _GrappleLineRender.SetPosition(1,_grapplePoint);
    }

    private void PrimeGrappplePoint()
    {
        _bIsGrappled = true;
        _playerMovement._bIsFrozen = false;

        
    }
    private void GrappleMovement()
    {
        Debug.Log("Grappling");
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = _grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + _overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = _overShootYAxis;

        _playerMovement.JumpToPosition(_grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        _bIsGrappled = false;
        _bIsGrappling = false;

        _playerMovement._bIsFrozen= false;
        _cooldownTimer = _coolDown;
        _GrappleLineRender.enabled = false;
    }
}
