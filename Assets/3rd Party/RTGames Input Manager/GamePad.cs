using System;
using UnityEngine;
using System.Runtime.InteropServices;

class Imports
{
    internal const string DLLName = "XInputInterface";

    #if UNITY_32 || UNITY_EDITOR_32
        [DllImport("XInputInterface")]
    #else
        [DllImport("XInputInterface64")]
    #endif
    public static extern uint XInputGamePadGetState(uint playerIndex, IntPtr state);

    #if UNITY_32 || UNITY_EDITOR_32
        [DllImport("XInputInterface")]
    #else
        [DllImport("XInputInterface64")]
    #endif
    public static extern void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor);
}

public enum ButtonState
{
    Pressed,
    Released
}

public struct GamePadButtons
{
    ButtonState start, back, leftStick, rightStick, leftShoulder, rightShoulder, a, b, x, y;

    internal GamePadButtons(ButtonState start, ButtonState back, ButtonState leftStick, ButtonState rightStick,
                            ButtonState leftShoulder, ButtonState rightShoulder, ButtonState a, ButtonState b,
                            ButtonState x, ButtonState y)
    {
        this.start = start;
        this.back = back;
        this.leftStick = leftStick;
        this.rightStick = rightStick;
        this.leftShoulder = leftShoulder;
        this.rightShoulder = rightShoulder;
        this.a = a;
        this.b = b;
        this.x = x;
        this.y = y;
    }

    public ButtonState Start
    {
        get { return start; }
    }

    public ButtonState Back
    {
        get { return back; }
    }

    public ButtonState LeftStick
    {
        get { return leftStick; }
    }

    public ButtonState RightStick
    {
        get { return rightStick; }
    }

    public ButtonState LeftShoulder
    {
        get { return leftShoulder; }
    }

    public ButtonState RightShoulder
    {
        get { return rightShoulder; }
    }

    public ButtonState A
    {
        get { return a; }
    }

    public ButtonState B
    {
        get { return b; }
    }

    public ButtonState X
    {
        get { return x; }
    }

    public ButtonState Y
    {
        get { return y; }
    }
}

public struct GamePadDPad
{
    ButtonState up, down, left, right;

    internal GamePadDPad(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }

    public ButtonState Up
    {
        get { return up; }
    }

    public ButtonState Down
    {
        get { return down; }
    }

    public ButtonState Left
    {
        get { return left; }
    }

    public ButtonState Right
    {
        get { return right; }
    }
}

public struct GamePadThumbSticks
{
    public struct StickValue
    {
        float x, y;

        internal StickValue(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float X
        {
            get { return x; }
        }

        public float Y
        {
            get { return y; }
        }
    }

    StickValue left, right;

    internal GamePadThumbSticks(StickValue left, StickValue right)
    {
        this.left = left;
        this.right = right;
    }

    public StickValue Left
    {
        get { return left; }
    }

    public StickValue Right
    {
        get { return right; }
    }
}

public struct GamePadTriggers
{
    float left;
    float right;

    internal GamePadTriggers(float left, float right)
    {
        this.left = left;
        this.right = right;
    }

    public float Left
    {
        get { return left; }
    }

    public float Right
    {
        get { return right; }
    }
}

public struct GamePadState
{
    internal struct RawState
    {
        public uint dwPacketNumber;
        public GamePad Gamepad;

        public struct GamePad
        {
            public ushort dwButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }
    }

    bool isConnected;
    uint packetNumber;
    GamePadButtons buttons;
    GamePadDPad dPad;
    GamePadThumbSticks thumbSticks;
    GamePadTriggers triggers;

    enum ButtonsConstants
    {
        DPadUp = 0x00000001,
        DPadDown = 0x00000002,
        DPadLeft = 0x00000004,
        DPadRight = 0x00000008,
        Start = 0x00000010,
        Back = 0x00000020,
        LeftThumb = 0x00000040,
        RightThumb = 0x00000080,
        LeftShoulder = 0x0100,
        RightShoulder = 0x0200,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000
    }

