using UnityEngine;
using qASIC.InputManagement.Map;
using System.Linq;

namespace qASIC.InputManagement
{
    [System.Serializable]
    public class InputMapItemReference
    {
        [SerializeField] string guid;

        public string Guid => guid;

        public InputMapItemReference() { }

        public InputMapItemReference(string guid)
        {
            this.guid = guid;
        }

        public InputMap GetMap()
        {
#if UNITY_EDITOR
            return Application.isPlaying ? InputManager.Map : ProjectSettings.InputProjectSettings.Instance.map;
#else
            return InputManager.Map;
#endif
        }

        public bool ItemExists() =>
            GetMap()?.ItemsDictionary.ContainsKey(guid) ?? false;

        public InputGroup GetGroup()
        {
            var item = GetItem();
            return GetMap()?.groups
            .Where(x => x.items.Contains(item))
            .First();
        }

        public InputMapItem GetItem()
        {
            if (!ItemExists())
                return null;

            return GetMap().ItemsDictionary[guid];
        }

        public string GetGroupName() =>
            GetGroup()?.ItemName ?? string.Empty;

        public string GetItemName() =>
            GetItem()?.ItemName ?? string.Empty;
    }
}