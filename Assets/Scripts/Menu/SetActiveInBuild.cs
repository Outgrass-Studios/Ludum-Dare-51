using UnityEngine;

namespace Game.Menu
{
    public class SetActiveInBuild : MonoBehaviour
    {
        [SerializeField] bool state = false;
        [SerializeField] bool changeOnBuild = true;
        [SerializeField] bool changeOnDevBuild;
        [SerializeField] bool changeInEditor;

        private void Awake()
        {
            bool change = (changeOnBuild && !Application.isEditor) ||
                (changeOnDevBuild && Debug.isDebugBuild) ||
                (changeInEditor && Application.isEditor);

            gameObject.SetActive(change == state);
        }
    }
}