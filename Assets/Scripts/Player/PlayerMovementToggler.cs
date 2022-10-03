using System;
using UnityEngine;

namespace Game.Player
{
    public class PlayerMovementToggler : MonoBehaviour
    {
        [SerializeField] PlayerMovementController normalMovement;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] PlayerFlightController flightMovement;

        static event Action OnFlightChange;
        private static bool _flight = false;
        public static bool FlightActive 
        { 
            get
            {
                return _flight;
            }
            set
            {
                _flight = value;
                OnFlightChange();
            }
        }

        private void Reset()
        {
            normalMovement = GetComponent<PlayerMovementController>();
            rb = GetComponent<Rigidbody2D>();
            flightMovement = GetComponent<PlayerFlightController>();
        }

        private void Awake()
        {
            UpdateToggle();

            OnFlightChange += UpdateToggle;
        }

        public void UpdateToggle()
        {
            rb.isKinematic = FlightActive;
            rb.velocity = Vector2.zero;
            normalMovement.enabled = !FlightActive;
            flightMovement.enabled = FlightActive;
        }
    }
}