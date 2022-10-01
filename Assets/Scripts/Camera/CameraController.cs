using UnityEngine;
using qASIC;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] bool setAsMain = true;
        [SerializeField] float smoothness = 0.125f;
        [SerializeField] float defaultZOffset = -20f;
        [SerializeField] CameraTrack defaultTrack;

        private List<CameraTrigger> ActiveTriggers { get; set; } = new List<CameraTrigger>();

        public CameraTrack CurrentTrack { get; private set; }
        public static CameraController Main { get; private set; }


        private void Awake()
        {
            if (setAsMain && Main == null)
                Main = this;

            SetTrack(defaultTrack);
            StartCoroutine(OnPhysicsUpdate());
        }

        internal void AddTrigger(CameraTrigger trigger)
        {
            if (ActiveTriggers.Contains(trigger)) return;
            ActiveTriggers.Add(trigger);

            HandleTriggerChange();
        }

        internal void RemoveTrigger(CameraTrigger trigger)
        {
            if (!ActiveTriggers.Contains(trigger)) return;
            ActiveTriggers.Remove(trigger);

            HandleTriggerChange();
        }

        void HandleTriggerChange()
        {
            if (ActiveTriggers.Count == 1)
                ActiveTriggers[0].Trigger(this);
        }

        private IEnumerator OnPhysicsUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                Follow();
            }
        }

        void Follow()
        {
            if (CameraTarget.Target == null)
                return;

            Transform target = CameraTarget.Target.transform;

            Vector3 targetPosition = target.position;

            if (CurrentTrack != null)
            {
                switch (CurrentTrack.Dimension)
                {
                    case CameraTrack.Dimension2D.X:
                        targetPosition.y = CurrentTrack.Offset.y;
                        targetPosition.x = ClampInTrack(targetPosition.x, CurrentTrack.Offset.x);
                        break;
                    case CameraTrack.Dimension2D.Y:
                        targetPosition.x = CurrentTrack.Offset.x;
                        targetPosition.y = ClampInTrack(targetPosition.y, CurrentTrack.Offset.y);
                        break;
                }
            }

            targetPosition.z += defaultZOffset;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothness);
            transform.position = smoothPosition;

            qDebug.DisplayValue("Active camera tracker", CurrentTrack == null ? "NONE" : CurrentTrack.name);
            qDebug.DisplayValue("Active triggers", ActiveTriggers.Count);
        }

        float ClampInTrack(float value, float offset)
        {
            float newValue = value;
            float minLimit = CurrentTrack.StartPoint + offset;
            float maxLimit = CurrentTrack.EndPoint + offset;

            if (newValue < minLimit && CurrentTrack.PreviousTrack != null)
                SetTrack(CurrentTrack.PreviousTrack);

            if (value <= minLimit)
                newValue = minLimit;

            if (newValue > maxLimit && CurrentTrack.NextTrack != null)
                SetTrack(CurrentTrack.NextTrack);

            if (value >= maxLimit)
                newValue = maxLimit;

            return newValue;
        }

        public void SetTrack(CameraTrack track)
        {
            CurrentTrack = track;
        }
    }
}
