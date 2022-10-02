using qASIC.Options;
using qASIC.Options.Menu;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Menu.Settings
{
    public class PopupMenuOption : MenuOption
    {
        [SerializeField] List<string> values;
        [SerializeField] ToggableValue<string> firstOptionTextOverride;

        int _index;

        public void CycleThroughOptions()
        {
            if (values.Count == 0)
                return;

            int i = _index + 1;
            if (i >= values.Count)
                i = 0;

            _index = i;
            SetValue(values[i], true);
        }

        public override string GetLabel()
        {
            if (values.Count == 0) return "NONE";
            return $"{optionLabelName}{(_index == 0 && firstOptionTextOverride.Enabled ? firstOptionTextOverride.Value : values[_index])}";
        }

        public override void LoadOption()
        {
            if (values.Count == 0) return;
            if (!OptionsController.TryGetOptionValue(optionName, out string optionValue)) return;

            int i = values.IndexOf(optionValue);

            if (i == -1)
                return;

            _index = i;
        }
    }
}