using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using qASIC.InputManagement.Devices;
using qASIC.InputManagement.Map;
using System.Linq;
using System.Collections.Generic;

namespace qASIC.InputManagement.Menu
{
    [AddComponentMenu("qASIC/Options/Input Asign")]
    public class InputAssign : MonoBehaviour
    {
        [Header("Updating name")]
        [SerializeField] TextMeshProUGUI nameText;
        public string optionLabelName;
        public string listeningForKeyText = "Listening for key";

        [Header("Options")]
        [SerializeField] int playerIndex;
        public InputMapItemReference inputAction;
        [SerializeField] int keyIndex;
        [SerializeField] string keyRootPath = "key_keyboard";

        [Header("Events")]
        [SerializeField] UnityEvent OnStartListening;
        [SerializeField] UnityEvent OnAssign;

        [HideInInspector] public bool isListening = false;

        private void Reset()
        {
            Button button = GetComponent<Button>();

#if UNITY_EDITOR
            if (button != null)
                UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, StartListening);
#endif

            nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void LateUpdate()
        {
            if (nameText != null)
                nameText.text = isListening ? listeningForKeyText : GetLabel();

            if (isListening)
                ListenForKey();
        }

        private void ListenForKey()
        {
            List<IInputDevice> devices = InputManager.Players[playerIndex].CurrentDevices;

            foreach (var device in devices)
            {
                string key = device.GetAnyKeyDown();
                if (string.IsNullOrEmpty(key)) continue;
                if (key.StartsWith(keyRootPath)) continue;

                Assign(key);
                break;
            }
        }

        public string GetLabel()
        {
            string currentKey = "UNKNOWN";
            if (InputManager.Players[playerIndex].MapData.ItemsDictionary.TryGetValue(inputAction.Guid, out InputMapItem item) &&
                item is InputBinding binding)
                currentKey = binding.keys[keyIndex].Split('/').Last();

            return $"{optionLabelName}{currentKey}";
        }

        public void StartListening()
        {
            isListening = true;
            OnStartListening.Invoke();
        }

        public void Assign(string key)
        {
            InputManager.ChangeInput(inputAction.GetGroupName(), inputAction.GetItemName(), keyIndex, key);
            isListening = false;
            OnAssign.Invoke();
        }
    }
}