using AkaneTools;
using SchwesteriumLibrary.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HammerSmash
{
    public class PlayerController : MoveBehaviour, IAttackable, IInputHandlerOwner<PlayerInputHandler>
    {
        //移動の閾値
        private const float MOVE_THRESHOLD = 0.1f;
        private Vector3 RESPAWN_POINT = new Vector3(2.5f, 10f, -14f);

        [SerializeField]
        private Vector3 startPosition;

        private PlayerInputHandler _inputHandler = null;
        public PlayerInputHandler InputHandler { get { return _inputHandler; } }

        [SerializeField]
        private PlayerHammer _hammer = null;

        private enum State
        {
            Sleep,
            Idle,
            Walk,
            Dash,
            AttackHold,
            Attack,
            Stun,
            Dead
        }
        private State _currentState = State.Idle;

        private Vector2 _moveInput = Vector2.zero;

        [SerializeField]
        private float _stunTime = 1f;

        private float _stateTimer = 0f;

        public event Action OnDeadEvent;

        [SerializeField]
        private Animator _animator = null;
        private static readonly int s_isWalkId = Animator.StringToHash("IsWalk");
        private static readonly int s_attackId = Animator.StringToHash("Attack");
        private static readonly int s_endAttackId = Animator.StringToHash("EndAttack");

        private void Awake()
        {
            _inputHandler = new();

            _inputHandler.OnMove += HandleMove;
            _inputHandler.OnJump += HandleJump;
            _inputHandler.OnAttack += ctx => HandleAttack(ctx);

            InitMoveBehaviour();

            _hammer.Init();
            _hammer.OnEndAttack += EndAttack;
            _hammer.OnChangePowerChargeState += state => OnChangeHammerState(state);
        }


        private void Start()
        {
            FindFirstObjectByType<StageManager>().OnOverTimeStart += SetOverTimeBaseSpeed;
        }

        private void FixedUpdate()
        {
            if (_currentState == State.Dead || _currentState == State.Sleep) { return; }

            OnFixedUpdate();

            switch (_currentState)
            {
                case State.Idle:
                    if (IsMoving(_moveInput))
                    {
                        _currentState = State.Walk;
                        return;
                    }
                    break;
                case State.Walk:
                    if (!IsMoving(_moveInput))
                    {
                        Move(Vector3.zero);
                        _moveInput = Vector2.zero;
                        _currentState = State.Idle;
                        _animator.SetBool(s_isWalkId, false);
                        return;
                    }

                    _animator.SetBool(s_isWalkId, true);

                    Movement();
                    break;
                case State.Dash:
                    break;
                case State.AttackHold:

                    _hammer.AccumulatePower();

                    if (!IsMoving(_moveInput))
                    {
                        Move(Vector3.zero);
                        _moveInput = Vector2.zero;
                        _animator.SetBool(s_isWalkId, false);
                        return;
                    }

                    _animator.SetBool(s_isWalkId, true);

                    Movement();
                    break;
                case State.Attack:
                    break;
                case State.Stun:
                    //スタン時間中は動けないようにする
                    _stateTimer += Time.fixedDeltaTime;

                    if (_stateTimer >= _stunTime)
                    {
                        _stateTimer = 0f;
                        _currentState = IsMoving(_moveInput) ? State.Walk : State.Idle;
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnGameStart()
        {
            _hammer.PowerReset();
            _rb.position = startPosition;
        }

        public void OnRetry()
        {
            OnGameStart();

            _rb.isKinematic = false;

            _inputHandler.OnMove += HandleMove;
            _inputHandler.OnJump += HandleJump;
            _inputHandler.OnAttack += ctx => HandleAttack(ctx);

            _hammer.OnEndAttack += EndAttack;
            _hammer.OnChangePowerChargeState += state => OnChangeHammerState(state);
        }

        //移動しているかどうかを判定する
        private bool IsMoving(Vector2 vector2)
        {
            return Mathf.Abs(vector2.x) > MOVE_THRESHOLD || Mathf.Abs(vector2.y) > MOVE_THRESHOLD;
        }

        //移動処理
        private void Movement()
        {
            if (_rb.isKinematic) { return; }

            //入力のMoveアクションを三次元ベクトル化
            var inputDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

            //inputDirectionから角度を計算
            var inputAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

            //移動、回転する
            var targetHeading = Quaternion.AngleAxis(inputAngle, new Vector3(0f, 1f, 0f));
            Rotate(targetHeading);
            var moveDirection = _rb.rotation * new Vector3(0f, 0f, 1f);
            Move(moveDirection);
        }

        public void OnDead()
        {
            AudioManager.Instance.PlaySE("WaterHit");

            // ステージ管理クラスを見つけて登録（中山が追加）
            StageManager stageManager = FindFirstObjectByType<StageManager>();

            // もしステージ管理クラスがある場合（中山が追加）
            if (stageManager != null)
            {
                // ステージ管理クラスに死亡したことを通知（中山が追加）
                if (stageManager.OnPlayerDied())
                {
                    _rb.position = RESPAWN_POINT;
                    return;
                }
            }

            if (_currentState == State.Dead) { return; }

            _currentState = State.Dead;

            _inputHandler.OnMove -= HandleMove;
            _inputHandler.OnJump -= HandleJump;
            _inputHandler.OnAttack -= ctx => HandleAttack(ctx);

            _hammer.OnEndAttack -= EndAttack;
            _hammer.OnChangePowerChargeState -= state => OnChangeHammerState(state);

            _rb.isKinematic = true;
            gameObject.SetActive(false);

            OnDeadEvent?.Invoke();
        }

        private void OnChangeHammerState(PlayerHammer.PowerChargeState state)
        {
            switch (state)
            {
                case PlayerHammer.PowerChargeState.Low:
                    SetSpeedMultiply(1f);
                    break;
                case PlayerHammer.PowerChargeState.Medium:
                    SetSpeedMultiply(0.9f);
                    break;
                case PlayerHammer.PowerChargeState.High:
                    SetSpeedMultiply(0.75f);
                    break;
                case PlayerHammer.PowerChargeState.ExHigh:
                    SetSpeedMultiply(0.5f);
                    break;
                default:
                    break;
            }
        }

        #region Handler
        private void HandleMove(Vector2 moveDirection)
        {
            _moveInput = moveDirection;
        }
        private void HandleJump()
        {
            if (_currentState == State.Sleep) { return; }

            Jump();
        }
        private void HandleAttack(InputAction.CallbackContext ctx)
        {
            if(_currentState == State.Sleep) { return; }

            if (ctx.phase == InputActionPhase.Started)
            {
                if (_currentState == State.AttackHold || _currentState == State.Attack) { return; }

                _hammer.PowerReset();
                _currentState = State.AttackHold;
            }

            if (ctx.phase == InputActionPhase.Canceled)
            {
                if (_currentState != State.AttackHold) { return; }

                Debug.Log(_animator == null);
                Debug.Log(ReferenceEquals(_animator, null));
                _animator.SetBool(s_isWalkId, false);
                _animator.SetTrigger(s_attackId);
                _currentState = State.Attack;
            }
        }
        #endregion

        private void EndAttack()
        {
            SetSpeedMultiply();

            _animator.SetBool(s_isWalkId, IsMoving(_moveInput));
            _animator.SetBool(s_endAttackId, true);

            if(_currentState == State.Sleep) { return; }

            _currentState = IsMoving(_moveInput) ? State.Walk : State.Idle;
        }

        public void TakeAttack(AttackInfo info)
        {
            _currentState = State.Stun;
            _hammer.PowerReset();

            switch (info.ChargeState)
            {
                case PlayerHammer.PowerChargeState.Low:
                    AudioManager.Instance.PlaySE("Hit1");
                    break;
                case PlayerHammer.PowerChargeState.Medium:
                    AudioManager.Instance.PlaySE("Hit2");
                    break;
                case PlayerHammer.PowerChargeState.High:
                    AudioManager.Instance.PlaySE("Hit3");
                    break;
                case PlayerHammer.PowerChargeState.ExHigh:
                    AudioManager.Instance.PlaySE("Hit4");
                    CameraShaker.CameraShake(0.1f, 1f);
                    break;
                case PlayerHammer.PowerChargeState.Max:
                    AudioManager.Instance.PlaySE("Hit4");
                    CameraShaker.CameraShake(0.1f, 1f);
                    break;
                default:
                    break;
            }

            KnockBack(info.KnockBackDirection, info.KnockBackDistance);
        }

        public void SleepEnable(bool v) => _currentState = v ? State.Sleep : State.Idle;

        public void ImWin()
        {
            _animator.SetBool(s_isWalkId, true);
        }

        private void OnDestroy()
        {
            _inputHandler.Dispose();
            _animator = null;
        }

        PlayerInputHandler IInputHandlerOwner<PlayerInputHandler>.GetInputHandler()
        {
            return _inputHandler;
        }
    }
}