    internal GamePadState(bool isConnected, RawState rawState, GamePadDeadZone deadZone)
    {
        this.isConnected = isConnected;

        if (!isConnected)
        {
            rawState.dwPacketNumber = 0;
            rawState.Gamepad.dwButtons = 0;
            rawState.Gamepad.bLeftTrigger = 0;
            rawState.Gamepad.bRightTrigger = 0;
            rawState.Gamepad.sThumbLX = 0;
            rawState.Gamepad.sThumbLY = 0;
            rawState.Gamepad.sThumbRX = 0;
            rawState.Gamepad.sThumbRY = 0;
        }

        packetNumber = rawState.dwPacketNumber;
        buttons = new GamePadButtons(
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Start) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Back) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.LeftThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.RightThumb) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.LeftShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.RightShoulder) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.A) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.B) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.X) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.Y) != 0 ? ButtonState.Pressed : ButtonState.Released
        );
        dPad = new GamePadDPad(
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadUp) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadDown) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadLeft) != 0 ? ButtonState.Pressed : ButtonState.Released,
            (rawState.Gamepad.dwButtons & (uint)ButtonsConstants.DPadRight) != 0 ? ButtonState.Pressed : ButtonState.Released
        );

        thumbSticks = new GamePadThumbSticks(
            Utils.ApplyLeftStickDeadZone(rawState.Gamepad.sThumbLX, rawState.Gamepad.sThumbLY, deadZone),
            Utils.ApplyRightStickDeadZone(rawState.Gamepad.sThumbRX, rawState.Gamepad.sThumbRY, deadZone)
        );
        triggers = new GamePadTriggers(
            Utils.ApplyTriggerDeadZone(rawState.Gamepad.bLeftTrigger, deadZone),
            Utils.ApplyTriggerDeadZone(rawState.Gamepad.bRightTrigger, deadZone)
        );
    }

    public uint PacketNumber
    {
        get { return packetNumber; }
    }

    public bool IsConnected
    {
        get { return isConnected; }
    }

    public GamePadButtons Buttons
    {
        get { return buttons; }
        internal set { buttons = value; }
    }

    public GamePadDPad DPad
    {
        get { return dPad; }
        internal set { dPad = value; }
    }

    public GamePadTriggers Triggers
    {
        get { return triggers; }
        internal set { triggers = value; }
    }

    public GamePadThumbSticks ThumbSticks
    {
        get { return thumbSticks; }
        internal set { thumbSticks = value; }
    }
}

public enum PlayerIndex
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3
}

public enum GamePadDeadZone
{
    Circular,
    IndependentAxes,
    None
}

public class GamePad
{
    private static bool OnWindowsNative()
    {
        return (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);
    }

    public static GamePadState GetState(PlayerIndex playerIndex)
    {
        return GetState(playerIndex, GamePadDeadZone.Circular);
    }

