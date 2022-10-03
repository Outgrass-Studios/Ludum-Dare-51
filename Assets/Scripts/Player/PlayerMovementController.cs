using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC;
using qASIC.InputManagement;
using Game.SpriteAnimations;
using Game.Checkpoints;

namespace Game.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] GrapplingHook grapplingHook;

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
        [SerializeField] float jumpCheckSize;
        [SerializeField] LayerMask jumpCheckLayer;

        [Header("Wall jumping & climbing")]
        [SerializeField] Vector2 wallCheckCenter;
        [SerializeField] Vector2 wallCheckSize;
        [SerializeField] LayerMask wallCheckLayer;
        [SerializeField] float wallClimbGravityThreashold = 0.3f;
        [SerializeField] float wallSlideSpeed;
        [SerializeField] float wallClimbSpeed;
        [SerializeField] float wallClimbJump;
        [SerializeField] float wallJumpXForce;

        [Header("Assist")]
        [SerializeField] float coyoteTime = 0.2f;
        [SerializeField] float coyoteWallJumpTime = 0.2f;
        [SerializeField] float jumpQueue = 0.2f;

        [Header("Stamina")]
        [SerializeField] float maximumStamina = 20f;
        [SerializeField] float grabStamina = 3.5f;
        [SerializeField] float climbStamina = 4f;

        [Header("Trampoline")]
        [SerializeField] [TagSelector] string trampolineTag;
        [SerializeField] float trampolineJumpHeight;

        [Header("Animation")]
        [SerializeField] SpriteAnimator anim;
        [SerializeField] SpriteAnimation idleAnimation;
        [SerializeField] SpriteAnimation runAnimation;
        [SerializeField] SpriteAnimation climbAnimation;
        [SerializeField] SpriteAnimation climbIdleAnimation;

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

        float _lastWallSlideTime;
        float _lastGrabTime;
        bool _canCoyote;

        bool _flipDirection = false;
        bool _wasLastWallFlipped = false;

        bool _canControlAirTime;

        float _stamina;

        Collider2D[] _groundColliders;

        private void Reset()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Awake()
        {
            LevelCheckpointManager.OnStateReverted += LevelCheckpointManager_OnStateReverted;
        }

        private void LevelCheckpointManager_OnStateReverted()
        {
            _stamina = maximumStamina;
            _flipDirection = false;
            _wasLastWallFlipped = false;
        }

        private void Update()
        {
            ReadInput();

            HandleGrappleInput();

            qDebug.DisplayValue("move", _input.move);
            qDebug.DisplayValue("jump", _input.jump);
            qDebug.DisplayValue("jumpThisFrame", _input.jumpThisFrame);
        }

        private void FixedUpdate()
        {
            if(!grapplingHook.IsGrabbed())
                Gravity();

            if (!_isGrabing)
                Move(_isGrounded ? groundAcceleration : airAcceleration, _isGrounded ? groundDeceleration : airDeceleration);

            if (_input.jumpThisFrame)
                _input.jumpThisFrame = false;

            SetAnimation();

            qDebug.DisplayValue("_isGrounded", _isGrounded);

            qDebug.DisplayValue("velocity", rb.velocity);
            qDebug.DisplayValue("_canCoyote", _canCoyote);
            qDebug.DisplayValue("_isJumping", _isJumping);
            qDebug.DisplayValue("_stamina", _stamina);
        }

        void SetAnimation()
        {
            if (anim == null) return;
            SpriteAnimation currentAnimation;

            switch (_isGrabing)
            {
                case false:
                    currentAnimation = idleAnimation;
                    if (_input.move.x != 0f)
                        currentAnimation = runAnimation;
                    break;
                default:
                    currentAnimation = climbIdleAnimation;
                    if (_input.move.y != 0f)
                        currentAnimation = climbAnimation;
                    break;
            }

            anim.spriteRenderer.flipX = _flipDirection;
            anim.ChangeAnimation(currentAnimation);
        }

        void ReadInput()
        {
            bool previousJump = _input.jump;
            bool jumpThisFramePrevious = _input.jumpThisFrame;
            bool jumpInput = InputManager.GetInput(jump.GetGroupName(), jump.GetItemName());
            float jumpPressedTime = InputManager.GetInputDown(jump.GetGroupName(), jump.GetItemName()) ? Time.time : _input.jumpPressedTime;

            _input = new PlayerInput()
            {
                move = new Vector2(InputManager.GetFloatInput(horizontal.GetGroupName(), horizontal.GetItemName()),
                InputManager.GetFloatInput(vertical.GetGroupName(), vertical.GetItemName())),
                jump = jumpInput,
                jumpThisFrame = (!previousJump && jumpInput) || jumpThisFramePrevious,
                jumpPressedTime = jumpPressedTime,
                grab = InputManager.GetInput(grab.GetGroupName(), grab.GetItemName()),
                grapple = InputManager.GetInput(grapple.GetGroupName(), grapple.GetItemName())
            };

            if (_input.move.x != 0f && !_isGrabing)
                _flipDirection = _input.move.x < 0f;
        }

        void HandleGrappleInput()
        {
            if (InputManager.GetInputDown(grapple.GetItemName()) && !_isGrounded)
                grapplingHook.Grab(_input.move);
            if (InputManager.GetInputUp(grapple.GetItemName()))
                grapplingHook.LetGo();
            if (InputManager.GetInputDown(grappleJump.GetItemName()) && _input.grapple)
                grapplingHook.Jump();
        }

        void Move(float acceleration, float deceleration, float lerp = 1f)
        {
            float targetSpeed = _input.move.x * walkSpeed;

            targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerp);

            bool useAcceleration = (Mathf.Abs(targetSpeed) > 0.01f) || Mathf.Abs(rb.velocity.x) > walkSpeed;
            float accelerationRate = useAcceleration ? acceleration : deceleration;

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

            foreach (var collider in _groundColliders)
            {
                if (collider.CompareTag(trampolineTag))
                {
                    Jump(trampolineJumpHeight, false);
                    _isGrounded = false;
                }
            }


            _isGrabing = _isTouchingWall && _input.grab && (gravityThreasholdReached || _isGrabing) && _stamina > 0f;
            _isWallSliding = !_isGrounded && _isTouchingWall && isMovingHoritontally && gravityThreasholdReached && !_isGrabing;
     
            if (IsTouchingAnyWall() && _input.jumpThisFrame)
            {
                if (!_isTouchingWall)
                    _wasLastWallFlipped = !_flipDirection;

                WallJump();
            }

            if (_isGrounded)
                _stamina = maximumStamina;

            if (_isWallSliding)
            {
                _wasLastWallFlipped = _flipDirection;
                _canCoyote = true;
                _isJumping = false;
                _lastWallSlideTime = Time.time;
                rb.velocity = Vector2.down * wallSlideSpeed;

                return;
            }

            if (_isGrabing)
            {
                _stamina -= (_input.move.y == 0f ? climbStamina : grabStamina) * Time.fixedDeltaTime;
                _wasLastWallFlipped = _flipDirection;
                _canCoyote = true;
                _isJumping = false;
                _lastGrabTime = Time.time;
                rb.velocity = new Vector2(0f, _input.move.y * wallClimbSpeed);

                return;
            }


            switch (_isGrounded)
            {
                case true:
                    if (!isGroundedPrevious)
                    {
                        _isJumping = false;
                        _canCoyote = true;
                        if ((Time.time - _input.jumpPressedTime) <= jumpQueue)
                            _input.jumpThisFrame = true;
                    }

                    if (_input.jumpThisFrame)
                        Jump(jumpHeight);
                    break;
                case false:
                    if (isGroundedPrevious)
                        _lastGroundTime = Time.time;

                    HandleCoyote();

                    rb.velocity -= Vector2.up * gravity * ((_input.jump && _canControlAirTime && rb.velocity.y >= 0f ? fallMultiplier : lowJumpMultiplier) - 1) * Time.fixedDeltaTime;
                    break;
            }
        }

        void HandleCoyote()
        {
            if (CanJump(Mathf.Max(_lastGrabTime, _lastWallSlideTime), coyoteWallJumpTime))
            {
                WallJump();
                return;
            }

            if (CanJump(_lastGroundTime, coyoteTime))
                Jump(jumpHeight);

            bool CanJump(float time, float coyoteTime) =>
                (Time.time - time) <= coyoteTime && _canCoyote && !_isJumping && _input.jumpThisFrame;
        }

        void Jump(float jumpHeight, bool canControlAirTime = true)
        {
            _canControlAirTime = canControlAirTime;
            _canCoyote = false;

            rb.velocity = new Vector2(rb.velocity.x,
                Mathf.Sqrt(jumpHeight * 2f * jumpGravity));
            _isJumping = true;
        }

        void WallJump()
        {
            //rb.AddForce(wallSlideJump.x * (_wasLastWallFlipped ? 1f : -1f) * Vector2.right, ForceMode2D.Impulse);
            rb.AddForce(Vector2.right * wallJumpXForce * (_wasLastWallFlipped ? 1f : -1f), ForceMode2D.Impulse);
            Jump(wallClimbJump);
            _isWallSliding = false;
            _isGrabing = false;
        }

        bool IsGrounded() =>
            (_groundColliders = Physics2D.OverlapCircleAll((Vector2)transform.position + jumpCheckCenter, jumpCheckSize, jumpCheckLayer)).Length != 0;

        bool IsTouchingWall() =>
            IsTouchingWall(_flipDirection);

        bool IsTouchingWall(bool flip)
        {
            Vector2 center = wallCheckCenter;
            center.x *= flip ? -1f : 1f;

            return CheckBox(center, wallCheckSize, wallCheckLayer);
        }
        
        bool IsTouchingAnyWall() =>
            IsTouchingWall(false) || IsTouchingWall(true);

        bool CheckBox(Vector2 center, Vector2 size, LayerMask layer) =>
            Physics2D.OverlapBoxAll((Vector2)transform.position + center, size, 0f, layer).Length != 0;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3)jumpCheckCenter, jumpCheckSize);
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
        }
    }
}