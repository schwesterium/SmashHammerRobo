using UnityEngine;
using UnityEngine.UI;

namespace HammerSmash
{
    /// <summary>
    /// タイトルUIを管理するクラス
    /// </summary>
    public class TitleUIManager : MonoBehaviour
    {
        /// <summary>
        /// スタートボタンを参照する変数
        /// </summary>
        public Button StartButton;
        /// <summary>
        /// ゲーム終了ボタンを参照する変数
        /// </summary>
        public Button ExitButton;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            StartButton.Select();

            // --- 最初はボタンの判定をオフ ---
            StartButton.enabled = false;
            ExitButton.enabled = false;
        }
    }
}