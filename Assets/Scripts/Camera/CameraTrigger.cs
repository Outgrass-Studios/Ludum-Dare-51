using UnityEngine;

namespace Game.Camera
{
    public class CameraTrigger : MonoBehaviour
    {
        const string PLAYER_TAG = "Player";

        [SerializeField] CameraController cam;
        [SerializeField] CameraTrack track;

        private void Reset()
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null)
                collider = gameObject.AddComponent<BoxCollider2D>();

            collider.isTrigger = true;
        }

        public void Trigger(CameraController camera)
        {
            camera.SetTrack(track);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag != PLAYER_TAG)
                return;

            CameraController camera = cam ?? CameraController.Main;

            camera.AddTrigger(this);
            Trigger(camera);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag != PLAYER_TAG)
                return;

            (cam ?? CameraController.Main).RemoveTrigger(this);
        }
    }
}
