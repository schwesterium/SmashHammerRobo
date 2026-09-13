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
using System.Collections.Generic;
using UnityEngine;

namespace AkaneTools
{
    //ランタイムで編集すると中身が壊れます！！！
    //Listの表示はEditorでのみ有効

    [Serializable]
    public class SerializableDictionary<Tkey, Tvalue>
    {
        [SerializeField]
        private List<Pair<Tkey, Tvalue>> _keyValueList = new();

        private Dictionary<Tkey, Tvalue> _dictionary = null;

        /// <summary>
        /// Dictionaryがnullでない場合はDictionaryを作成します
        /// </summary>
        /// <returns>Dictionaryが作成できたかどうか</returns>
        public bool TryCreateDictionary()
        {
            //Dictionaryが生成済みなら処理は行わない
            if (_dictionary != null) { return false; }

            _dictionary = new();

            //生成したDictionaryにkeyとvalueを追加する
            foreach (var pair in _keyValueList)
            {
                _dictionary.Add(pair.Key, pair.Value);
            }

            return true;
        }

        /// <summary>
        /// 要素を追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddElement(Tkey key, Tvalue value)
        {
            if (_dictionary == null)
            {
                Debug.LogWarning("_dictionaryがnullです！");
                return;
            }

            _dictionary.Add(key, value);

#if UNITY_EDITOR
            //Inspector上の表示も更新する
            _keyValueList.Add(new Pair<Tkey, Tvalue>(key, value));
#endif
        }

        /// <summary>
        /// 指定した要素を更新する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateElementValue(Tkey key, Tvalue value)
        {
            _dictionary[key] = value;
        }

        /// <summary>
        /// 要素を取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Tvalue GetElement(Tkey key)
        {
            return _dictionary[key];
        }

        /// <summary>
        /// 辞書内にKeyがあるかどうかを調べる
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Tkey key)
        {
            return _dictionary.ContainsKey(key);
        }

#if UNITY_EDITOR
        /// <summary>
        /// リストを最新の状態に更新する
        /// <para>※ Editorでのみ有効</para>
        /// </summary>
        public void UpdateList()
        {
            //前提として、_dictionaryが最新の状態、_keyValueListは更新前の状態

            for (int i = 0; i < _keyValueList.Count; i++)
            {
                //keyの取り出し
                var key = _keyValueList[i].Key;

                //keyが存在する場合は、Listの要素を更新する
                if (_dictionary.TryGetValue(key, out Tvalue value))
                {
                    _keyValueList[i] = new Pair<Tkey, Tvalue>(key, value);
                }
            }
        }
#endif
    }

    //keyとvalueを保存しておくクラス
    [Serializable]
    public class Pair<Tkey, Tvalue>
    {
        [SerializeField]
        private Tkey _key;
        [SerializeField]
        private Tvalue _value;

        public Pair(Tkey key, Tvalue value)
        {
            _key = key;
            _value = value;
        }

        public Tkey Key { get => _key; }
        public Tvalue Value { get => _value; }
    }
}