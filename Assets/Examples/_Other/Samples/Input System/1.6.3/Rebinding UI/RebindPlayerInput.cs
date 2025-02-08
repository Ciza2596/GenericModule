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
        _playerInput.actions["Attack"].started += OnAttack;
    }

    void OnDisable()
    {
        _playerInput.actions["Interact"].started -= OnInteract;
        _playerInput.actions["Attack"].started -= OnAttack;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact.");
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack.");
    }
}