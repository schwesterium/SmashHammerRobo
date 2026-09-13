using UnityEngine;

namespace HammerSmash
{
    /// <summary>
    /// ゲームイントロUIを管理するクラス
    /// </summary>
    public class CountDownUIManager : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator = null;

        private static readonly int s_lobbyCountId = Animator.StringToHash("OnLobbyCount");
        private static readonly int s_mainGameCountId = Animator.StringToHash("OnMainGameCount");

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // 最初はUIを非表示
            SetActive(false);
        }

        public void PlayCountLobby() => _animator.SetTrigger(s_lobbyCountId);
        public void PlayCountMainGame() => _animator.SetTrigger(s_mainGameCountId);

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