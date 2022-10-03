using UnityEngine;

namespace Game
{
    public class ParticleSystemStarter : MonoBehaviour
    {
        [SerializeField] ParticleSystem system;
        [SerializeField] float startTime;

        private void Reset()
        {
            system = GetComponent<ParticleSystem>();
        }

        private void Awake()
        {
            system.Simulate(startTime);
            system.Play();
        }
    }
}