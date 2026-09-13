using AkaneTools;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HammerSmash
{
/// <summary>
/// タイトルシーン処理を管理するクラス
/// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// タイトルUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private TitleUIManager _titleUI_Manager;

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
        /// ゲームスタート時の演出トリガーを参照する変数
        /// </summary>
        private static readonly int _startGameTriggerID = Animator.StringToHash("OnStart");
        /// <summary>
        /// ゲーム終了時の演出トリガーを参照する変数
        /// </summary>
        private static readonly int _exitGameTriggerID = Animator.StringToHash("OnExit");

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
            _titleUI_Manager.StartButton.onClick.AddListener(StartGame);
            _titleUI_Manager.ExitButton.onClick.AddListener(GameExit);

            AudioManager.Instance.PlayBGM("Title");

            // タイトルシーンのイントロ演出再生
            StartCoroutine(TitleIntroAnimCoroutine());
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
        /// ゲームを終了する関数
        /// </summary>
        private void GameExit()
        {
            // ゲーム終了の処理・演出を呼び出し
            StartCoroutine(ExitGameAnimCoroutine());
        }

        /// <summary>
        /// ゲームを開始する関数
        /// </summary>
        private void StartGame()
        {
            // ゲームスタート時の処理を呼び出し
            StartCoroutine(StartGameAnimCoroutine());
        }

        /// <summary>
        /// ゲームスタート時の処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartGameAnimCoroutine()
        {
            // 演出用UI表示
            _transitionUI_Manager.ShowHide(true);
            // 演出トリガーを起動
            _animator.SetTrigger(_startGameTriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 指定のシーンに遷移
            MoveScene("StageScene");
        }

        /// <summary>
        /// ゲーム終了時の処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExitGameAnimCoroutine()
        {
            // 演出用UI表示
            _transitionUI_Manager.ShowHide(true);
            // 演出トリガーを起動
            _animator.SetTrigger(_exitGameTriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);

            // --- ゲームを終了する ---
            Debug.Log("ゲーム終了");
            Application.Quit();
        }

        /// <summary>
        /// ゲームスタート時の処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator TitleIntroAnimCoroutine()
        {
            // 演出用UI表示
            _transitionUI_Manager.ShowHide(true);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 演出用UI非表示
            _transitionUI_Manager.ShowHide(false);

            // --- ボタンの判定をオン ---
            _titleUI_Manager.StartButton.enabled = true;
            _titleUI_Manager.ExitButton.enabled = true;
        }
    }
}