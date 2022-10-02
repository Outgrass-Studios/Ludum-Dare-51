using UnityEngine;

namespace Game.Checkpoints
{
    public class PlayerStateReverter : MonoBehaviour
    {
        [SerializeField] Vector3 startPositionOffset;

        public static Vector3 RestartPosition { get; set; }

        private void Awake()
        {
            RestartPosition = transform.position + startPositionOffset;
            LevelCheckpointManager.OnStateReverted += LevelCheckpointManager_OnStateReverted;
        }

        private void LevelCheckpointManager_OnStateReverted()
        {
            transform.position = RestartPosition;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + startPositionOffset, 0.4f);
        }
    }
}