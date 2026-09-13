/*
Author : schwesterium
Date   : 2026/08/01
*/

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace SchwesteriumLibrary.Input
{
    public abstract class MultiPlayInputHandlerBase : IDisposable
    {
        protected int _playerId = -1;

        protected InputUser _inputUser;
        protected InputDevice _device = null;
        protected InputActionMap _map = null;

        protected bool _isJoined = false;

        public bool IsActive { get; private set; } = false;
        public InputDevice ParentDevice { get { return _device; } }
        public int PlayerId { get { return _playerId; } }

        /// <summary>
        /// デバイスにハンドラーを割り当てる
        /// </summary>
        /// <param name="device"></param>
        public void Join(InputDevice device, int playerId)
        {
            if (_isJoined)
            {
                Debug.LogWarning($"{{id {playerId}, device {device.name}}} はすでにJoinしています！");
                return;
            }
            _isJoined = true;

            _playerId = playerId;
            _device = device;

            //InputUserを作成してデバイスを割り当て
            _inputUser = InputUser.CreateUserWithoutPairedDevices();
            InputUser.PerformPairingWithDevice(_device, user: _inputUser);

            //ActionMapの作成
            _map = new InputActionMap($"Player{_playerId}");
            SetUpAction();
            
            //ユーザーに割り当て
            _inputUser.AssociateActionsWithUser(_map);

            Enable();
        }

        public void Leave(InputDevice device)
        {
            if (!_isJoined)
            {
                Debug.LogWarning($"{{device {device.name}}} はすでにLeaveしています！");
                return;
            }

            if(device != _device)
            {
                Debug.LogWarning("無効なdevice");
                return;
            }

            _isJoined = false;

            _playerId = -1;

            ReleaseInternal();
        }

        protected abstract void SetUpAction();

        public void Enable()
        {
            IsActive = true;
            _map?.Enable();
        }
        public void Disable()
        {
            IsActive = false;
            _map?.Disable();
        }

        protected virtual void ReleaseInternal()
        {
            if (_map != null)
            {
                _map.Disable();
                _map.Dispose();
                _map = null;
            }

            if (_inputUser.valid) { _inputUser.UnpairDevicesAndRemoveUser(); }
            _device = null;

            IsActive = false;
        }

        public void Dispose()
        {
            ReleaseInternal();

            ReleaseAction();

            _isJoined = false;
        }

        protected abstract void ReleaseAction();
    }
}