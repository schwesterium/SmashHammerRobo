using UnityEngine;

namespace HammerSmash
{
    /// <summary>
    /// タイトルUIを管理するクラス
    /// </summary>
    public class TransitionUIManager : MonoBehaviour
    {
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
    }
}