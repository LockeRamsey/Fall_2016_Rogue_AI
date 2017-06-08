using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Will return a value between -1 and 1 for certain buttons, but safest to use this if you are only looking for a value of true or false.
/// </summary>
/// <remarks>
/// This is the base class for checking for values of all input variables. It runs the updating on each individual input only when asked for a check on the value's state.
/// </remarks>
public class CombineInputs
{
    int lastUpdateFrameCount;
    string name;
    public List<AllKeysButtonsAndAxes> inputs = new List<AllKeysButtonsAndAxes>();
    public PlayerIndex player = PlayerIndex.One;

    List<AllKeysButtonsAndAxes> baseInputs = new List<AllKeysButtonsAndAxes>();

    float Value;
    bool isPressed, wasPressed;

    /// <summary>
    /// Combine Inputs allows you to have multiple key/button presses reflecting a single action.
    /// </summary>
    /// <param name="player">The specified gamepad for players 1-4</param>
    /// <param name="code">The single keycode that will cause correspond to this input action.</param>
    public CombineInputs(PlayerIndex player, string name, AllKeysButtonsAndAxes code)
    {
        this.player = player;
        this.name = name;
        AddKeyBind(code);
        baseInputs.Add(code);
    }

    /// <summary>
    /// Combine Inputs allows you to have multiple key/button presses reflecting a single action.
    /// </summary>
    /// <param name="player">The specified gamepad for players 1-4</param>
    /// <param name="code">A comma separated list of keycodes that will cause correspond to this input action.</param>
    public CombineInputs(PlayerIndex player, string name, params AllKeysButtonsAndAxes[] codes)
    {
        this.player = player;
        this.name = name;
        foreach (AllKeysButtonsAndAxes c in codes) AddKeyBind(c);
        foreach (AllKeysButtonsAndAxes c in codes) baseInputs.Add(c);
    }

    /// <summary>
    /// Adds another keycode to this input.
    /// </summary>
    public void AddKeyBind(AllKeysButtonsAndAxes input) { inputs.Add(input); }

    /// <summary>
    /// Clears all keycodes associated with this input.
    /// </summary>
    public void ClearKeyBinds() { inputs.Clear(); }

    /// <summary>
    /// Returns the percentage that the button was pressed down. For Keyboard keys and buttons, will return a value of either 0 or 1.
    /// </summary>
    public float GetValue()
    {
        if (lastUpdateFrameCount != Time.frameCount)
            Update();

        return Value;
    }

    /// <summary>
    /// Returns true while the key is pressed down.
    /// </summary>
    public bool GetDown()
    {
        if (lastUpdateFrameCount != Time.frameCount)
            Update();

        return isPressed;
    }

    /// <summary>
    /// Returns true the first frame the key was released from the down state.
    /// </summary>
    public bool GetReleased()
    {
        if (lastUpdateFrameCount != Time.frameCount)
            Update();

        return !isPressed && wasPressed;
    }

    /// <summary>
    /// Returns true the first frame the key was pressed down.
    /// </summary>
    public bool GetPressed()
    {
        if (lastUpdateFrameCount != Time.frameCount)
            Update();

        return isPressed && !wasPressed;
    }

    void Update()
    {
        if (lastUpdateFrameCount == 0)
            ReadFromPlayerPrefs();
        lastUpdateFrameCount = Time.frameCount;

        float value = 0;

        foreach (AllKeysButtonsAndAxes code in inputs)
        {
            float tempValue = InputValues.GetButtonOrAxisValue(code, player);
            if (Mathf.Abs(tempValue) > Mathf.Abs(value))
                value = tempValue;
        }
        wasPressed = isPressed;
        isPressed = Mathf.Abs(value) > 0;

        Value = value;
    }

    void ReadFromPlayerPrefs()
    {
        string prefs = PlayerPrefs.GetString("Input_" + name, "");
        if (prefs == "")
            return;

        ClearKeyBinds();

        string[] keys = prefs.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);

