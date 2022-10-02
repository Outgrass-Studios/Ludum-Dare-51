using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

namespace Game.Menu
{
    [ExecuteInEditMode]
    public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] TMP_Text tmpText;
        public string text;
        [Space]
        [SerializeField] string normalFormat = "{0}";
        [SerializeField] string highlightedFormat = ">{0}<";

        private string _currentFormat;
        public string CurrentFormat => _currentFormat;

        private void Awake()
        {
            SetText(normalFormat);
        }

        private void OnEnable()
        {
            SetText(normalFormat);
        }

        private void Update()
        {
            SetText(_currentFormat);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetText(highlightedFormat);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetText(normalFormat);
        }

        void SetText(string format)
        {
            _currentFormat = format;
            if (tmpText == null) return;
            tmpText.text = string.Format(format, text);
        }
    }
}
