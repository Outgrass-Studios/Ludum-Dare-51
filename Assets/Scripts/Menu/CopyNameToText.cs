using TMPro;
using UnityEngine;

namespace Game.Menu
{
    [ExecuteInEditMode]
    public class CopyNameToText : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            UpdateText();
        }

#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void Update()
        {
            UpdateText();
        }
#endif

        void UpdateText()
        {
            if (text != null)
                text.text = name;
        }
    }
}