    public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZone)
    {
        if (OnWindowsNative())
        {
            IntPtr gamePadStatePointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GamePadState.RawState)));
            uint result = Imports.XInputGamePadGetState((uint)playerIndex, gamePadStatePointer);
            GamePadState.RawState state = (GamePadState.RawState)Marshal.PtrToStructure(gamePadStatePointer, typeof(GamePadState.RawState));
            Marshal.FreeHGlobal(gamePadStatePointer);
            return new GamePadState(result == Utils.Success, state, deadZone);
        }
        else
        {
            GamePadState state = new GamePadState();

            state.Buttons = GetMacButtons(playerIndex);
            state.DPad = GetMacDPad(playerIndex);
            state.Triggers = new GamePadTriggers(Mathf.Clamp01(Input.GetAxisRaw("Player" + playerIndex.ToString() + "Axis5")), Mathf.Clamp01(Input.GetAxisRaw("Player" + playerIndex.ToString() + "Axis6")));
            state.ThumbSticks = new GamePadThumbSticks(new GamePadThumbSticks.StickValue(Input.GetAxisRaw("Player" + playerIndex.ToString() + "AxisX"), Input.GetAxisRaw("Player" + playerIndex.ToString() + "AxisY")),
                                                       new GamePadThumbSticks.StickValue(Input.GetAxisRaw("Player" + playerIndex.ToString() + "Axis3"), Input.GetAxisRaw("Player" + playerIndex.ToString() + "Axis4")));
            return state;
        }
    }

    static GamePadButtons GetMacButtons(PlayerIndex player)
    {
        GamePadButtons buttons = new GamePadButtons();
        switch (player)
        {
            case PlayerIndex.One:
                buttons = new GamePadButtons(GetButton(KeyCode.Joystick1Button9),      //Start
                                          GetButton(KeyCode.Joystick1Button10),     //Back
                                          GetButton(KeyCode.Joystick1Button11),     //LeftStick
                                          GetButton(KeyCode.Joystick1Button12),     //RightStick
                                          GetButton(KeyCode.Joystick1Button13),     //LeftShoulder
                                          GetButton(KeyCode.Joystick1Button14),     //RightShoulder
                                          GetButton(KeyCode.Joystick1Button16),     //A
                                          GetButton(KeyCode.Joystick1Button17),     //B
                                          GetButton(KeyCode.Joystick1Button18),     //X
                                          GetButton(KeyCode.Joystick1Button19));    //Y
                break;
            case PlayerIndex.Two:
                buttons = new GamePadButtons(GetButton(KeyCode.Joystick2Button9),      //Start
                                          GetButton(KeyCode.Joystick2Button10),     //Back
                                          GetButton(KeyCode.Joystick2Button11),     //LeftStick
                                          GetButton(KeyCode.Joystick2Button12),     //RightStick
                                          GetButton(KeyCode.Joystick2Button13),     //LeftShoulder
                                          GetButton(KeyCode.Joystick2Button14),     //RightShoulder
                                          GetButton(KeyCode.Joystick2Button16),     //A
                                          GetButton(KeyCode.Joystick2Button17),     //B
                                          GetButton(KeyCode.Joystick2Button18),     //X
                                          GetButton(KeyCode.Joystick2Button19));    //Y
                break;
            case PlayerIndex.Three:
                buttons = new GamePadButtons(GetButton(KeyCode.Joystick3Button9),      //Start
                                          GetButton(KeyCode.Joystick3Button10),     //Back
                                          GetButton(KeyCode.Joystick3Button11),     //LeftStick
                                          GetButton(KeyCode.Joystick3Button12),     //RightStick
                                          GetButton(KeyCode.Joystick3Button13),     //LeftShoulder
                                          GetButton(KeyCode.Joystick3Button14),     //RightShoulder
                                          GetButton(KeyCode.Joystick3Button16),     //A
                                          GetButton(KeyCode.Joystick3Button17),     //B
                                          GetButton(KeyCode.Joystick3Button18),     //X
                                          GetButton(KeyCode.Joystick3Button19));    //Y
                break;
            case PlayerIndex.Four:
                buttons = new GamePadButtons(GetButton(KeyCode.Joystick4Button9),      //Start
                                          GetButton(KeyCode.Joystick4Button10),     //Back
                                          GetButton(KeyCode.Joystick4Button11),     //LeftStick
                                          GetButton(KeyCode.Joystick4Button12),     //RightStick
                                          GetButton(KeyCode.Joystick4Button13),     //LeftShoulder
                                          GetButton(KeyCode.Joystick4Button14),     //RightShoulder
                                          GetButton(KeyCode.Joystick4Button16),     //A
                                          GetButton(KeyCode.Joystick4Button17),     //B
                                          GetButton(KeyCode.Joystick4Button18),     //X
                                          GetButton(KeyCode.Joystick4Button19));    //Y
                break;
        }
        return buttons;
    }

    static GamePadDPad GetMacDPad(PlayerIndex player)
    {
        GamePadDPad DPad = new GamePadDPad();

        switch (player)
        {
            case PlayerIndex.One:
                DPad = new GamePadDPad(GetButton(KeyCode.Joystick1Button5),              //DPadUp
                                       GetButton(KeyCode.Joystick1Button6),              //DPadDown
                                       GetButton(KeyCode.Joystick1Button7),              //DPadLeft
                                       GetButton(KeyCode.Joystick1Button8));             //DPadRight
                break;
            case PlayerIndex.Two:
                DPad = new GamePadDPad(GetButton(KeyCode.Joystick2Button5),              //DPadUp
                                       GetButton(KeyCode.Joystick2Button6),              //DPadDown
                                       GetButton(KeyCode.Joystick2Button7),              //DPadLeft
                                       GetButton(KeyCode.Joystick2Button8));             //DPadRight
                break;
            case PlayerIndex.Three:
                DPad = new GamePadDPad(GetButton(KeyCode.Joystick3Button5),              //DPadUp
                                       GetButton(KeyCode.Joystick3Button6),              //DPadDown
                                       GetButton(KeyCode.Joystick3Button7),              //DPadLeft
                                       GetButton(KeyCode.Joystick3Button8));             //DPadRight
                break;
            case PlayerIndex.Four:
                DPad = new GamePadDPad(GetButton(KeyCode.Joystick4Button5),              //DPadUp
                                       GetButton(KeyCode.Joystick4Button6),              //DPadDown
                                       GetButton(KeyCode.Joystick4Button7),              //DPadLeft
                                       GetButton(KeyCode.Joystick4Button8));             //DPadRight
                break;
        }

        return DPad;
    }

    static ButtonState GetButton(KeyCode code)
    {
        return Input.GetKey((KeyCode)code) ? ButtonState.Pressed : ButtonState.Released;
    }

    public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
    {
        Imports.XInputGamePadSetState((uint)playerIndex, leftMotor, rightMotor);
    }
}