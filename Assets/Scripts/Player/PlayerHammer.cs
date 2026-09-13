using AkaneTools;
using System;
using System.Collections;
using UnityEngine;

namespace HammerSmash
{
    public class PlayerHammer : MonoBehaviour
    {
        private const float POWER_MIN = 0.1f;

        private const float POWER_MAX_RATIO_LOW = 0.25f;
        private const float POWER_MAX_RATIO_MEDIUM = 0.5f;
        private const float POWER_MAX_RATIO_HIHG = 0.75f;
        private const float POWER_MAX_RATIO_EXHIHG = 1f;

        [SerializeField]
        private TriggerChecker _damageColTrigger = null;

        [SerializeField]
        private Collider _damageCol = null;

        [SerializeField]
        private HammerParameter _hammerParameter = null;
        private float _invMaxPower = 0f;

        private float _chargeSpeedPer = 1f;
        private float _currentPower = POWER_MIN;

        public event Action OnEndAttack;

        [SerializeField]
        private Transform[] _effects = null;

        [SerializeField]
        private AnimationEventHandler _eventHandler = null;

        private Quaternion _startRotation = Quaternion.Euler(90f, 0f, -2.545f);
        private Quaternion _attackRotation = Quaternion.Euler(38.3f, 25f, 32f);

        private bool _isAttacking = false;

        public enum PowerChargeState
        {
            None,
            Low,
            Medium,
            High,
            ExHigh
        }

        private PowerChargeState _currentState = PowerChargeState.Low;

        //変数の変更を検知するため、こちらを常に使うこと
        public PowerChargeState CurrentState
        {
            get { return _currentState; }

            set
            {
                if (value == _currentState) { return; }

                _currentState = value;
                OnChangePowerChargeState?.Invoke(_currentState);
            }
        }

        public event Action<PowerChargeState> OnChangePowerChargeState;

        private void Update()
        {
            if (_isAttacking)
            {
                transform.localRotation = _attackRotation;
            }
            else
            {
                transform.localRotation = _startRotation;
            }
        }

        public void Init()
        {
            _damageColTrigger.OnEnter += OnEnter;
            _damageColTrigger.OnEnter -= OnExit;

            _currentPower = POWER_MIN;
            _currentState = PowerChargeState.Low;

            _invMaxPower = 1f / _hammerParameter.MaxPower;

            OnChangePowerChargeState += state => OnChangeState(state);

            _damageCol.enabled = false;

            FindFirstObjectByType<StageManager>().OnOverTimeStart += SetSpeedMul;

            _eventHandler.EventA += StartAttack;
            _eventHandler.EventB += DisableCollider;
            _eventHandler.EventC += EndAttack;
        }

        private void OnChangeState(PowerChargeState state)
        {
            switch (state)
            {
                case PowerChargeState.Low:
                    _effects[3].gameObject.SetActive(false);
                    _effects[0].gameObject.SetActive(true);
                    break;
                case PowerChargeState.Medium:
                    _effects[0].gameObject.SetActive(false);
                    _effects[1].gameObject.SetActive(true);
                    break;
                case PowerChargeState.High:
                    _effects[1].gameObject.SetActive(false);
                    _effects[2].gameObject.SetActive(true);
                    break;
                case PowerChargeState.ExHigh:
                    _effects[2].gameObject.SetActive(false);
                    _effects[3].gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void EnableEffect(PowerChargeState state, bool v)
        {
            switch (state)
            {
                case PowerChargeState.Low:
                    _effects[0].gameObject.SetActive(v);
                    break;
                case PowerChargeState.Medium:
                    _effects[1].gameObject.SetActive(v);
                    break;
                case PowerChargeState.High:
                    _effects[2].gameObject.SetActive(v);
                    break;
                case PowerChargeState.ExHigh:
                    _effects[3].gameObject.SetActive(v);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ハンマーのチャージを初期状態にする
        /// </summary>
        public void PowerReset()
        {
            _currentPower = POWER_MIN;
            EnableEffect(_currentState, false);
            CurrentState = PowerChargeState.None;
        }

        /// <summary>
        /// パワーをためる
        /// </summary>
        public void AccumulatePower()
        {
            if (_currentPower >= _hammerParameter.MaxPower)
            {
                _currentPower = _hammerParameter.MaxPower;
                return;
            }

            _currentPower += Time.fixedDeltaTime * _chargeSpeedPer;

            CurrentState = EvaluteChargeState();
        }

        //チャージ段階の判定
        private PowerChargeState EvaluteChargeState()
        {
            var ratio = _currentPower * _invMaxPower;

            if (POWER_MAX_RATIO_LOW >= ratio) { return PowerChargeState.Low; }
            else if (POWER_MAX_RATIO_MEDIUM >= ratio) { return PowerChargeState.Medium; }
            else if (POWER_MAX_RATIO_HIHG >= ratio) { return PowerChargeState.High; }
            else { return PowerChargeState.ExHigh; }
        }

        
        private void OnEnter(Collider obj)
        {
            var targetParent = obj.transform.parent;

            if (transform.parent != targetParent && targetParent.TryGetComponent<IAttackable>(out var attackable))
            {
                //ノックバックの方向
                var dir = targetParent.position - transform.parent.position;
                dir.y = 0.5f;
                dir.Normalize();

                float distance = _currentPower * _invMaxPower * _hammerParameter.MaxKnockBackDistance;

                var info = new AttackInfo(_currentPower, dir, distance, _currentState);

                //対象を攻撃する
                attackable.TakeAttack(info);
            }
        }
        private void OnExit(Collider obj)
        {

        }

        private void StartAttack()
        {
            _isAttacking = true;
            _damageCol.enabled = true;

            PlaySwingHammerSE();
        }

        private void DisableCollider()
        {
            _damageCol.enabled = false;
        }

        private void EndAttack()
        {
            _isAttacking = false;
            
            _effects[0].gameObject.SetActive(false);
            _effects[1].gameObject.SetActive(false);
            _effects[2].gameObject.SetActive(false);
            _effects[3].gameObject.SetActive(false);

            PowerReset();

            OnEndAttack?.Invoke();
        }

        private void PlaySwingHammerSE()
        {
            switch (_currentState)
            {
                case PowerChargeState.Low:
                    AudioManager.Instance.PlaySE("ShakeHammer1");
                    break;
                case PowerChargeState.Medium:
                    AudioManager.Instance.PlaySE("ShakeHammer2");
                    break;
                case PowerChargeState.High:
                    AudioManager.Instance.PlaySE("ShakeHammer3");
                    break;
                case PowerChargeState.ExHigh:
                    AudioManager.Instance.PlaySE("ShakeHammer4");
                    break;
                default:
                    break;
            }
        }

        private void SetSpeedMul() => _chargeSpeedPer += _hammerParameter.AddChargeSpeedMulPer;

        private void OnDestroy()
        {
            OnEndAttack = null;
        }
    }

}
