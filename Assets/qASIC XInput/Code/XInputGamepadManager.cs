using qASIC.InputManagement.Update;
using qASIC.InputManagement.Devices;
using System.Collections.Generic;
using XInputDotNetPure;
using System.Xml.Linq;
using UnityEngine;

namespace qASIC.XInput.Devices
{
    public static class XInputGamepadManager
    {
        const int DEVICE_LIMIT = 4;

        static readonly PlayerIndex[] _avaliableIndexes = new PlayerIndex[DEVICE_LIMIT]
        {
            PlayerIndex.One,
            PlayerIndex.Two,
            PlayerIndex.Three,
            PlayerIndex.Four,
        };


        static bool[] _slotConnectionStates = new bool[DEVICE_LIMIT];

        static List<XInputGamepad> _gamepads = new List<XInputGamepad>(new XInputGamepad[DEVICE_LIMIT]);

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            InputUpdateManager.OnUpdate += Update;
        }

        static void Update()
        {
            for (int i = 0; i < DEVICE_LIMIT; i++)
            {
                PlayerIndex index = _avaliableIndexes[i];
                bool isConnected = GamePad.GetState(index).IsConnected;

                //Device connected
                if (isConnected && !_slotConnectionStates[i])
                    OnDeviceConnected(index);

                //Device disconnected
                if (!isConnected && _slotConnectionStates[i])
                    OnDeviceDisconnected(index);

                _slotConnectionStates[i] = isConnected;
            }
        }

        static void OnDeviceConnected(PlayerIndex index)
        {
            XInputGamepad gamepad = new XInputGamepad($"Gamepad {index}", index);
            _gamepads[(int)index] = gamepad;
            DeviceManager.RegisterDevice(gamepad);
            qDebug.Log($"[XInput] Device connected: {gamepad.DeviceName}", "xinput");
        }

        static void OnDeviceDisconnected(PlayerIndex index)
        {
            XInputGamepad gamepad = _gamepads[(int)index];
            DeviceManager.RegisterDevice(gamepad);
            qDebug.Log($"[XInput] Device disconnected: {gamepad.DeviceName}", "xinput");
            _gamepads[(int)index] = null;
        }
    }
}