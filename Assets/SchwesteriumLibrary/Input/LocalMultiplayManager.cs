/*
Author : schwesterium
Date   : 2026/08/01
*/

using HammerSmash;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SchwesteriumLibrary.Input
{
    public class LocalMultiplayManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _playerPrefabs = new GameObject[2];

        [SerializeField]
        private int _maxPlayers = 4;

        [SerializeField]
        private bool _useKeyboard = true;

        [SerializeField]
        private bool _initResister = true;

        private readonly List<PlayerInputHandler> _activePlayers = new();
        private readonly HashSet<InputDevice> _pairedDevices = new();

        private bool IsReceiving = false;

        public bool UseKeyboard { get => _useKeyboard; private set => _useKeyboard = value; }
        public int ActivePlayerCount => _activePlayers.Count;

        public void Init()
        {
            //デバイス切断の監視
            InputSystem.onDeviceChange += OnDeviceChange;

            if (_initResister)
            {
                RegisterAllDevices();
            }
        }

        public void SetActiveUnassignedPlayer(bool v)
        {
            for (int i = 0; i < _maxPlayers - _activePlayers.Count; ++i)
            {
                Debug.Log(i);
                _playerPrefabs[_playerPrefabs.Length - i - 1].SetActive(v);
            }
        }

        public void StartReceiving() => IsReceiving = true;
        public void EndReceiving() => IsReceiving = false;

        /// <summary>
        /// 
        /// </summary>
        public void RegisterAllDevices()
        {
            var devices = InputSystem.devices;

            foreach (var device in devices)
            {
                Debug.Log(device.name);

                //プレイヤー数の上限チェック
                if (_activePlayers.Count >= _maxPlayers) { return; }

                //既に参加済みのデバイスは無視
                if (_pairedDevices.Contains(device)) { continue; }

                if (UseKeyboard)
                {
                    //ゲームパッドとキーボードを登録する
                    if (device is Gamepad || device is Keyboard)
                    {
                        JoinPlayer(device);
                    }
                }
                else
                {
                    //ゲームパッドのみ登録する
                    if (device is Gamepad)
                    {
                        JoinPlayer(device);
                    }
                }

            }
        }

        private void RegisterDevice(InputDevice device)
        {
            if (!IsReceiving) { return; }

            //プレイヤー数の上限チェック
            if (_activePlayers.Count >= _maxPlayers) { return; }

            //既に参加済みのデバイスは無視
            if (_pairedDevices.Contains(device)) { return; }

            if (UseKeyboard)
            {
                //ゲームパッドとキーボードを登録する
                if (device is Gamepad || device is Keyboard)
                {
                    JoinPlayer(device);
                }
            }
            else
            {
                //ゲームパッドのみ登録する
                if (device is Gamepad)
                {
                    JoinPlayer(device);
                }
            }
        }

        private void JoinPlayer(InputDevice device)
        {
            var playerId = _activePlayers.Count;

            if (!_playerPrefabs[playerId].TryGetComponent<IInputHandlerOwner<PlayerInputHandler>>(out var owner))
            {
                Debug.LogError("ownerが見つかりません");
                return;
            }

            //デバイスの紐付け
            var handler = owner.GetInputHandler();

            handler.Join(device, playerId);

            //プレイヤーとデバイスの記録
            _activePlayers.Add(handler);
            _pairedDevices.Add(device);

            Debug.Log($"Player{++playerId} が参加しました{device.displayName}");
        }

        /// <summary>
        /// デバイスの接続状態が変化したとき（切断対応）
        /// </summary>
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    RegisterDevice(device);
                    break;
                case InputDeviceChange.Reconnected:
                    RegisterDevice(device);
                    break;

                case InputDeviceChange.Disconnected:
                    if (!_pairedDevices.Contains(device)) return;

                    //切断されたデバイスのプレイヤーを探す
                    var disconnected = _activePlayers.Find(p => p.ParentDevice == device);
                    
                    _pairedDevices.Remove(device);
                    _activePlayers.Remove(disconnected);

                    Debug.LogWarning($"{device.displayName} が切断されたわよ");

                    disconnected.Leave(device);
                    break;
                default:
                    break;
            }
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }
    }
}