using UnityEngine;

namespace Game
{
    public class SceneLoadTrigger : MonoBehaviour
    {
        const string PLAYER_TAG = "Player";

        [SerializeField] Collider2D coll;
        [SerializeField] [SceneName] string sceneName;

        private void Reset()
        {
            coll = GetComponent<Collider2D>();

            if (coll == null)
                coll = gameObject.AddComponent<BoxCollider2D>();

            coll.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(PLAYER_TAG)) return;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
