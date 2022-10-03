using UnityEngine;

namespace Game.Checkpoints
{
    public class LevelCheckpoint : MonoBehaviour
    {
        const string PLAYER_TAG = "Player";

        [SerializeField] Vector3 respawnPosition;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(PLAYER_TAG)) return;

            PlayerStateReverter.RestartPosition = transform.position + respawnPosition;
            LevelCheckpointManager.CreateState();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + respawnPosition, 0.4f);
        }
    }
}