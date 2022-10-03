using Game.Camera;
using UnityEngine;

namespace Game.Checkpoints
{
    public class CameraStateReverter : MonoBehaviour
    {
        [SerializeField] CameraController cam;

        public CameraTrack RestartTrack { get; set; }

        private void Reset()
        {
            cam = GetComponent<CameraController>();
        }

        private void Start()
        {
            RestartTrack = cam.CurrentTrack;
            LevelCheckpointManager.OnStateReverted += LevelCheckpointManager_OnStateReverted;
        }

        private void LevelCheckpointManager_OnStateReverted()
        {
            cam.SetTrack(RestartTrack);
        }
    }
}
