using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC;
using qASIC.InputManagement;
using UnityEngine.Windows;

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

        [Header("Wall jumping & climbing")]
        [SerializeField] Vector2 wallCheckCenter;
        [SerializeField] Vector2 wallCheckSize;
        [SerializeField] LayerMask wallCheckLayer;
        [SerializeField] float wallClimbGravityThreashold = 0.3f;
        [SerializeField] float wallSlideSpeed;
        [SerializeField] float wallClimbSpeed;
        [SerializeField] float wallClimbJump;
        [SerializeField] Vector2 wallSlideJump;

        [Header("Assist")]
        [SerializeField] float coyoteTime = 0.2f;
        [SerializeField] float jumpQueue = 0.2f;

        [Header("Input")]
        [SerializeField] InputMapItemReference horizontal;
        [SerializeField] InputMapItemReference vertical;
        [SerializeField] InputMapItemReference jump;
        [SerializeField] InputMapItemReference grab;
        [SerializeField] InputMapItemReference grapple;
        [SerializeField] InputMapItemReference grappleJump;

        PlayerInput _input = new PlayerInput()
        { 
            jumpPressedTime = float.MinValue,
        };
        bool _isGrounded;
        bool _isTouchingWall;

        float _lastGroundTime;
        bool _isJumping;
        bool _isGrabing;
        bool _isWallSliding;

        bool _flipDirection = false;

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
            qDebug.DisplayValue("grapple", _input.grapple);
            qDebug.DisplayValue("grappleJump", _input.grappleJump);
        }

        private void FixedUpdate()
        {
            Gravity();

            if (!_isGrabing)
                Move(_isGrounded ? groundAcceleration : airAcceleration, _isGrounded ? groundDeceleration : airDeceleration);

            if (_input.jumpThisFrame)
                _input.jumpThisFrame = false;

            qDebug.DisplayValue("_isGrounded", _isGrounded);

            qDebug.DisplayValue("velocity", rb.velocity);
        }

        void ReadInput()
        {
            bool previousJump = _input.jump;
            bool jumpThisFramePrevious = _input.jumpThisFrame;
            bool jumpInput = InputManager.GetInput(jump.GetGroupName(), jump.GetItemName());
            bool grappleInput = InputManager.GetInput(grapple.GetGroupName(), grapple.GetItemName());
            bool grappleJumpInput = InputManager.GetInput(grappleJump.GetGroupName(), grappleJump.GetItemName());
            float jumpPressedTime = InputManager.GetInputDown(jump.GetGroupName(), jump.GetItemName()) ? Time.time : _input.jumpPressedTime;

            if (grappleInput == !_input.grapple)
            {
                if(grappleInput)
                    Grab();
                else
                    LetGo();
            }

            if (grappleJumpInput == !_input.grappleJump && grappleJumpInput)
                GrappleJump();

            _input = new PlayerInput()
            {
                move = new Vector2(InputManager.GetFloatInput(horizontal.GetGroupName(), horizontal.GetItemName()),
                InputManager.GetFloatInput(vertical.GetGroupName(), vertical.GetItemName())),
                jump = jumpInput,
                jumpThisFrame = (!previousJump && jumpInput) || jumpThisFramePrevious,
                jumpPressedTime = jumpPressedTime,
                grab = InputManager.GetInput(grab.GetGroupName(), grab.GetItemName()),
                grapple = grappleInput,
                grappleJump = grappleJumpInput
            };

            if (_input.move.x != 0f && !_isGrabing)
                _flipDirection = _input.move.x < 0f;
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
            _isTouchingWall = IsTouchingWall();
            bool isGroundedPrevious = _isGrounded;
            _isGrounded = IsGrounded();

            float gravity = rb.velocity.y >= 0f ? jumpGravity : fallGravity;

            bool isMovingHoritontally = _input.move.x != 0f;
            bool gravityThreasholdReached = rb.velocity.y <= wallClimbGravityThreashold;

            if (!_isGrounded)
            {
                _isGrabing = _isTouchingWall && _input.grab && (gravityThreasholdReached || _isGrabing); 
                _isWallSliding = _isTouchingWall && isMovingHoritontally && gravityThreasholdReached && !_isGrabing;

                if (_isWallSliding)
                {
                    rb.velocity = Vector2.down * wallSlideSpeed;

                    if (_input.jumpThisFrame)
                    {
                        Debug.Log("JUMP");
                        rb.AddForce(wallSlideJump.x * (_flipDirection ? 1f : -1f) * Vector2.right, ForceMode2D.Impulse);
                        Jump(wallSlideJump.y);
                        _isWallSliding = false;
                    }

                    return;
                }

                if (_isGrabing)
                {
                    rb.velocity = new Vector2(0f, _input.move.y * wallClimbSpeed);

                    if (_input.jumpThisFrame)
                    {
                        Jump(wallClimbJump);
                        _isGrabing = false;
                    }
                    return;
                }
            }

            switch (_isGrounded)
            {
                case true:
                    if (!isGroundedPrevious)
                    {
                        _isJumping = false;
                        if ((Time.time - _input.jumpPressedTime) <= jumpQueue)
                            _input.jumpThisFrame = true;
                    }

                    if (_input.jumpThisFrame)
                        Jump(jumpHeight);
                    break;
                case false:
                    if (isGroundedPrevious)
                        _lastGroundTime = Time.time;

                    if ((Time.time - _lastGroundTime) <= coyoteTime && !_isJumping && _input.jumpThisFrame)
                        Jump(jumpHeight);

                    rb.velocity -= Vector2.up * gravity * ((_input.jump && rb.velocity.y >= 0f ? fallMultiplier : lowJumpMultiplier) - 1) * Time.fixedDeltaTime;
                    break;
            }
        }

        void Jump(float jumpHeight)
        {
            rb.velocity = new Vector2(rb.velocity.x,
                Mathf.Sqrt(jumpHeight * 2f * jumpGravity));
            _isJumping = true;
        }
        void Grab()
        {
            Vector2 dir = _input.move;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);

            Debug.Log("Grabbed");
            // Swing mechanics
            // Collision handling
        }
        void LetGo()
        {
            Debug.Log("Let go");
        }
        void GrappleJump()
        {
            Debug.Log("Grapple jumped");
        }

        bool IsGrounded() =>
            CheckBox(jumpCheckCenter, jumpCheckSize, jumpCheckLayer);

        bool IsTouchingWall()
        {
            Vector2 center = wallCheckCenter;
            center.x *= _flipDirection ? -1f : 1f;

            return CheckBox(center, wallCheckSize, wallCheckLayer);
        }

        bool CheckBox(Vector2 center, Vector2 size, LayerMask layer) =>
            Physics2D.OverlapBoxAll((Vector2)transform.position + center, size, 0f, layer).Length != 0;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + (Vector3)jumpCheckCenter, new Vector3(jumpCheckSize.x, jumpCheckSize.y, 0.1f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + (Vector3)wallCheckCenter, new Vector3(wallCheckSize.x, wallCheckSize.y, 0.1f));
        }

        [System.Serializable]
        public struct PlayerInput
        {
            public Vector2 move;
            public bool jump;
            public bool jumpThisFrame;
            public float jumpPressedTime;
            public bool grab;
            public bool grapple;
            public bool grappleJump;
        }
    }
}