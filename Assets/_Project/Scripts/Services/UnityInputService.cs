using System;
using TileGame.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TileGame.Services
{
    public class UnityInputService : IInputService
    {
        public Vector2 MoveInput => ReadMoveInput();
        public event Action OnPausePressed;

        private readonly InputAction _pauseAction;

        public UnityInputService()
        {
            _pauseAction = new InputAction("Pause", InputActionType.Button);
            _pauseAction.AddBinding("<Keyboard>/escape");
            _pauseAction.AddBinding("<Keyboard>/p");
            _pauseAction.performed += HandlePausePerformed;
            _pauseAction.Enable();
        }

        private Vector2 ReadMoveInput()
        {
            Vector2 input = Vector2.zero;

            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
            }

            if (input == Vector2.zero && Gamepad.current != null)
            {
                input = Gamepad.current.leftStick.ReadValue();
            }

            return input.sqrMagnitude > 1f ? input.normalized : input;
        }

        private void HandlePausePerformed(InputAction.CallbackContext context)
        {
            OnPausePressed?.Invoke();
        }

        public void Dispose()
        {
            _pauseAction.performed -= HandlePausePerformed;
            _pauseAction.Disable();
            _pauseAction.Dispose();
        }
    }
}