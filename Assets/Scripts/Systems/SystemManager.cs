using qASIC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Systems
{
    public static class SystemManager
    {
        const string SYSTEMS_NAME = "Systems";

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (!Application.CanStreamedLevelBeLoaded(SYSTEMS_NAME))
            {
                qDebug.LogError("[Systems] Cannot load systems!");
                return;
            }

            SceneManager.LoadScene(SYSTEMS_NAME, LoadSceneMode.Additive);
        }
    }
}