        foreach (string s in keys)
        {
            int i = Convert.ToInt32(s);

            AddKeyBind((AllKeysButtonsAndAxes)i);
        }
    }

    public void WriteToPlayerPrefs()
    {
        string prefs = "";

        foreach (AllKeysButtonsAndAxes k in inputs)
            prefs = prefs + (int)k + ",";

        PlayerPrefs.SetString("Input_" + name, prefs);
    }

    public void DeleteFromPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("Input_" + name);
        ClearKeyBinds();
        foreach (AllKeysButtonsAndAxes c in baseInputs) inputs.Add(c);
    }
}

public class CombineInputsArray
{
    CombineInputs[] Values = new CombineInputs[4];
    
    public CombineInputs this[PlayerIndex index] { get { return Values[(int)index]; } }

    /// <param name="index">The value to access</param>
    /// <param name="offset">True if your counting should start at one.</param>
    public CombineInputs this[int index, bool offset]
    {
        get
        {
            index += offset ? -1 : 0;
            switch (index)
            {
                case 0:
                    return Values[0];
                case 1:
                    return Values[1];
                case 2:
                    return Values[2];
                case 3:
                    return Values[3];
            }
            return null;
        }
    }

    public CombineInputsArray(CombineInputs one, CombineInputs two, CombineInputs three, CombineInputs four)
    {
        Values[0] = one;
        Values[1] = two;
        Values[2] = three;
        Values[3] = four;
    }

    /// <summary>
    /// Returns the percentage that the button was pressed down. For Keyboard keys and buttons, will return a value of either 0 or 1.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public float GetValue(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetValue(); }

    /// <summary>
    /// Returns true while the key is pressed down.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public bool GetDown(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetDown(); }

    /// <summary>
    /// Returns true the first frame the key was released from the down state.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public bool GetReleased(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetReleased(); }

    /// <summary>
    /// Returns true the first frame the key was pressed down.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public bool GetPressed(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetPressed(); }

}

/// <summary>
/// Allows joining two CombinedInputs together to represent a positive or negative value ranging from -1 to 1, thus creating an axis.
/// </summary>
public class InputAxis
{
    CombineInputs Positive;
    CombineInputs Negative;

    /// <summary>
    /// Returns true if the axis has its positive and negative inputs swapped.
    /// </summary>
    public bool isInverted { get; private set; }

    bool singleAxis = false;

    /// <summary>
    /// Use this if you want two CombinedInputs to represent a positive and negative.
    /// </summary>
    public InputAxis(PlayerIndex player, CombineInputs positive, CombineInputs negative)
    {
        if (positive == negative)
        {
            Positive = positive;
            singleAxis = true;
        }
        else
        {
            Positive = positive;
            Negative = negative;
        }
    }

    /// <summary>
    /// Used to reverse the axis so positive is negative, and negative is positive.
    /// </summary>
    public void InvertAxis()
    {
        if (!singleAxis)
        {
            CombineInputs temp = Positive;
            Positive = Negative;
            Negative = temp;
        }

        isInverted = !isInverted;
    }

    /// <summary>
    /// Returns a value greater than 0 if positive, or less than 0 if negative. These will be reversed if the isInverted is true.
    /// </summary>
    public float GetAxis()
    {
        float value = 0;
        if (!singleAxis)
        {
            if (Mathf.Abs(Positive.GetValue()) >= Mathf.Abs(Negative.GetValue()))
                value = Positive.GetValue();
            else if (Mathf.Abs(Positive.GetValue()) < Mathf.Abs(Negative.GetValue()))
            {
                if (Negative.GetValue() > 0)
                    value = -Negative.GetValue();
                else
                    value = Negative.GetValue();
            }
        }
        else
        {
            value = Positive.GetValue();
            if (isInverted)
                value *= -1;
        }

        return value;
    }

