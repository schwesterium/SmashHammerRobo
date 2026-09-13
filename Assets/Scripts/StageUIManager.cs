using UnityEngine;
using UnityEngine.UI;

namespace HammerSmash
{
    /// <summary>
    /// メインステージUIを管理するクラス
    /// </summary>
    public class StageUIManager : MonoBehaviour
    {
        /// <summary>
        /// 数字の画像 (0～9の順番で10個セットする)を参照する変数
        /// </summary>
        [SerializeField]
        private Sprite[] _numberSprites;

        /// <summary>
        /// 分における10の位を参照する変数
        /// </summary>
        [SerializeField]
        private Image _minuteTensImage;
        /// <summary>
        /// 分における1の位を参照する変数
        /// </summary>
        [SerializeField]
        private Image _minuteUnitsImage;
        /// <summary>
        /// 秒における10の位を参照する変数
        /// </summary>
        [SerializeField]
        private Image _secondTensImage;
        /// <summary>
        /// 秒における1の位を参照する変数
        /// </summary>
        [SerializeField]
        private Image _secondUnitsImage;

        /// <summary>
        /// UIを指定して表示・非表示する関数
        /// </summary>
        public void TargetShowHide(Transform target,bool isActive)
        {
            // 指定のオブジェクトを表示・非表示
            target.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 現在の時間を表示する関数
        /// </summary>
        /// <param name="timeToDisplay"></param>
        public void DisplayTime(float timeToDisplay)
        {
            // --- 分と秒の計算 ---
            int minutes = Mathf.FloorToInt(timeToDisplay / 60);
            int seconds = Mathf.FloorToInt(timeToDisplay % 60);

            // もし99分59秒を超えた場合
            if (minutes > 99&&seconds >59)
            {
                // --- 99分59秒で固定 ---
                minutes = 99;
                seconds = 59;
            }

            // --- 各桁の数値を計算 ---
            int minTens = minutes / 10;
            int minUnits = minutes % 10;
            int secTens = seconds / 10;
            int secUnits = seconds % 10;

            // もしスプライト配列が正しくセットされている（0〜9の10個ある）場合
            if (_numberSprites != null && _numberSprites.Length >= 10)
            {
                // もし分における10の位画像がある場合
                if (_minuteTensImage != null)
                {
                    // 算出した桁の数字をインデックスとして、配列から画像をセットする
                    _minuteTensImage.sprite = _numberSprites[minTens];
                }

                // もし分における1の位画像がある場合
                if (_minuteUnitsImage != null)
                {
                    // 算出した桁の数字をインデックスとして、配列から画像をセットする
                    _minuteUnitsImage.sprite = _numberSprites[minUnits];
                }

                // もし秒における10の位画像がある場合
                if (_secondTensImage != null)
                {
                    // 算出した桁の数字をインデックスとして、配列から画像をセットする
                    _secondTensImage.sprite = _numberSprites[secTens];
                }

                // もし秒における1の位画像がある場合
                if (_secondUnitsImage != null)
                {
                    // 算出した桁の数字をインデックスとして、配列から画像をセットする
                    _secondUnitsImage.sprite = _numberSprites[secUnits];
                }
            }
        }
    }
}