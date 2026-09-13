using System;
using UnityEngine;
using UnityEngine.UI;

namespace HammerSmash
{
    /// <summary>
    /// リザルトUIを管理するクラス
    /// </summary>
    public class ResultUIManager : MonoBehaviour
    {
        [SerializeField]
        private Button _retryButton = null;
        /// <summary>
        /// タイトルボタンを参照する変数
        /// </summary>
        [SerializeField]
        private Button _titleButton = null;

        public event Action OnClickRetryButton;
        public event Action OnClickTitleButton;

        [SerializeField]
        private Animator _resultAnimator = null;

        [SerializeField]
        private TransitionUIManager _transitionUI = null;

        public TransitionUIManager TransitionUI { get => _transitionUI; }

        private static readonly int s_introId = Animator.StringToHash("OnResultIntro");
        private static readonly int s_outroId = Animator.StringToHash("OnResultOutro");

        public void Init()
        {
            _retryButton.onClick.AddListener(() => OnClickRetryButton?.Invoke());
            _titleButton.onClick.AddListener(() => OnClickTitleButton?.Invoke());

            // --- 最初はボタンの判定をオフ ---
            _retryButton.enabled = false;
            _titleButton.enabled = false;

            // 最初はUIを非表示
            SetActive(false);
            _transitionUI.SetActive(false);
        }

        private void OnDestroy()
        {
            _retryButton.onClick.RemoveAllListeners();
            _titleButton.onClick.RemoveAllListeners();

            OnClickRetryButton = null;
            OnClickTitleButton = null;
        }

        /// <summary>
        /// UIを表示・非表示にする関数
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            // 子オブジェクトを全てチェック
            foreach (Transform child in transform)
            {
                // 子オブジェクトを表示・非表示
                child.gameObject.SetActive(isActive);
            }
        }

        public void PlayIntro() => _resultAnimator.SetTrigger(s_introId);
        public void PlayOutro()
        {
            _transitionUI.SetActive(true);
            _resultAnimator.SetTrigger(s_outroId);
        }

        public void ButtonEnable(bool v)
        {
            _retryButton.enabled = v;
            _titleButton.enabled = v;

            if (v) { _retryButton.Select(); }
        }
    }
}