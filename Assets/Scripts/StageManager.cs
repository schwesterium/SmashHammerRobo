using AkaneTools;
using SchwesteriumLibrary.Input;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HammerSmash
{
    /// <summary>
    /// メインステージシーン処理を管理するクラス
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        /// <summary>
        /// メインステージUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private StageUIManager _stageUIManager;
        /// <summary>
        /// 遷移UI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private TransitionUIManager _transitionUIManager;
        /// <summary>
        /// ローカルマルチ操作デバイス管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private LocalMultiplayManager _localMultiplayManager;
        /// <summary>
        /// リザルトUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private ResultUIManager _resultUIManager;
        /// <summary>
        /// ゲームイントロUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private MainGameIntroUIManager _mainGameIntroUIManager;
        /// <summary>
        /// カウントダウンUI管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private CountDownUIManager _countDownUIManager;

        /// <summary>
        /// プレイヤー管理クラスを参照する変数
        /// </summary>
        [SerializeField]
        private PlayerController[] _playerControllers;

        /// <summary>
        /// 操作説明のUIを参照する変数
        /// </summary>
        [SerializeField]
        private Transform _controlUI;

        /// <summary>
        /// アニメーターを参照する変数
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// 第一アウトロ演出トリガーを参照する変数
        /// </summary>
        private static readonly int _outro1TriggerID = Animator.StringToHash("OnOutro1");

        /// <summary>
        /// ゲームの演出時間を参照する変数
        /// </summary>
        private float _gameAnimTime = 1.0f;
        /// <summary>
        /// ゲームスタートイントロ演出時間を参照する変数
        /// </summary>
        private float _mainGameStartIntroTime = 9.9f;
        /// <summary>
        /// リザルト演出時間を参照する変数
        /// </summary>
        private float _resultAnimTime = 2.0f;
        /// <summary>
        /// ロビー時のカウントダウン演出時間を参照する変数
        /// </summary>
        private float _lobbyCountAnimTime = 4.0f;
        /// <summary>
        /// 経過時間（0からスタート）を参照する変数
        /// </summary>
        public float _timeElapsed = 0f;
        /// <summary>
        /// 時間の最大値を参照する変数
        /// </summary>
        private float _maxTime = 3599f;

        /// <summary>
        /// タイマーが動くかを判別するフラグを参照する変数
        /// </summary>
        private bool _isTimerRunning = true;
        /// <summary>
        /// 経過時間がカンストしたことを判別するフラグを参照する変数
        /// </summary>
        private bool _isMaxTimeLimit = false;

        /// <summary>
        /// 生存人数を管理する変数
        /// </summary>
        private int _alivePlayerCount = 0;
        private int _currentLobbyPlayerCount = 0;

        private bool _isOverTime = false;

        public event Action OnOverTimeStart;

        [SerializeField]
        private TriggerChecker _lobbyStartArea = null;

        //===add tosa====
        private enum GameState
        {
            Lobby,
            Intro,
            Play,

        }
        private GameState _currentState = GameState.Lobby;

        private CameraContoller _cameraContoller = null;
        //========================

        /// <summary>
        /// 初期設定を行う関数
        /// </summary>
        private void Awake()
        {
            // --- コンポーネントの登録 ---
            _animator = GetComponent<Animator>();

            // --- 各ボタンに関数を登録 ---
            _resultUIManager.RetryButton.onClick.AddListener(Retry);
            _resultUIManager.TitleButton.onClick.AddListener(Title);

            // デバイス管理の初期設定を行う
            _localMultiplayManager.Init();
        }

        /// <summary>
        /// 初回起動時に処理を行う関数
        /// </summary>
        private void Start()
        {
            _cameraContoller = Camera.main.GetComponent<CameraContoller>();

            // 現在あるデバイスを設定
            //_localMultiplayManager.RegisterAllDevices();

            _lobbyStartArea.OnEnter += col => LobbyEnter(col);
            _lobbyStartArea.OnExit += col => LobbyExit(col);

            //コントローラの接続チェック開始
            _localMultiplayManager.StartReceiving();

            AudioManager.Instance.PlayBGM("Lobby");

            // メインステージシーンのイントロ演出再生
            StartCoroutine(StageIntroAnimCoroutine());
        }

        /// <summary>
        /// 毎フレーム処理を行う関数
        /// </summary>
        private void Update()
        {
            switch (_currentState)
            {
                case GameState.Lobby:
                    break;
                case GameState.Intro:
                    break;
                case GameState.Play:
                    //もしタイマーが動くフラグオンでないならなにもしない by tosa
                    if (!_isTimerRunning) { return; }

                    // 毎フレーム経過時間を足す
                    _timeElapsed += Time.deltaTime;

                    // もし制限時間が設定されている場合
                    if (_isMaxTimeLimit && _timeElapsed >= _maxTime)
                    {
                        // --- 時間を超えないようにストップ ---
                        _timeElapsed = _maxTime;
                        _isTimerRunning = false;
                    }

                    //90sec経過したら延長戦 tosa
                    if (!_isOverTime && _timeElapsed >= 90f)
                    {
                        _isOverTime = true;
                        OnOverTimeStart?.Invoke();
                        AudioManager.Instance.PlayBGM("OverTime");

                    }

                    if (_isOverTime)
                    {
                        _time += Time.deltaTime;

                        if (_time >= 30f)
                        {
                            _time -= 30f;

                            OnOverTimeStart?.Invoke();
                        }
                    }

                    // 現在の時間をUIとして表示
                    _stageUIManager.DisplayTime(_timeElapsed);

                    break;
                default:
                    break;
            }
        }
        float _time = 0f;
        private void LobbyEnter(Collider col)
        {
            ++_currentLobbyPlayerCount;

            if (_currentLobbyPlayerCount <= 1) { return; }

            if (_currentLobbyPlayerCount == _localMultiplayManager.ActivePlayerCount)
            {
                // カウントダウン処理開始（中山が追加）
                CountDown(CountDownUIManager.LobbyCountTriggerID);
            }
        }

        private void LobbyExit(Collider col)
        {
            --_currentLobbyPlayerCount;
        }

        public void StartGame()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");

            foreach (var p in players)
            {
                p.GetComponent<PlayerController>().OnGameStart();
            }

            // 参加人数を生存人数として初期設定する
            _alivePlayerCount = _localMultiplayManager.ActivePlayerCount;

            AudioManager.Instance.PlayBGM("Main");
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
        /// リザルト処理を行う関数
        /// </summary>
        private void Result()
        {
            // リザルト表示時の処理を呼び出し
            StartCoroutine(ResultAnimCoroutine());
        }

        /// <summary>
        /// リトライ処理を行う関数
        /// </summary>
        private void Retry()
        {          
            _alivePlayerCount = _localMultiplayManager.ActivePlayerCount;

            _animator.enabled = true;

            StartCoroutine(RetryGame());
        }

        private IEnumerator RetryGame()
        {
            //リザルトアウトロアニメーション
            _resultUIManager.ResultAnimator.SetTrigger(ResultUIManager.ResultOutroTriggerID);
            yield return new WaitForSeconds(_gameAnimTime);
            _resultUIManager.ShowHide(false);

            _currentState = GameState.Intro;

            AudioManager.Instance.PlayBGM("Main");

            for (int i = 0; i < _alivePlayerCount; ++i)
            {
                var pc = _playerControllers[i];

               pc.gameObject.SetActive(true);
               pc.OnGameStart();
               pc.SleepEnable(true);
            }

            //遷移アニメーション
            _transitionUIManager.ShowHide(true);
            _animator.SetTrigger(_outro1TriggerID);
            yield return new WaitForSeconds(_gameAnimTime);
            _transitionUIManager.ShowHide(false);

            //カウントダウンアニメーション
            _mainGameIntroUIManager.ShowHide(true);
            CountDown(CountDownUIManager.MainGameCountTriggerID);
            yield return new WaitForSeconds(_mainGameStartIntroTime);
            _mainGameIntroUIManager.ShowHide(false);

            foreach (PlayerController playerController in _playerControllers)
            {
                playerController.SleepEnable(false);
            }

            _currentState = GameState.Play;
        }

        /// <summary>
        /// ゲームスタートイントロ処理を行う関数
        /// </summary>
        private void GameStartIntro()
        {
            // アウトロ演出時の処理を呼び出し
            StartCoroutine(MainGameIntroAnimCoroutine());
        }

        /// <summary>
        /// タイトル遷移処理を行う関数
        /// </summary>
        private void Title()
        {
            // アウトロ演出時の処理を呼び出し
            StartCoroutine(OutroAnimCoroutine("TitleScene"));
        }

        /// <summary>
        /// リザルト表示時の処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResultAnimCoroutine()
        {
            // プレイヤー操作管理クラスを全て参照
            foreach (PlayerController playerController in _playerControllers)
            {
                // プレイヤー操作禁止
                playerController.SleepEnable(true);
            }

            // リザルトUI表示
            _resultUIManager.ShowHide(true);
            // 演出トリガーを起動
            _resultUIManager.ResultAnimator.SetTrigger(ResultUIManager.ResultIntroTriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_resultAnimTime);

            _resultUIManager.ButtonEnable(true);
        }

        /// <summary>
        /// アウトロ演出時の処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator OutroAnimCoroutine(string sceneName)
        {
            // 演出用UI表示
            _resultUIManager.ShowHide(true);
            // 演出トリガーを起動
            _resultUIManager.ResultAnimator.SetTrigger(ResultUIManager.ResultOutroTriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 指定のシーンに遷移
            MoveScene(sceneName);
        }

        /// <summary>
        /// ゲームイントロ時の処理を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator MainGameIntroAnimCoroutine()
        {
            _currentState = GameState.Intro;

            // プレイヤー操作管理クラスを全て参照
            foreach (PlayerController playerController in _playerControllers)
            {
                // プレイヤー操作禁止
                playerController.SleepEnable(true);
            }

            // 演出用UI表示
            _transitionUIManager.ShowHide(true);
            // 演出トリガーを起動
            _animator.SetTrigger(_outro1TriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 操作説明のUIを隠す
            _stageUIManager.TargetShowHide(_controlUI,false);

            //コントローラの接続チェック終了
            _localMultiplayManager.EndReceiving();
            _localMultiplayManager.SetActiveUnassignedPlayer(false);

            // メインゲームの初期設定処理を行う
            StartGame();
            // メインゲームイントロUI表示
            _mainGameIntroUIManager.ShowHide(true);
            // カウントダウンを開始
            CountDown(CountDownUIManager.MainGameCountTriggerID);
            // 演出時間分待機
            yield return new WaitForSeconds(_mainGameStartIntroTime);
            // メインゲームイントロUI非表示
            _mainGameIntroUIManager.ShowHide(false);
            // 演出用UI非表示
            _transitionUIManager.ShowHide(false);

            // プレイヤー操作管理クラスを全て参照
            foreach (PlayerController playerController in _playerControllers)
            {
                // プレイヤー操作許可
                playerController.SleepEnable(false);
            }

            // ゲームの状態をプレイへ変更（中山が編集）
            _currentState = GameState.Play;
        }

        /// <summary>
        /// メインステージ時のイントロ処理・演出を行うコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator StageIntroAnimCoroutine()
        {
            // 演出用UI表示
            _transitionUIManager.ShowHide(true);
            // 演出時間分待機
            yield return new WaitForSeconds(_gameAnimTime);
            // 演出用UI非表示
            _transitionUIManager.ShowHide(false);
        }

        /// <summary>
        /// プレイヤーが死亡した時に外部（PlayerControllerなど）から呼ばれる関数
        /// </summary>
        /// <param name="transform"></param>
        /// <returns>リスポーンの可否</returns>
        public bool OnPlayerDied()
        {
            //ロビーにいる場合はリスポーンさせる
            //リスポーンさせたら処理は終了ですわ by tosa
            if (_currentState == GameState.Lobby) { return true; }

            // 生存人数を1減らす
            _alivePlayerCount--;

            // もし生存人数が1人以下になったら
            if (_alivePlayerCount <= 1)
            {
                // --- メインゲーム終了処理 ---
                // タイマーを止める
                _isTimerRunning = false;

                _animator.enabled = false;

                StartCoroutine(SearchPlayer());

                AudioManager.Instance.PlayBGM("Result");

                // リザルト画面へ遷移
                Result();
            }

            return false;
        }

        private IEnumerator SearchPlayer()
        {
            yield return null;

            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                if (!p.activeSelf) { continue; }

                _cameraContoller.SetTarget(p.transform);
                _cameraContoller.StartZoom();

                p.transform.position = new Vector3(0f, 10f, 0f);

                p.GetComponent<PlayerController>().ImWin();
            }
        }

        private void OnDestroy()
        {
            OnOverTimeStart = null;
        }

        /// <summary>
        /// カウントダウンを行う関数
        /// </summary>
        /// <param name="animNumber"></param>
        private void CountDown(int animNumber)
        {
            // カウントダウン処理を呼び出し
            StartCoroutine(CountDownCoroutine(animNumber));
        }

        /// <summary>
        /// カウントダウンの演出および処理を行うコルーチン
        /// </summary>
        /// <param name="animNumber"></param>
        /// <returns></returns>
        private IEnumerator CountDownCoroutine(int animNumber)
        {
            // もし指定のアニメーションIDがメインゲーム用なら
            if (animNumber == CountDownUIManager.LobbyCountTriggerID)
            {
                // ロビーのゲーム開始ポイントを削除
                _lobbyStartArea.gameObject.SetActive(false);

                // カウントダウンUIを表示
                _countDownUIManager.ShowHide(true);
                // 演出を再生
                _countDownUIManager.CountDownAnimator.SetTrigger(CountDownUIManager.LobbyCountTriggerID);
                // 演出時間分待機
                yield return new WaitForSeconds(_lobbyCountAnimTime);
                // カウントダウンUIを非表示
                _countDownUIManager.ShowHide(false);

                // メインゲームイントロ開始（中山が編集）
                GameStartIntro();
            }
            else
            {
                // カウントダウンUIを表示
                _countDownUIManager.ShowHide(true);
                // 演出を再生
                _countDownUIManager.CountDownAnimator.SetTrigger(CountDownUIManager.MainGameCountTriggerID);
                // 演出時間分待機
                yield return new WaitForSeconds(_mainGameStartIntroTime);
                // カウントダウンUIを非表示
                _countDownUIManager.ShowHide(false);
            }
        }
    }
}