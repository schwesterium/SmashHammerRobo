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

using UnityEngine;
using UnityEngine.Audio;

//BGMやSEの音量を管理するためのクラス
//AudioMixerを使用して音量を調整する

namespace AkaneTools
{
    public class VolumeSetter
    {
        private AudioMixer _audioMixer;

        private float _bgmDecibel = -10f;

        //BGMのボリュームを取得または設定する
        public float BGM_Volume
        {
            get
            {
                //デシベルをボリュームに変換する(10に decibelBGM / 20f 乗する)
                return Mathf.Pow(10f, _bgmDecibel / 20f);
            }
            set
            {
                //ボリュームをデシベルに変換する(log10をとって20をかける)
                float decibel = 20f * Mathf.Log10(value);
                //-80から0にClampする
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                _bgmDecibel = decibel;
                _audioMixer.SetFloat("BGMParam", decibel);
            }
        }

        private float _seDecibel = -10f;

        //SEのボリュームを取得または設定する
        public float SE_Volume
        {
            get
            {
                return Mathf.Pow(10f, _seDecibel / 20f);
            }
            set
            {
                //ボリュームをデシベルに変換する(log10をとって20をかける)
                float decibel = 20f * Mathf.Log10(value);
                //-80から0にClampする
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                _seDecibel = decibel;
                _audioMixer.SetFloat("SEParam", decibel);
            }
        }

        private float _uiDecibel = -10f;

        //UIのSEボリュームを取得または設定する
        public float UI_Volume
        {
            get
            {
                return Mathf.Pow(10f, _uiDecibel / 20f);
            }
            set
            {
                //ボリュームをデシベルに変換する(log10をとって20をかける)
                float decibel = 20f * Mathf.Log10(value);
                //-80から0にClampする
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                _uiDecibel = decibel;
                _audioMixer.SetFloat("UISoundParam", decibel);
            }
        }

        public VolumeSetter(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;

            //音量を初期の状態に設定する
            _audioMixer.SetFloat("BGMParam", _bgmDecibel);
            _audioMixer.SetFloat("SEParam", _seDecibel);
            _audioMixer.SetFloat("UISoundParam", _uiDecibel);
        }
    }
}