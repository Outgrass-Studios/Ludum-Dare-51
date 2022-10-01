using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC;
using qASIC.InputManagement;

namespace Game.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] float gravity;
        [SerializeField] float fallMultiplier;
        [SerializeField] float lowJumpMultiplier;
        [SerializeField] float jumpHeight = 3f;
        [SerializeField] Vector2 jumpCheckCenter;
        [SerializeField] Vector2 jumpCheckSize;
        [SerializeField] LayerMask jumpCheckLayer;

        [SerializeField] InputMapItemReference horizontal;
        [SerializeField] InputMapItemReference vertical;
        [SerializeField] InputMapItemReference jump;

        [Disable] [SerializeField] PlayerInput _input;
        [Disable] [SerializeField] bool _isGrounded;

        private void Reset()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            bool previousJump = _input.jump;
            bool jumpThisFramePrevious = _input.jumpThisFrame;
            bool jumpInput = InputManager.GetInput(jump.GetGroupName(), jump.GetItemName());

            _input = new PlayerInput()
            {
                move = new Vector2(InputManager.GetFloatInput(horizontal.GetGroupName(), horizontal.GetItemName()),
                InputManager.GetFloatInput(vertical.GetGroupName(), vertical.GetItemName())),
                jump = jumpInput,
                jumpThisFrame = (!previousJump && jumpInput) || jumpThisFramePrevious,
            };
        }

        private void FixedUpdate()
        {
            _isGrounded = IsGrounded();
            if (_input.jumpThisFrame && _isGrounded)
                rb.velocity = Vector2.up * Mathf.Sqrt(jumpHeight * 2f * gravity);

            rb.velocity -= Vector2.up * gravity * ((_input.jump ? fallMultiplier : lowJumpMultiplier) - 1) * Time.fixedDeltaTime;

            if (_input.jumpThisFrame)
                _input.jumpThisFrame = false;
        }

        bool IsGrounded() =>
            Physics2D.OverlapBoxAll((Vector2)transform.position + jumpCheckCenter, jumpCheckSize, 0f, jumpCheckLayer).Length != 0;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + (Vector3)jumpCheckCenter, new Vector3(jumpCheckSize.x, jumpCheckSize.y, 0.1f));
        }

        [System.Serializable]
        public struct PlayerInput
        {
            public Vector2 move;
            public bool jump;
            public bool jumpThisFrame;
        }
    }
}