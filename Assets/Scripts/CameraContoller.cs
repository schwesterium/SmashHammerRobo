using System.Collections;
using UnityEngine;

//Create by Claude
public class CameraContoller : MonoBehaviour
{
    [Header("ズーム設定")]
    [Tooltip("ターゲットからどれだけ距離を取るか")]
    [SerializeField] private float _zoomDistance = 3f;

    [Tooltip("ターゲットからどれだけ高さをオフセットするか")]
    [SerializeField] private float _zoomHeight = 1.5f;

    [Tooltip("ズーム/リセット移動の速さ")]
    [SerializeField] private float _moveSpeed = 3f;

    [Tooltip("見た目の回転が変わる速さ")]
    [SerializeField] private float _rotateSpeed = 5f;

    // ズーム対象
    private Transform _target;

    // 初期状態(リセット用)
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    // 現在実行中のコルーチン管理用
    private Coroutine _currentRoutine = null;

    private void Awake()
    {
        // 起動時の位置・回転を保存しておく
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    /// <summary>
    /// ズーム対象のTransformをセットする
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    /// <summary>
    /// SetTargetでセットしたTransformにズームしてLookAtする
    /// </summary>
    public void StartZoom()
    {
        if (_target == null)
        {
            Debug.LogWarning("[CameraFollowZoom] ターゲットが未設定です。SetTarget()を先に呼んでください。");
            return;
        }

        // ターゲットの後ろ上方に寄った位置を計算
        Vector3 dirFromTarget = (transform.position - _target.position);
        if (dirFromTarget.sqrMagnitude < 0.0001f)
        {
            // カメラとターゲットがほぼ同じ位置の場合は適当な方向を使う
            dirFromTarget = -_target.forward;
        }
        dirFromTarget.Normalize();

        Vector3 destination = _target.position + dirFromTarget * _zoomDistance + Vector3.up * _zoomHeight;

        Quaternion destRotation = Quaternion.LookRotation(_target.position - destination);

        MoveTo(destination, destRotation, lookAtTarget: _target);
    }

    /// <summary>
    /// カメラを起動時の初期位置・回転に戻す
    /// </summary>
    public void ResetView()
    {
        MoveTo(_initialPosition, _initialRotation, lookAtTarget: null);
    }

    /// <summary>
    /// 指定位置・回転へ滑らかに移動する共通処理
    /// </summary>
    private void MoveTo(Vector3 destination, Quaternion destRotation, Transform lookAtTarget)
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }

        _currentRoutine = StartCoroutine(MoveRoutine(destination, destRotation, lookAtTarget));
    }

    private IEnumerator MoveRoutine(Vector3 destination, Quaternion destRotation, Transform lookAtTarget)
    {
        while (Vector3.Distance(transform.position, destination) > 0.01f ||
               Quaternion.Angle(transform.rotation, destRotation) > 0.1f)
        {
            // Lerpではなく距離ベースのMoveTowardsにすると「近づかない」が起きにくい
            transform.position = Vector3.MoveTowards(
                transform.position, destination, _moveSpeed * Time.deltaTime);

            Quaternion targetRotation = lookAtTarget != null
                ? Quaternion.LookRotation(lookAtTarget.position - transform.position)
                : destRotation;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, _rotateSpeed * 50f * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        transform.rotation = destRotation;
        _currentRoutine = null;
    }
}
