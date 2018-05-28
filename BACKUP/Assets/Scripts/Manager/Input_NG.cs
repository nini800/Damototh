using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerButton
{
    RightPadDown,
    RightPadRight,
    RightPadUp,
    RightPadLeft,
    LeftPadDown,
    LeftPadRight,
    LeftPadUp,
    LeftPadLeft,
    RightTrigger,
    RightBumper,
    RightMenu,
    RightClick,
    LeftClick,
    LeftMenu,
    LeftBumper,
    LeftTrigger,
    PS,
    Pad
}
public enum ControllerAxis
{
    leftStickHorizontal = 0,
    leftStickVertical = 1,
    rightStickHorizontal = 2,
    rightStickVertical = 3,
    LeftTrigger = 4,
    RightTrigger = 5,
    padHorizontal = 6,
    padVertical = 7
}
public enum ControllerType
{
    Null,
    Keyboard,
    PS4,
    xBox
}


public class Input_NG : MonoBehaviour
{
    static List<bool?[]> lastAxisInput;
    static List<bool?[]> axisInput;

    protected void Start()
    {
        lastAxisInput = new List<bool?[]>();
        axisInput = new List<bool?[]>();
    }

    protected void Update()
    {
        ActualizeAxisInputs();
    }
    public static void ActualizeAxisInputs()
    {
        string[] names = Input.GetJoystickNames();
        ControllerType[] controllersType = new ControllerType[names.Length];

        for (int i = 0; i < names.Length; i++)
            controllersType[i] = GetControllerType(i+1);

        lastAxisInput = new List<bool?[]>(axisInput.ToArray());
        axisInput.Clear();

        float axisValue;
        for (int c = 1; c <= controllersType.Length; c++)
        {
            axisInput.Add(new bool?[8]);
            for (int a = 0; a < 8; a++)
            {
                axisValue = GetControllerAxis((ControllerAxis)a, c, controllersType[c - 1]);
                axisInput[c-1][a] = (axisValue > 0.8f ? true : (axisValue < -0.8f ? false : (bool?)null));
            }
        }
    }


    public static ControllerType GetControllerType(int id)
    {
        if (id == 0)
            return ControllerType.Keyboard;


        string[] joystickNames = Input.GetJoystickNames();

        switch (joystickNames[id - 1].Length)
        {
            case 19:
                return ControllerType.PS4;
            case 33:
                return ControllerType.xBox;
        }

        return ControllerType.xBox;
    }

    delegate bool GetKey(KeyCode key);
    delegate bool GetMouseButton(int button);
    public static bool GetControllerButton(ControllerButton button, int id, int inputMode = 1, ControllerType type = ControllerType.Null)
    {
        GetKey getKey;
        if (inputMode == 0)
            getKey = Input.GetKey;
        else if (inputMode == 2)
            getKey = Input.GetKeyUp;
        else
            getKey = Input.GetKeyDown;
            
        switch (type == ControllerType.Null ? GetControllerType(id) : type)
        {
            case ControllerType.PS4:
                switch (button)
                {
                    case ControllerButton.RightPadDown:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button1"));
                    case ControllerButton.RightPadRight:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button2"));
                    case ControllerButton.RightPadUp:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button3"));
                    case ControllerButton.RightPadLeft:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button0"));
                    case ControllerButton.RightTrigger:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button7"));
                    case ControllerButton.RightBumper:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button5"));
                    case ControllerButton.RightMenu:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button9"));
                    case ControllerButton.RightClick:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button11"));
                    case ControllerButton.LeftClick:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button10"));
                    case ControllerButton.LeftMenu:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button8"));
                    case ControllerButton.LeftBumper:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button4"));
                    case ControllerButton.LeftTrigger:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button6"));
                    case ControllerButton.PS:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button12"));
                    case ControllerButton.Pad:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button13"));
                    
                }
                break;
            case ControllerType.xBox:
                switch (button)
                {
                    case ControllerButton.RightPadDown:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button0"));
                    case ControllerButton.RightPadRight:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button1"));
                    case ControllerButton.RightPadUp:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button3"));
                    case ControllerButton.RightPadLeft:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button2"));
                    case ControllerButton.RightBumper:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button5"));
                    case ControllerButton.RightMenu:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button7"));
                    case ControllerButton.RightClick:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button9"));
                    case ControllerButton.LeftClick:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button8"));
                    case ControllerButton.LeftMenu:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button6"));
                    case ControllerButton.LeftBumper:
                        return getKey((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + id + "Button4"));
                    case ControllerButton.RightTrigger:
                        return GetControllerAxis(ControllerAxis.RightTrigger, id, inputMode);
                    case ControllerButton.LeftTrigger:
                        return GetControllerAxis(ControllerAxis.LeftTrigger, id, inputMode);
                    case ControllerButton.PS:
                        return false;
                    case ControllerButton.Pad:
                        return false;
                }
                break;

        }
        switch (button)
        {
            case ControllerButton.LeftPadDown:
                return GetControllerAxis(ControllerAxis.padVertical, id, inputMode, false);
            case ControllerButton.LeftPadUp:
                return GetControllerAxis(ControllerAxis.padVertical, id, inputMode, true);
            case ControllerButton.LeftPadLeft:
                return GetControllerAxis(ControllerAxis.padHorizontal, id, inputMode, false);
            case ControllerButton.LeftPadRight:
                return GetControllerAxis(ControllerAxis.padHorizontal, id, inputMode, true);
        }

        return false;
    }

