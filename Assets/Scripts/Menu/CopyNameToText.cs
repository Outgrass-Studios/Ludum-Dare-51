using TMPro;
using UnityEngine;

namespace Game.Menu
{
    [ExecuteInEditMode]
    public class CopyNameToText : MonoBehaviour
    {
        [SerializeField] ButtonController target;

        private void Reset()
        {
            target = GetComponent<ButtonController>();
        }

        private void Awake()
        {
            UpdateText();
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdateText();
        }
#endif

        void UpdateText()
        {
            if (target != null)
                target.text = name;
        }
    }
}
