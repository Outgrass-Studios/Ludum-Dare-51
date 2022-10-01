using UnityEngine;

namespace Game.Camera
{
    public class CameraTarget : MonoBehaviour
    {
        public static CameraTarget Target { get; private set; }

        private void Awake()
        {
            if (Target == null)
            {
                Target = this;
                return;
            }

            if (Target != this)
                Destroy(this);
        }
    }
}