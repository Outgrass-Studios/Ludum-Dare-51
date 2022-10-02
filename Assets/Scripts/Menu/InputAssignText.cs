using qASIC.InputManagement.Menu;
using UnityEngine;

namespace Game.Menu
{
    [ExecuteInEditMode]
    public class InputAssignText : MonoBehaviour
    {
        public InputAssign inputAssign;
        public ButtonController controller;

        private void Reset()
        {
            controller = GetComponent<ButtonController>();
            inputAssign = GetComponent<InputAssign>();
        }

        private void Awake()
        {
            if (inputAssign != null)
                inputAssign.optionLabelName = $"{name}: ";
        }

        private void Update()
        {
            if (controller == null || inputAssign == null) return;
            controller.text = inputAssign.GetLabel();
        }
    }
}
