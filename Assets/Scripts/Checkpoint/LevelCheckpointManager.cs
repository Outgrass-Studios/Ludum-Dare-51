using System;
using qASIC;

namespace Game.Checkpoints
{
    public static class LevelCheckpointManager
    {
        public static event Action OnStateReverted;
        public static event Action OnStateCreated;

        public static void CreateState()
        {
            OnStateCreated?.Invoke();
        }

        public static void RevertState()
        {
            OnStateReverted?.Invoke();
            qDebug.Log("[Checkpoint] Reverted level state", "checkpoint");
        }

        public static void KillPlayer()
        {
            RevertState();
        }
    }
}
