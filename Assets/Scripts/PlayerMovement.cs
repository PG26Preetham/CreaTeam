using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    WorldInputMap m_InputMap;

    private Rigidbody Rb;

    [SerializeField]
    private float Speed = 10;

    [SerializeField]
    private float JumpForce = 300f;

    [SerializeField]
    private Vector3 MoveDirection;


    
    void Start()
    {
        m_InputMap = WorldInputMap.Instance;
        Rb = GetComponent<Rigidbody>();

        m_InputMap.OnJumpAction += Jump;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVectorDir = m_InputMap.GetMoveDirection();

        MoveDirection = new Vector3(inputVectorDir.x , 0 , inputVectorDir.y) * Speed;

        Debug.Log(m_InputMap.GetLookAxis());

    }

    private void FixedUpdate()
    {
        Rb.velocity = new Vector3(MoveDirection.x  , Rb.velocity.y , MoveDirection.z );
    }

    private void Jump(object Sender , System.EventArgs args)
    {
        Rb.AddForce(new Vector3(0, JumpForce , 0) , ForceMode.Impulse);
    }
}
