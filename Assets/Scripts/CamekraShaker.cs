using UnityEditor;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    private static bool _isShaking = false;
    private static float _shakeMagnitude = 0f;
    private static float _shakeTime = 0f;

#if UNITY_EDITOR //Domain‚ÌƒŠƒ[ƒh‚Í‚µ‚È‚¢‚½‚ßAstatic•Ï”‚Íè“®‚Å‰Šú‰»‚·‚é
    private void Start()
    {
        EditorApplication.playModeStateChanged += OnExitPlayMode;
    }

    private void OnExitPlayMode(PlayModeStateChange st)
    {
        if (st != PlayModeStateChange.ExitingPlayMode) { return; }

        _isShaking = false;
        _shakeMagnitude = 0f;
        _shakeTime = 0f;

        EditorApplication.playModeStateChanged -= OnExitPlayMode;
    }
#endif

    private void LateUpdate()
    {
        if (!_isShaking) { return; }

        //U“®ŠÔ’†‚ÍƒJƒƒ‰‚ğU“®‚³‚¹‚é
        if (_shakeTime > 0f)
        {
            float x = Random.Range(-1f, 1f) * _shakeMagnitude;
            float y = Random.Range(-1f, 1f) * _shakeMagnitude;

            var position = transform.localPosition;
            position.x += x;
            position.y += y;
            transform.localPosition = position;

            _shakeTime -= Time.deltaTime;
        }
        else
        {
            _isShaking = false;
        }
    }

    /// <summary>
    /// ƒJƒƒ‰‚ğ—h‚ç‚·
    /// </summary>
    /// <param name="duration">U“®ŠÔ</param>
    /// <param name="magnitude">U“®‚Ì‹­‚³(Šî€’l‚Í -1 ~ 1)</param>
    public static void CameraShake(float duration, float magnitude)
    {
        _isShaking = true;

        _shakeMagnitude = magnitude;
        _shakeTime = duration;
    }
}