    public static float GetControllerAxis(ControllerAxis axis, int id, ControllerType type = ControllerType.Null)
    {
        switch (type == ControllerType.Null ? GetControllerType(id) : type)
        {
            case ControllerType.PS4:
                switch (axis)
                {
                    case ControllerAxis.leftStickHorizontal:
                        return Input.GetAxis("Joystick" + id + "AxisX");
                    case ControllerAxis.leftStickVertical:
                        return Input.GetAxis("Joystick" + id + "AxisY") * -1;
                    case ControllerAxis.rightStickHorizontal:
                        return Input.GetAxis("Joystick" + id + "Axis3");
                    case ControllerAxis.rightStickVertical:
                        return Input.GetAxis("Joystick" + id + "Axis6") * -1;
                    case ControllerAxis.LeftTrigger:
                        return Input.GetAxis("Joystick" + id + "Axis4");
                    case ControllerAxis.RightTrigger:
                        return Input.GetAxis("Joystick" + id + "Axis5");
                    case ControllerAxis.padHorizontal:
                        return Input.GetAxis("Joystick" + id + "Axis7");
                    case ControllerAxis.padVertical:
                        return Input.GetAxis("Joystick" + id + "Axis8");
                    default:
                        break;
                }
                break;
            case ControllerType.xBox:
                switch (axis)
                {
                    case ControllerAxis.leftStickHorizontal:
                        return Input.GetAxis("Joystick" + id + "AxisX");
                    case ControllerAxis.leftStickVertical:
                        return Input.GetAxis("Joystick" + id + "AxisY") * -1;
                    case ControllerAxis.rightStickHorizontal:
                        return Input.GetAxis("Joystick" + id + "Axis4");
                    case ControllerAxis.rightStickVertical:
                        return Input.GetAxis("Joystick" + id + "Axis5");
                    case ControllerAxis.LeftTrigger:
                        return (Input.GetAxis("Joystick" + id + "Axis9") - 0.5f)*2f;
                    case ControllerAxis.RightTrigger:
                        return (Input.GetAxis("Joystick" + id + "Axis10") - 0.5f)*2f;
                    case ControllerAxis.padHorizontal:
                        return Input.GetAxis("Joystick" + id + "Axis6");
                    case ControllerAxis.padVertical:
                        return Input.GetAxis("Joystick" + id + "Axis7");
                    default:
                        break;
                }
                break;
        }
        return 0f;
    }
    public static bool GetControllerAxis(ControllerAxis axis, int id, int InputMode = 0, bool positiveComparison = true)
    {
        if (positiveComparison)
        {
            switch (InputMode)
            {
                case 0://held
                    return axisInput[id-1][(int)axis] == true;
                case 1://down
                    return axisInput[id-1][(int)axis] == true && lastAxisInput[id-1][(int)axis] != true;
                case 2://up
                    return axisInput[id-1][(int)axis] != true && lastAxisInput[id-1][(int)axis] == true;
            }
        }
        else
        {
            switch (InputMode)
            {
                case 0://held
                    return axisInput[id-1][(int)axis] == false;
                case 1://down
                    return axisInput[id-1][(int)axis] == false && lastAxisInput[id-1][(int)axis] != false;
                case 2://up
                    return axisInput[id-1][(int)axis] != false && lastAxisInput[id-1][(int)axis] == false;
            }
        }
        

        throw new System.NotImplementedException();
    }
}
