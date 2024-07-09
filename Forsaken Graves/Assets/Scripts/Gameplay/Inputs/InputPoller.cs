using ForsakenGraves.Identifiers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace ForsakenGraves.Gameplay.Inputs
{
    public class InputPoller : IInitializable
    {
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _lookAction;
        private InputAction _attackAction;

        private InputFlags _lastInput;

        public void Initialize()
        {
            _moveAction = InputSystem.actions.FindAction(InputActions.MOVE);
            _jumpAction = InputSystem.actions.FindAction(InputActions.JUMP);
            _lookAction = InputSystem.actions.FindAction(InputActions.LOOK);
            _attackAction = InputSystem.actions.FindAction(InputActions.ATTACK);
        }
        
        public InputFlags GetMovementInput()
        {
            InputFlags movementInput = 0;

            Vector2 movementVector = _moveAction.ReadValue<Vector2>();
            bool jumpPressed = _jumpAction.IsPressed();
            
            if (movementVector.y > 0) 
                movementInput |= InputFlags.Forward;

            if (movementVector.y < 0) 
                movementInput |= InputFlags.Back;

            if (movementVector.x < 0) 
                movementInput |= InputFlags.Left;

            if (movementVector.x > 0) 
                movementInput |= InputFlags.Right;

            if (jumpPressed)
                movementInput |= InputFlags.Jump;
            
            //in order to simulate getkeydown in nonregular tick rate, 
            //check last input and remove if same input is present
            InputFlags lastInput = _lastInput;
            _lastInput = movementInput;
            
            if ((lastInput & InputFlags.Jump) != 0)
            {
                movementInput &= ~InputFlags.Jump;
            }
            
            return movementInput;
        }

        public bool GetShootingInput()
        {
            return _attackAction.IsPressed();
        }
        
        public float GetRotationXInput()
        {
            return _lookAction.ReadValue<Vector2>().x;
        }
        
        public float GetRotationYInput()
        {
            return _lookAction.ReadValue<Vector2>().y;
        }
        
        private InputFlags MovementInputFromOldInputSystem(InputFlags movementInput)
        {
            if (UnityEngine.Input.GetKey(KeyCode.W)) 
                movementInput |= InputFlags.Forward;

            if (UnityEngine.Input.GetKey(KeyCode.S)) 
                movementInput |= InputFlags.Back;

            if (UnityEngine.Input.GetKey(KeyCode.A)) 
                movementInput |= InputFlags.Left;

            if (UnityEngine.Input.GetKey(KeyCode.D)) 
                movementInput |= InputFlags.Right;

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                movementInput |= InputFlags.Jump;
            
            return movementInput;
        }
    }
}