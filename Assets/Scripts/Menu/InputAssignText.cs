using qASIC.InputManagement.Menu;
using UnityEngine;
using TMPro;
using qASIC.InputManagement.Map;

namespace Game.Menu
{
    public class InputAssignText : MonoBehaviour
    {
        public InputAssign inputAssign;
        public TMP_Text tmpText;
        public string text = "{0}";

        private void Reset()
        {
            tmpText = GetComponent<TMP_Text>();
            inputAssign = GetComponent<InputAssign>();
        }

        private void Awake()
        {
            if (inputAssign != null)
                inputAssign.optionLabelName = inputAssign.inputAction.GetItemName();
        }

        private void Update()
        {
            UpdateText();
        }

        void UpdateText()
        {
            if (tmpText == null) return;
            tmpText.text = string.Format(text, inputAssign.GetLabel());
        }
    }
}
