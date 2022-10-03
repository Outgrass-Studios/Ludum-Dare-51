using qASIC.InputManagement;
using qASIC.Console;
using UnityEngine;

namespace Game
{
    public class PlayerCheatShortcuts : MonoBehaviour
    {
        [SerializeField] Shortcut[] shortcuts;

        public static bool EnableShortcuts { get; set; }

        private void Update()
        {
            if (!EnableShortcuts) return;
            
            foreach (var shortcut in shortcuts)
            {
                if (InputManager.GetInputDown(shortcut.input.GetGroupName(), shortcut.input.GetItemName()))
                    GameConsoleController.RunCommand(shortcut.command);
            }
        }

        [System.Serializable]
        public struct Shortcut
        {
            public InputMapItemReference input;
            public string command;
        }
    }
}
