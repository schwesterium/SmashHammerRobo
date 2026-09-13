using UnityEngine;

namespace HammerSmash
{
    /// <summary>
    /// タイトルUIを管理するクラス
    /// </summary>
    public class TransitionUIManager : MonoBehaviour
    {
        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
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
    }
}