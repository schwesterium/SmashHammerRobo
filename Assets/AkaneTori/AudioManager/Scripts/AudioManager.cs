//======================================================================================

//Copyright (c) 2026 SveaGUN
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the “Software”), to deal in the Software without 
//restriction, including without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
//Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or 
//substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//OTHER DEALINGS IN THE SOFTWARE.

//======================================================================================

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;


namespace AkaneTools
{
    //BGMとSE、音量を管理するクラス
    //BGMのフェードイン、フェードアウトができる
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("AudioClips")]

        [SerializeField]
        private AudioClipsData _bgmData = null;

        [SerializeField]
        private AudioClipsData _seData = null;

        [SerializeField]
        private AudioClipsData _uiData = null;

        [Header("AudioSources\n上はBGM,中はSE,下はUI")]
        [SerializeField]
        [Tooltip("BGMを再生するAudioSource")]
        private AudioSource _bgmSource = null;

        [SerializeField]
        [Tooltip("SEを再生するAudioSource")]
        private AudioSource _seSource = null;

        [SerializeField]
        [Tooltip("SEを再生するAudioSource")]
        private AudioSource _uiSource = null;

        //現在フェード中かどうか
        private bool _isBgmFading = false;

        public bool IsFading { get => _isBgmFading; }

        [SerializeField]
        private AudioMixer _audioMixer = null;

        public VolumeSetter Volume { get; private set; } = null;

        //====================================================================
        //初期化処理
        //====================================================================

        private void Awake()
        {
            //シングルトン
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                //辞書の初期化
                _bgmData.ClipsDict.TryCreateDictionary();
                _seData.ClipsDict.TryCreateDictionary();
                _uiData.ClipsDict.TryCreateDictionary();

                Volume = new(_audioMixer);
            }
            else
            {
                Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        private void Start()
        {
            EditorApplication.playModeStateChanged += OnExitPlayMode;
        }

        private void OnExitPlayMode(PlayModeStateChange st)
        {
            if (st != PlayModeStateChange.ExitingPlayMode) { return; }

            Instance = null;

            EditorApplication.playModeStateChanged -= OnExitPlayMode;
        }
#endif

        //====================================================================
        //BGM処理
        //====================================================================

        /// <summary>
        /// BGMを再生する。フェード中は再生できない
        /// </summary>
        /// <param name="bgmName">BGM名</param>
        public void PlayBGM(string bgmName, float volume = 1f)
        {
            if (!_bgmData.ClipsDict.ContainsKey(bgmName))
            {
                Debug.LogError($"BGM {bgmName} は存在しません");
                return;
            }

            if (_isBgmFading)
            {
                Debug.LogError("BGMがフェード中です");
                return;
            }

            //BGMが再生中なら停止する
            if (_bgmSource.isPlaying) { _bgmSource.Stop(); }

            //BGMを再生する
            _bgmSource.clip = _bgmData.ClipsDict.GetElement(bgmName);
            _bgmSource.volume = volume;
            _bgmSource.Play();
        }

        public void PauseBGM()
        {
            _bgmSource.Pause();
        }

        /// <summary>
        /// BGMを停止する。フェード中は停止不可
        /// </summary>
        public void StopBGM()
        {
            if (_isBgmFading)
            {
                Debug.LogError("BGMがフェード中です");
                return;
            }

            _bgmSource.Stop();
        }

        /// <summary>
        /// BGMを強制停止する。フェード中でも停止可
        /// </summary>
        public void ForcedStopBGM()
        {
            if (_isBgmFading)
            {
                StopAllCoroutines(); //全てのコルーチンを停止
                _isBgmFading = false; //フェード中フラグを解除
            }

            _bgmSource.Stop();
            Debug.LogWarning("BGMを強制停止しました");
        }

        //====================================================================
        //BGMフェードイン・フェードアウト処理
        //====================================================================

        //1は最大音量、0は最小音量

        /// <summary>
        /// BGMをフェードインさせる
        /// </summary>
        /// <remarks>
        /// <para>フェードイン時間 : 1 / fadeInAmount</para>
        /// フェードインの総フレーム数 : 1 / (fadeInAmount / FPS)
        /// </remarks>
        /// <param name="fadeInAmount">値が大きいほど、早くフェードインする</param>
        /// <param name="targetVolume">目標ボリューム</param>
        /// <param name="bgmName">BGM名</param>
        /// <param name="callback">フェードイン後に実行する関数</param>
        public void FadeInBGM(float fadeInAmount, float targetVolume = 1, string bgmName = null, Action callback = null)
        {
            if (_isBgmFading)
            {
                Debug.LogWarning("フェード中です");
                return;
            }

            if (fadeInAmount <= 0)
            {
                Debug.LogWarning("fadeInAmount は 0 を超える値にしてください");
                return;
            }

            targetVolume = Mathf.Clamp01(targetVolume);

            if (!string.IsNullOrEmpty(bgmName)) { PlayBGM(bgmName, 0); }

            //フェードイン開始
            _isBgmFading = true;
            StartCoroutine(OnFadeInBGM(fadeInAmount, targetVolume, callback));
        }

        //BGMをフェードインさせる
        private IEnumerator OnFadeInBGM(float fadeInAmount, float volume, Action callback)
        {
            Debug.Log("BGMフェードイン開始");

            //フェードイン
            while (_bgmSource.volume < volume)
            {
                _bgmSource.volume += fadeInAmount * Time.deltaTime;
                yield return null;
            }

            //音量を指定した音量にする
            _bgmSource.volume = volume;

            //フェードイン終了
            _isBgmFading = false;
            callback?.Invoke();
        }

        /// <summary>
        /// BGMをフェードアウトさせる
        /// </summary>
        /// <remarks>
        /// <para>フェードアウト時間 : 1 / fadeOutAmount</para>
        /// フェードアウトの総フレーム数 : 1 / (fadeOutAmount / FPS)
        /// </remarks>
        /// <param name="fadeOutAmount">値が大きいほど、早くフェードアウトする</param>
        /// <param name="targetVolume">目標ボリューム</param>
        /// <param name="isStop">フェードアウト終了時に、BGMを停止するか</param>
        /// <param name="callback">フェードアウト後に実行する関数</param>
        public void FadeOutBGM(float fadeOutAmount, float targetVolume = 0, bool isStop = false, Action callback = null)
        {
            if (_isBgmFading)
            {
                Debug.LogWarning("フェード中です");
                return;
            }

            if (fadeOutAmount <= 0)
            {
                Debug.LogWarning("fadeOutAmount は 0 を超える値にしてください");
                return;
            }

            targetVolume = Mathf.Clamp01(targetVolume);

            //フェードアウト開始
            _isBgmFading = true;
            StartCoroutine(OnFadeOutBGM(fadeOutAmount, isStop, targetVolume, callback));
        }

        //BGMをフェードアウトさせる
        private IEnumerator OnFadeOutBGM(float fadeOutAmount, bool isStop, float volume, Action callback)
        {
            //フェードアウト
            while (_bgmSource.volume > volume)
            {
                _bgmSource.volume -= fadeOutAmount * Time.deltaTime;
                yield return null;
            }

            _bgmSource.volume = volume;

            if (isStop) { _bgmSource.Stop(); }

            //フェードアウト終了
            _isBgmFading = false;
            Debug.Log("BGMフェードアウト終了");
            callback?.Invoke();
        }

        //====================================================================
        //SE処理
        //====================================================================

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="seName">SE名</param>
        /// <param name="volume">音量</param>
        public void PlaySE(string seName, float volume = 1)
        {
            if (!_seData.ClipsDict.ContainsKey(seName))
            {
                Debug.LogError($"SE {seName} は存在しません");
                return;
            }

            //SEを再生する
            _seSource.PlayOneShot(_seData.ClipsDict.GetElement(seName), volume);
        }

        /// <summary>
        /// UIのSEを再生する
        /// </summary>
        /// <param name="seName"></param>
        /// <param name="volume">音量</param>
        public void PlayUI(string seName, float volume = 1)
        {
            if (!_uiData.ClipsDict.ContainsKey(seName))
            {
                Debug.LogError($"UI_SE {seName} は存在しません");
                return;
            }

            //SEを再生する
            _uiSource.PlayOneShot(_uiData.ClipsDict.GetElement(seName), volume);
        }
    }
}