using Game.Checkpoints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DeadZone : MonoBehaviour
    {
        const string PLAYER_TAG = "Player";

        private void Reset()
        {
            Collider2D coll = GetComponent<Collider2D>();
            if (GetComponent<Collider2D>() == null)
                coll = gameObject.AddComponent<BoxCollider2D>();

            coll.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(PLAYER_TAG)) return;

            LevelCheckpointManager.KillPlayer();
        }
    }
}
