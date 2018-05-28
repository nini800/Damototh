using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_Controller : MonoBehaviour
{
    ControllerButton sprintButton = ControllerButton.LeftClick;
    KeyCode sprintKey = KeyCode.LeftShift;

    ControllerButton dashButton = ControllerButton.RightPadLeft;
    KeyCode dashKey = KeyCode.LeftControl;

    ControllerButton jumpButton = ControllerButton.RightPadDown;
    KeyCode jumpKey = KeyCode.Space;

    ControllerButton interactButton = ControllerButton.RightPadUp;
    KeyCode interactKey = KeyCode.E;
    ControllerAxis interactAxis = ControllerAxis.leftStickHorizontal;

    ControllerButton lightAttackButton = ControllerButton.RightBumper;
    ControllerButton heavyAttackButton = ControllerButton.RightTrigger;

    ControllerButton lockButton = ControllerButton.LeftTrigger;
    KeyCode lockKey = KeyCode.A;


    [SerializeField] int controllerId = 0;
    [SerializeField] ControllerType controllerType = ControllerType.Keyboard;

    p_Base linkedBase;
    p_PlayerBeing linkedBeing;

    float oldMouseX = 0, oldMouseY = 0;

    protected void Awake()
    {
        linkedBase = GetComponent<p_Base>();
        linkedBeing = GetComponent<p_PlayerBeing>();
    }

    protected void Update()
    {
        InputMaxPressTimeHandler();
        InputMemoryHandler();
    }

    protected void LateUpdate()
    {
        oldMouseX = Input.GetAxis("Mouse X");
        oldMouseY = Input.GetAxis("Mouse Y");
    }

    const float AUTO_PRESS_TIME = 0.1f;
    float lightAttackPressTime = -Mathf.Infinity, heavyAttackPressTime = -Mathf.Infinity;
    void InputMaxPressTimeHandler()
    {
        if (LightAttackDown)
            lightAttackPressTime = Time.time;
        else if (needResetLight)
            lightAttackPressTime = -Mathf.Infinity;

        if (HeavyAttackDown)
            heavyAttackPressTime = Time.time;
        else if (needResetHeavy)
            heavyAttackPressTime = -Mathf.Infinity;

    }

    const float MEMORY_TIME = 0.2f;
    float lastLightAttackTime = -Mathf.Infinity, lastHeavyAttackTime = -Mathf.Infinity;
    bool needResetLight = false, needResetHeavy = false;
    void InputMemoryHandler()
    {
        if (needResetLight)
        {
            needResetLight = false;
            lastLightAttackTime = -Mathf.Infinity;
        }
        if (needResetHeavy)
        {
            needResetHeavy = false;
            lastHeavyAttackTime = -Mathf.Infinity;
        }

        if (LightAttackWithoutMemory)
        {
            lastLightAttackTime = Time.time;
        }
        if (HeavyAttackWithoutMemory)
            lastHeavyAttackTime = Time.time;
    }

    public Vector2 MoveInput
    {
        get
        {
            Vector2 input = Vector2.zero;

            if (controllerType == ControllerType.Keyboard)
            {
                if (Input.GetKey(KeyCode.D))
                    input.x++;
                if (Input.GetKey(KeyCode.Q))
                    input.x--;
                if (Input.GetKey(KeyCode.Z))
                    input.y++;
                if (Input.GetKey(KeyCode.S))
                    input.y--;
            }
            else
            {
                input.x = Input_NG.GetControllerAxis(ControllerAxis.leftStickHorizontal, controllerId, controllerType);
                input.y = Input_NG.GetControllerAxis(ControllerAxis.leftStickVertical, controllerId, controllerType);
            }

            if (input.sqrMagnitude>1)
                input.Normalize();

            return Utilities.DeadzoneVector(input, 0.15f, 0.05f, 1.25f);
        }
    }
    public Vector2 ViewInput
    {
        get
        {
            Vector2 input = Vector2.zero;

            if (controllerType == ControllerType.Keyboard)
            {
                input.x = Input.GetAxis("Mouse X");
                input.y = Input.GetAxis("Mouse Y");
            }
            else
            {
                input.x = Input_NG.GetControllerAxis(ControllerAxis.rightStickHorizontal, controllerId, controllerType);
                input.y = Input_NG.GetControllerAxis(ControllerAxis.rightStickVertical, controllerId, controllerType);

                input = Utilities.DeadzoneVector(input, 0.05f, 0.05f, 1.3f);
            }

            return input * Utilities.Delta;
        }
    }

    public bool Sprint
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetKeyDown(sprintKey);
            else
                return Input_NG.GetControllerButton(sprintButton, controllerId, 1, controllerType);
        }
    }
    public bool Dash
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetKeyDown(dashKey);
            else
                return Input_NG.GetControllerButton(dashButton, controllerId, 1, controllerType);
        }
    }
    public bool Jump
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetKeyDown(jumpKey);
            else
                return Input_NG.GetControllerButton(jumpButton, controllerId, 1, controllerType);
        }
    }

    public bool Interact
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetKeyDown(interactKey);
            else
                return Input_NG.GetControllerButton(interactButton, controllerId, 1, controllerType);
        }
    }
    public bool StopInteract
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetKeyUp(interactKey);
            else
                return Input_NG.GetControllerButton(interactButton, controllerId, 2, controllerType);
        }
    }
    public float InteractAxis
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetAxis("Mouse Y");
            else
                return Input_NG.GetControllerAxis(interactAxis, controllerId, controllerType);
        }
    }

    bool LightAttackDown
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetMouseButtonDown(0);
            else
                return Input_NG.GetControllerButton(lightAttackButton, controllerId, 1, controllerType);
        }
    }
    bool HeavyAttackDown
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetMouseButtonDown(1);
            else
                return Input_NG.GetControllerButton(heavyAttackButton, controllerId, 1, controllerType);
        }
    }
    bool LightAttackWithoutMemory
    {
        get
        {
            bool inTime = (lightAttackPressTime + AUTO_PRESS_TIME) > Time.time;
            bool autoPress = (lightAttackPressTime + AUTO_PRESS_TIME < Time.time && lightAttackPressTime != -Mathf.Infinity);

            if (autoPress)
                lightAttackPressTime = -Mathf.Infinity;

            if (controllerType == ControllerType.Keyboard)
                return (Input.GetMouseButtonUp(0) && inTime || autoPress);
            else
                return Input_NG.GetControllerButton(lightAttackButton, controllerId, 2, controllerType) && inTime || autoPress;
        }
    }
    bool HeavyAttackWithoutMemory
    {
        get
        {
            bool inTime = (heavyAttackPressTime + AUTO_PRESS_TIME) > Time.time;
            bool autoPress = (heavyAttackPressTime + AUTO_PRESS_TIME < Time.time && heavyAttackPressTime != -Mathf.Infinity);

            if (autoPress)
                heavyAttackPressTime = -Mathf.Infinity;

            if (controllerType == ControllerType.Keyboard)
                return Input.GetMouseButtonUp(1) && inTime || autoPress;
            else
                return Input_NG.GetControllerButton(heavyAttackButton, controllerId, 2, controllerType) && inTime || autoPress;
        }
    }
    public bool LightAttackWithMemory
    {
        get
        {
            if (lastLightAttackTime + MEMORY_TIME > Time.time)
            {
                return true;
            }
            return false;
        }
    }
    public bool HeavyAttackWithMemory
    {
        get
        {
            if (lastHeavyAttackTime + MEMORY_TIME > Time.time)
            {
                return true;
            }
            return false;
        }
    }
    public bool LightAttack
    {
        get
        {
            if (LightAttackWithMemory && !HeavyAttackWithMemory)
                return true;
            return false;
        }
    }
    public bool HeavyAttack
    {
        get
        {
            if (HeavyAttackWithMemory && !LightAttackWithMemory)
                return true;
            return false;
        }
    }

    bool LightAttackHeld
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetMouseButton(0);
            else
                return Input_NG.GetControllerButton(lightAttackButton, controllerId, 0, controllerType);
        }
    }
    bool HeavyAttackHeld
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetMouseButton(1);
            else
                return Input_NG.GetControllerButton(heavyAttackButton, controllerId, 0, controllerType);
        }
    }

    public bool MutilateDown
    {
        get
        {
            if (LightAttackWithMemory && HeavyAttackWithMemory)
            {
                needResetLight = true;
                needResetHeavy = true;
                return true;
            }
            return false;
        }
    }
    public bool MutilateHeld
    {   
        get
        {
            return LightAttackHeld && HeavyAttackHeld;
        }
    }

    public bool LockTargetRight
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetAxis("Mouse X") > 5 && oldMouseX <= 5;
            else
                return Input_NG.GetControllerAxis(ControllerAxis.rightStickHorizontal, controllerId, 1, true);
        }
    }
    public bool LockTargetLeft
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetAxis("Mouse X") < -5 && oldMouseX > -5;
            else
                return Input_NG.GetControllerAxis(ControllerAxis.rightStickHorizontal, controllerId, 1, false);
        }
    }

    void OnCastAttack(PlayerAttackStats attack)
    {
        needResetHeavy = true;
        needResetLight = true;
    }

    public bool Lock
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetKeyDown(lockKey);
            else
                return Input_NG.GetControllerButton(lockButton, controllerId, 1, controllerType);
        }
    }

    private void OnValidate()
    {
        if (controllerType == ControllerType.Keyboard && controllerId != 0)
            controllerId = 0;
        else if (controllerType != ControllerType.Keyboard && controllerId < 1)
            controllerId = 1;
    }

}
