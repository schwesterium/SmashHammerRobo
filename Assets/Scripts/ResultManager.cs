using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HammerSmash
{
/// <summary>
/// リザルトシーン処理を管理するクラス
/// </summary>
    public class ResultManager : MonoBehaviour
    {
        /// <summary>
        /// リザルトUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private ResultUIManager _resultUI_Manager;

        /// <summary>
        /// 遷移UI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private TransitionUIManager _transitionUI_Manager;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// タイトル遷移時の演出トリガーを参照する変数
        /// </summary>
        private static readonly int _TitleTriggerID = Animator.StringToHash("OnTitle");

        /// <summary>
        /// ゲームの演出時間を参照する変数
        /// </summary>
        private float _gameAnimTime = 1.0f;

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Start()
        {
            // --- コンポーネントの登録 ---
            _animator = GetComponent<Animator>();

            // --- ボタンイベントの登録 ---
            _resultUI_Manager.TitleButton.onClick.AddListener(GoTitle);

            // リザルトシーンのイントロ演出再生
            StartCoroutine(ResultIntroAnimCoroutine());
        }

        /// <summary>
        /// シーンを遷移する関数
        /// </summary>
        /// <param name="name"></param>
        private void MoveScene(string name)
        { 
            SceneManager.LoadScene(name);
        }

        /// <summary>
        /// タイトルシーンに遷移する関数
        /// </summary>
        private void GoTitle()
        {
            // タイトル遷移時の処理を呼び出し
            StartCoroutine(TitleAnimCoroutine());
        }

        /// <summary>
        /// タイトル遷移時の処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator TitleAnimCoroutine()
        {
            // 演出用UI表示
            _transitionUI_Manager.ShowHide(true);
            // 演出トリガーを起動
            _animator.SetTrigger(_TitleTriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 指定のシーンに遷移
            MoveScene("TitleScene");
        }

        /// <summary>
        /// リザルト時のイントロ処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResultIntroAnimCoroutine()
        {
            // 演出用UI表示
            _transitionUI_Manager.ShowHide(true);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 演出用UI非表示
            _transitionUI_Manager.ShowHide(false);

            // --- ボタンの判定をオン ---
            _resultUI_Manager.TitleButton.enabled = true;
        }
    }
}