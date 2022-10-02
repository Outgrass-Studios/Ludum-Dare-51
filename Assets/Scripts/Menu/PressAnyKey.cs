using qASIC.InputManagement.Devices;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Menu
{
    public class PressAnyKey : MonoBehaviour
    {
        public UnityEvent OnAnyKeyPressed;

        private void Update()
        {
            var devices = DeviceManager.Devices;

            if (Input.anyKeyDown)
            {
                OnAnyKeyPressed?.Invoke();
                return;
            }

            foreach (var device in devices)
                if (!string.IsNullOrEmpty(device.GetAnyKeyDown()))
                    OnAnyKeyPressed?.Invoke();
        }
    }
}