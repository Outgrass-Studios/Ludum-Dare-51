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

        [Header("Movement")]
        [SerializeField] float walkSpeed;
        [SerializeField] float groundAcceleration;
        [SerializeField] float groundDeceleration;
        [SerializeField] float airAcceleration;
        [SerializeField] float airDeceleration;

        [Header("Gravity")]
        [SerializeField] float jumpGravity;
        [SerializeField] float fallGravity;
        [SerializeField] float fallMultiplier;
        [SerializeField] float lowJumpMultiplier;
        [SerializeField] float jumpHeight = 3f;
        [SerializeField] Vector2 jumpCheckCenter;
        [SerializeField] Vector2 jumpCheckSize;
        [SerializeField] LayerMask jumpCheckLayer;

        [Header("Input")]
        [SerializeField] InputMapItemReference horizontal;
        [SerializeField] InputMapItemReference vertical;
        [SerializeField] InputMapItemReference jump;

        PlayerInput _input;
        bool _isGrounded;

        float _lastJumpTime;
        bool _isJumping;

        private void Reset()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            ReadInput();

            qDebug.DisplayValue("move", _input.move);
            qDebug.DisplayValue("jump", _input.jump);
            qDebug.DisplayValue("jumpThisFrame", _input.jumpThisFrame);
        }

        private void FixedUpdate()
        {
            Move(_isGrounded ? groundAcceleration : airAcceleration, _isGrounded ? groundDeceleration : airDeceleration);
            Gravity();

            qDebug.DisplayValue("velocity", rb.velocity);
        }

        void ReadInput()
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

        void Move(float acceleration, float deceleration, float lerp = 1f)
        {
            float targetSpeed = _input.move.x * walkSpeed;
            targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerp);
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

            accelerationRate = 50 * accelerationRate / walkSpeed;

            float speedDifference = targetSpeed - rb.velocity.x;
            float movement = speedDifference * accelerationRate;

            rb.AddForce(movement * Vector2.right);
        }

        void Gravity()
        {
            bool isGroundedPrevious = _isGrounded;
            _isGrounded = IsGrounded();

            float gravity = rb.velocity.y >= 0f ? jumpGravity : fallGravity;

            switch (_isGrounded)
            {
                case true:
                    if (!isGroundedPrevious)
                        _isJumping = false;

                    if (_input.jumpThisFrame)
                    {
                        rb.velocity = new Vector2(rb.velocity.x,
                            Mathf.Sqrt(jumpHeight * 2f * jumpGravity));
                        _lastJumpTime = Time.time;
                        _isJumping = true;
                    }
                    break;
                case false:
                    rb.velocity -= Vector2.up * gravity * ((_input.jump ? fallMultiplier : lowJumpMultiplier) - 1) * Time.fixedDeltaTime;
                    break;
            }

            if (_input.jumpThisFrame)
                _input.jumpThisFrame = false;

            qDebug.DisplayValue("_isGrounded", _isGrounded);
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