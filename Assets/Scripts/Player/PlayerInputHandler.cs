using SchwesteriumLibrary.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤー入力の受け取りと通知を行う
namespace HammerSmash
{
    [Serializable]
    public class PlayerInputHandler : MultiPlayInputHandlerBase
    {
        public event Action<Vector2> OnMove;
        public event Action OnJump;
        public event Action<InputAction.CallbackContext> OnAttack;
        public event Action<int> OnPause;

        protected override void SetUpAction()
        {
            //Actionの作成
            var moveAction = _map.AddAction("Move", InputActionType.Value);
            var jumpAction = _map.AddAction("Jump", InputActionType.Button);
            var attackAction = _map.AddAction("Attack", InputActionType.Button);

            var pauseAction = _map.AddAction("Pause", InputActionType.Button);

            //各ActionにBindingを追加する
            moveAction.AddBinding("<Gamepad>/leftStick", groups: "Gamepad");
            moveAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w", groups: "Keyboard")
                .With("Down", "<Keyboard>/s", groups: "Keyboard")
                .With("Left", "<Keyboard>/a", groups: "Keyboard")
                .With("Right", "<Keyboard>/d", groups: "Keyboard");

            jumpAction.AddBinding("<Gamepad>/buttonSouth", groups: "Gamepad");
            jumpAction.AddBinding("<Keyboard>/f", groups: "Keyboard");

            attackAction.AddBinding("<Gamepad>/rightTrigger", groups: "Gamepad");
            attackAction.AddBinding("<Gamepad>/leftTrigger", groups: "Gamepad");
            attackAction.AddBinding("<Keyboard>/space", groups: "Keyboard");

            pauseAction.AddBinding("<Gamepad>/start", groups: "Gamepad");
            pauseAction.AddBinding("<Keyboard>/escape", groups: "Keyboard");

            //イベントの登録
            moveAction.performed += context => OnMove?.Invoke(context.ReadValue<Vector2>());
            moveAction.canceled += context => OnMove?.Invoke(new Vector2(0, 0));

            jumpAction.started += context => OnJump?.Invoke();

            attackAction.started += context => OnAttack?.Invoke(context);
            attackAction.canceled += context => OnAttack?.Invoke(context);

            pauseAction.started += context => OnPause?.Invoke(_playerId);
        }

        protected override void ReleaseAction()
        {
            OnMove = null;
            OnJump = null;
            OnAttack = null;
            OnPause = null;
        }
    }
}