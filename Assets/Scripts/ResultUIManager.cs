using UnityEngine;
using UnityEngine.UI;

namespace HammerSmash
{
    /// <summary>
    /// リザルトUIを管理するクラス
    /// </summary>
    public class ResultUIManager : MonoBehaviour
    {
        /// <summary>
        /// リトライボタンを参照する変数
        /// </summary>
        public Button RetryButton;
        /// <summary>
        /// タイトルボタンを参照する変数
        /// </summary>
        public Button TitleButton;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        public Animator ResultAnimator;

        /// <summary>
        /// リザルトイントロ演出トリガーを参照する変数
        /// </summary>
        public static readonly int ResultIntroTriggerID = Animator.StringToHash("OnResultIntro");
        /// <summary>
        /// リザルトアウトロ演出トリガーを参照する変数
        /// </summary>
        public static readonly int ResultOutroTriggerID = Animator.StringToHash("OnResultOutro");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- 最初はボタンの判定をオフ ---
            RetryButton.enabled = false;
            TitleButton.enabled = false;

            // 最初はUIを非表示
            ShowHide(false);
        }

        /// <summary>
        /// UIを表示・非表示にする関数
        /// </summary>
        /// <param name="isActive"></param>
        public void ShowHide(bool isActive)
        {
            // 子オブジェクトを全てチェック
            foreach (Transform child in transform)
            {
                // 子オブジェクトを表示・非表示
                child.gameObject.SetActive(isActive);
            }
        }

        public void ButtonEnable(bool v)
        {
            RetryButton.enabled = v;
            TitleButton.enabled = v;

            if (v) { RetryButton.Select(); }
        }
    }
}