using qASIC.Options.Menu;
using UnityEngine;

namespace Game.Menu
{
    [ExecuteInEditMode]
    public class MenuOptionText : MonoBehaviour
    {
        [SerializeField] ButtonController controller;
        [SerializeField] MenuOption option;

        private void Reset()
        {
            controller = GetComponent<ButtonController>();
            option = GetComponent<MenuOption>();
        }

        private void Awake()
        {
            if (option != null)
                option.optionLabelName = $"{name}: ";
        }

        private void Update()
        {
            if (controller == null || option == null) return;
            controller.text = option.GetLabel();
        }
    }
}