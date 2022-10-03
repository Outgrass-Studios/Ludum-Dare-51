using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Game.Player
{
    public class GrapplingHook : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] LineRenderer rope;

        [SerializeField][Range(0.0f, 1.0f)] float dampingCoefficient = 0.3f;
        
        Vector2 anchor;
        float theta;
        float lineLength;
        float velocity;

        bool grabbed;
        bool isResting;

        public void Grab(Vector2 moveDir)
        {
            if (moveDir == Vector2.zero || moveDir == new Vector2(0.0f, -1.0f))
                return;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir);

            if (!hit)
                return;

            grabbed = true;
            isResting = false;
            velocity = 0;
            anchor = hit.point;
            lineLength = Vector2.Distance(transform.position, anchor);
            Vector2 line = new Vector3(anchor.x, anchor.y) - transform.position;
            theta = Mathf.PI / 2 - Mathf.Atan2(line.y, line.x);
            UpdateRope();
            rope.enabled = true;
        }
        public void LetGo()
        {
            grabbed = false;
            rope.enabled = false;
            rb.velocity -= new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * velocity;
        }
        private void FixedUpdate()
        {
            if (grabbed)
            { 
                UpdatePosition();
                UpdateRope();
            }
        }
        void UpdatePosition()
        {
            if (!isResting)
            {
                float acceleration = -(100.0f / lineLength) * 2 * Mathf.Sin(theta) - velocity * dampingCoefficient;
                velocity += acceleration * Time.fixedDeltaTime;
                theta += velocity * Time.deltaTime;
            }
            transform.position = anchor - new Vector2(Mathf.Sin(theta), Mathf.Cos(theta)) * lineLength;
        }

        void UpdateRope()
        {
            rope.SetPosition(0, transform.position);
            rope.SetPosition(1, anchor);
        }

        public bool IsGrabbed() => grabbed;
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            velocity = -velocity / 2;
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            velocity = 0;
            isResting = true;
        }
    }
}
