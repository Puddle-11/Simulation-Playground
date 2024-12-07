using UnityEngine;
using UnityEngine.InputSystem;

namespace Scroll_Layout_Package.Scripts
{
    public class GameplayInputHandler : MonoBehaviour
    {
        public static GameplayInputHandler Instance;
        private PlayerInput _playerInputAction;
        
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _playerInputAction = new PlayerInput();
            _playerInputAction.Enable();
            
            InitializeInputActions();
        }
        
        private InputAction dpadInput;
        
        
        private void InitializeInputActions()
        {
        
            dpadInput = _playerInputAction.Player.DPad;
        }
        
        public float RawFloatMovementInput()
        {
            return dpadInput.ReadValue<float>();
        }

    }
}