    public float GetInvertedAxis()
    {
        float value = 0;
        if (!singleAxis)
        {
            if (Mathf.Abs(Negative.GetValue()) >= Mathf.Abs(Positive.GetValue()))
                value = Negative.GetValue();
            else if (Mathf.Abs(Negative.GetValue()) < Mathf.Abs(Positive.GetValue()))
            {
                if (Positive.GetValue() > 0)
                    value = -Positive.GetValue();
                else
                    value = Negative.GetValue();
            }
        }
        else
        {
            value = Positive.GetValue();
            value *= -1;
        }

        return value;
    }
}

public class InputAxisArray
{
    InputAxis[] Values = new InputAxis[4];

    public InputAxis this[PlayerIndex index]
    {
        get
        {
            switch (index)
            {
                case PlayerIndex.One:
                    return Values[0];
                case PlayerIndex.Two:
                    return Values[1];
                case PlayerIndex.Three:
                    return Values[2];
                case PlayerIndex.Four:
                    return Values[3];
            }
            return null;
        }
    }

    /// <param name="index">The value to access</param>
    /// <param name="offset">True if your counting should start at one.</param>
    public InputAxis this[int index, bool offset = false]
    {
        get
        {
            index += offset ? -1 : 0;
            switch (index)
            {
                case 0:
                    return Values[0];
                case 1:
                    return Values[1];
                case 2:
                    return Values[2];
                case 3:
                    return Values[3];
            }
            return null;
        }
    }

    public InputAxisArray(InputAxis one, InputAxis two, InputAxis three, InputAxis four)
    {
        Values[0] = one;
        Values[1] = two;
        Values[2] = three;
        Values[3] = four;
    }

    /// <summary>
    /// Returns a value greater than 0 if positive, or less than 0 if negative. These will be reversed if the isInverted is true.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public float GetAxis(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetAxis(); }

    public float GetInvertedAxis(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetInvertedAxis(); }
}

public class KeyCombination
{
    bool oldGetDown = false;
    List<CombineInputs> AllKeys = new List<CombineInputs>();

    public KeyCombination(PlayerIndex player, params CombineInputs[] keys) { foreach (CombineInputs key in keys) AllKeys.Add(key); }

