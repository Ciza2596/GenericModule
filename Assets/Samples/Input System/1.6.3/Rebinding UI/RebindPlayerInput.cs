using UnityEngine;
using UnityEngine.InputSystem;

public class RebindPlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _playerInput;

    // Start is called before the first frame update
    void OnEnable()
    {
        _playerInput.actions["Interact"].started += OnInteract;
    }
    
    void OnDisable()
    {
        _playerInput.actions["Interact"].started -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact.");
    }
}