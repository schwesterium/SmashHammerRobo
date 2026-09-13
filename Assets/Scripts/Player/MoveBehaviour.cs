using AkaneUtility;
using System.Collections;
using UnityEngine;

namespace HammerSmash
{
    [RequireComponent(typeof(Rigidbody), typeof(CubeCaster))]
    public class MoveBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerParameter _playerParameter = null;

        protected Rigidbody _rb;

        private CubeCaster _cubeCaster = null;

        private float _speed = 0f;
        private float _speedMultiply = 1f;

        [SerializeField]
        private float _launchDuration = 0.15f; //吹っ飛び中の初速維持時間

        private bool isKnockBacking = false;

        /// <summary>
        /// MoveBehaviourの初期化をします
        /// </summary>
        protected void InitMoveBehaviour()
        {
            _cubeCaster = GetComponent<CubeCaster>();
            _rb = GetComponent<Rigidbody>();

            _speed = _playerParameter.WalkSpeed;
        }

        protected void OnFixedUpdate()
        {
            _cubeCaster.OnFixedUpdate();
        }

        /// <summary>
        /// キャラクターを移動させる
        /// </summary>
        /// <param name="direction">移動ベクトル</param>
        protected void Move(Vector3 direction)
        {
            var spd = _speed * _speedMultiply;

            var velocity = _rb.linearVelocity;
            velocity.x = direction.x * spd;
            velocity.z = direction.z * spd;
            _rb.linearVelocity = velocity;
        }

        /// <summary>
        /// y軸にキャラクターを回転させる
        /// </summary>
        /// <param name="targetHeading">向かせる方向</param>
        protected void Rotate(Quaternion targetHeading)
        {
            _rb.MoveRotation(targetHeading);
        }

        /// <summary>
        /// キャラクターをジャンプさせる
        /// </summary>
        /// <returns>ジャンプができたか</returns>
        protected bool Jump()
        {
            if (_cubeCaster.IsCasted)
            {
                _rb.AddForce(Vector3.up * _playerParameter.JumpPower, ForceMode.Impulse);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 後ろに吹き飛ばす
        /// </summary>
        /// <param name="power"></param>
        protected void KnockBack(Vector3 dir, float dis)
        {
            if (isKnockBacking) { return; }

            StartCoroutine(KnockBackRoutine(dir, dis));
        }

        private IEnumerator KnockBackRoutine(Vector3 dir, float distance)
        {
            isKnockBacking = true;

            _rb.isKinematic = true;

            float elapsed = 0f;
            Vector3 startPos = _rb.position;

            while (elapsed < _launchDuration)
            {
                elapsed += Time.fixedDeltaTime;

                float t = 1 - EasingUtility.EaseOutSine(Mathf.Clamp01(elapsed / _launchDuration));
                Vector3 offset = distance * t * dir;

                _rb.MovePosition(startPos + offset);

                yield return new WaitForFixedUpdate();
            }
            
            //飛び終わったら重力に任せて落下させる
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;

            isKnockBacking = false;
        }


        protected void SetOverTimeBaseSpeed()=> _speed += _playerParameter.WalkSpeed * _playerParameter.OverTimeAddPer;
        protected void SetSpeedMultiply(float mul) => _speedMultiply = mul;
        protected void SetSpeedMultiply() => _speedMultiply = 1;
    }

}