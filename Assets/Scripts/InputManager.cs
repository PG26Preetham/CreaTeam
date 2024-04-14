using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  enum EInputMap
{
    World,
    UI
}


public class InputManager : MonoBehaviour
{


    public static InputManager Instance;

    public PlayerInputAction InputActions;

    [SerializeField]
    private EInputMap InputMap;

    private void Awake()
    {
        Instance = this;
        InputActions = new PlayerInputAction();
    }


    void Start()
    {
        ChangeInputMap(InputMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeInputMap(EInputMap inputMap)
    {
        InputMap = inputMap;
        switch(inputMap)
        {
            case EInputMap.World:
                WorldInputMap.Instance.InputActions.Enable();
                break;
            case EInputMap.UI: 
                break;
        }
    }
}
