using qASIC.Toggling;
using UnityEngine;

namespace Game.Menu
{
    public class TogglerModeSwitch : MonoBehaviour
    {
        [SerializeField] Toggler[] togglers;

        private void Reset()
        {
            togglers = GetComponents<Toggler>();
        }

        public void ChangeMode(bool both)
        {
            foreach (var item in togglers)
                if (item != null)
                    item.keyMode = both ? Toggler.KeyToggleMode.Both :  Toggler.KeyToggleMode.On;
        }
    }
}