    /// <summary>
    /// Returns true while all the keys are pressed down.
    /// </summary>
    public bool GetDown()
    {
        foreach (CombineInputs k in AllKeys)
        {
            if (!k.GetDown())
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns true the first frame atleast one key was released while in the down state.
    /// </summary>
    public bool GetReleased()
    {
        if (!GetDown() && oldGetDown)
            foreach (CombineInputs k in AllKeys)
                if (k.GetReleased())
                {
                    oldGetDown = GetDown();
                    return true;
                }

        oldGetDown = GetDown();
        return false;
    }

    /// <summary>
    /// Returns true the first frame all the keys were pressed down.
    /// </summary>
    public bool GetPressed()
    {
        if (GetDown())
            foreach (CombineInputs k in AllKeys)
                if (k.GetPressed())
                    return true;

        return false;
    }
}

public class KeyCombinationArray
{
    KeyCombination[] Values = new KeyCombination[4];

    public KeyCombination this[PlayerIndex index]
    {
        get
        {
            switch (index)
            {
                case PlayerIndex.One:
                    return Values[0];
                case PlayerIndex.Two:
                    return Values[1];
                case PlayerIndex.Three:
                    return Values[2];
                case PlayerIndex.Four:
                    return Values[3];
            }
            return null;
        }
    }

    /// <param name="index">The value to access</param>
    /// <param name="offset">True if your counting should start at one.</param>
    public KeyCombination this[int index, bool offset = false]
    {
        get
        {
            index += offset ? -1 : 0;
            switch (index)
            {
                case 0:
                    return Values[0];
                case 1:
                    return Values[1];
                case 2:
                    return Values[2];
                case 3:
                    return Values[3];
            }
            return null;
        }
    }

    public KeyCombinationArray(KeyCombination one, KeyCombination two, KeyCombination three, KeyCombination four)
    {
        Values[0] = one;
        Values[1] = two;
        Values[2] = three;
        Values[3] = four;
    }

    /// <summary>
    /// Returns true while all the keys are pressed down.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public bool GetDown(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetDown(); }

    /// <summary>
    /// Returns true the first frame atleast one key was released while in the down state.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public bool GetReleased(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetReleased(); }

    /// <summary>
    /// Returns true the first frame all the keys were pressed down.
    /// </summary>
    /// <param name="index">If left out, will return the value of Player one.</param>
    public bool GetPressed(PlayerIndex index = PlayerIndex.One) { return Values[(int)index].GetPressed(); }
}

public static class InputValues
{
    static Vector3 oldMousePosition;
    public static Vector2 MouseDelta;
    static int lastMousePosUpdate;

    public static float GetButtonOrAxisValue(AllKeysButtonsAndAxes code, PlayerIndex playerIndex)
    {
        if (Time.frameCount != lastMousePosUpdate)
        {
            MouseDelta = (Input.mousePosition - oldMousePosition);
            lastMousePosUpdate = Time.frameCount;
            oldMousePosition = Input.mousePosition;
        }

        float value = 0;
        switch (code)
        {
            #region GamePad Buttons
            case (AllKeysButtonsAndAxes.Start):
                try { if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Back):
                try { if (GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Left_Stick):
                try { if (GamePad.GetState(playerIndex).Buttons.LeftStick == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Right_Stick):
                try { if (GamePad.GetState(playerIndex).Buttons.RightStick == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Left_Shoulder):
                try { if (GamePad.GetState(playerIndex).Buttons.LeftShoulder == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Right_Shoulder):
                try { if (GamePad.GetState(playerIndex).Buttons.RightShoulder == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.GamePad_A):
                try { if (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.GamePad_B):
                try { if (GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.GamePad_X):
                try { if (GamePad.GetState(playerIndex).Buttons.X == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.GamePad_Y):
                try { if (GamePad.GetState(playerIndex).Buttons.Y == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Left_Trigger):
                try { value = GamePad.GetState(playerIndex).Triggers.Left; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Right_Trigger):
                try { value = GamePad.GetState(playerIndex).Triggers.Right; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Left_Stick_Vertical):
                try { value = GamePad.GetState(playerIndex).ThumbSticks.Left.Y; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Left_Stick_Horizontal):
                try { value = GamePad.GetState(playerIndex).ThumbSticks.Left.X; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Right_Stick_Vertical):
                try { value = GamePad.GetState(playerIndex).ThumbSticks.Right.Y; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.Right_Stick_Horizontal):
                try { value = GamePad.GetState(playerIndex).ThumbSticks.Right.X; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Left):
                try { if (GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Right):
                try { if (GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Up):
                try { if (GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Down):
                try { if (GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Left_Up):
                try { if (GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed && GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Left_Down):
                try { if (GamePad.GetState(playerIndex).DPad.Left == ButtonState.Pressed && GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Right_Up):
                try { if (GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed && GamePad.GetState(playerIndex).DPad.Up == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            case (AllKeysButtonsAndAxes.D_Pad_Right_Down):
                try { if (GamePad.GetState(playerIndex).DPad.Right == ButtonState.Pressed && GamePad.GetState(playerIndex).DPad.Down == ButtonState.Pressed) value = 1; }
                catch { Debug.LogError("XInput DLL not found or the build state does not support XInput."); }
                break;
            #endregion

            #region Mouse Functions
            case (AllKeysButtonsAndAxes.Mouse_Button_Left):
                value = Convert.ToInt32(Input.GetMouseButton(0));
                break;
            case (AllKeysButtonsAndAxes.Mouse_Button_Middle):
                value = Convert.ToInt32(Input.GetMouseButton(2));
                break;
            case (AllKeysButtonsAndAxes.Mouse_Button_Right):
                value = Convert.ToInt32(Input.GetMouseButton(1));
                break;
            case (AllKeysButtonsAndAxes.Mouse_X):
                value = MouseDelta.x * GameInput.MouseSensitivity;
                break;
            case (AllKeysButtonsAndAxes.Mouse_Y):
                value = MouseDelta.y * GameInput.MouseSensitivity;
                break;
            case (AllKeysButtonsAndAxes.Scroll_Wheel):
                value = Input.mouseScrollDelta.y * GameInput.ScrollWheelSensitivity;
                break;
            #endregion
            //All other Default Keycodes
            default:
                value = Convert.ToInt32(Input.GetKey((KeyCode)code));
                break;
        }
        return value;
    }

    public static AllKeysButtonsAndAxes CurrentPressedKey(PlayerIndex playerIndex)
    {
        foreach (AllKeysButtonsAndAxes item in (AllKeysButtonsAndAxes[])Enum.GetValues(typeof(AllKeysButtonsAndAxes)))
        {
            if (GameInput.ExclusionList.Contains(item))
                continue;
            if (GetButtonOrAxisValue(item, playerIndex) != 0)
                return item;
        }
        return AllKeysButtonsAndAxes.None;
    }
}

public enum AllKeysButtonsAndAxes
{
    #region Default Unity Keycodes
    None = 0,
    Backspace = 8,
    Tab,
    Clear = 12,
    Return,
    Pause = 19,
    Escape = 27,
    Space = 32,
    Exclaim,
    DoubleQuote,
    Hash,
    Dollar,
    Ampersand = 38,
    Quote,
    LeftParen,
    RightParen,
    Asterisk,
    Plus,
    Comma,
    Minus,
    Period,
    Slash,
    Alpha0,
    Alpha1,
    Alpha2,
    Alpha3,
    Alpha4,
    Alpha5,
    Alpha6,
    Alpha7,
    Alpha8,
    Alpha9,
    Colon,
    Semicolon,
    Less,
    Equals,
    Greater,
    Question,
    At,
    LeftBracket = 91,
    Backslash,
    RightBracket,
    Caret,
    Underscore,
    BackQuote,
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    Delete = 127,
    Keypad0 = 256,
    Keypad1,
    Keypad2,
    Keypad3,
    Keypad4,
    Keypad5,
    Keypad6,
    Keypad7,
    Keypad8,
    Keypad9,
    KeypadPeriod,
    KeypadDivide,
    KeypadMultiply,
    KeypadMinus,
    KeypadPlus,
    KeypadEnter,
    KeypadEquals,
    UpArrow,
    DownArrow,
    RightArrow,
    LeftArrow,
    Insert,
    Home,
    End,
    PageUp,
    PageDown,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12,
    F13,
    F14,
    F15,
    Numlock = 300,
    CapsLock,
    ScrollLock,
    RightShift,
    LeftShift,
    RightControl,
    LeftControl,
    RightAlt,
    LeftAlt,
    RightApple,
    LeftCommand,
    LeftWindows,
    RightWindows,
    AltGr,
    Help = 315,
    Print,
    SysReq,
    Break,
    Menu,
    #endregion

    #region GamePad Buttons
    Start,
    Back,
    Left_Stick,
    Right_Stick,
    Left_Shoulder,
    Right_Shoulder,
    GamePad_A,
    GamePad_B,
    GamePad_X,
    GamePad_Y,
    Left_Trigger,
    Right_Trigger,
    Left_Stick_Vertical,
    Left_Stick_Horizontal,
    Right_Stick_Vertical,
    Right_Stick_Horizontal,
    D_Pad_Left,
    D_Pad_Right,
    D_Pad_Up,
    D_Pad_Down,
    D_Pad_Left_Up,
    D_Pad_Left_Down,
    D_Pad_Right_Up,
    D_Pad_Right_Down,
    #endregion

    #region Mouse Functions
    Mouse_X,
    Mouse_Y,
    Scroll_Wheel,
    Mouse_Button_Left,
    Mouse_Button_Middle,
    Mouse_Button_Right
    #endregion
}