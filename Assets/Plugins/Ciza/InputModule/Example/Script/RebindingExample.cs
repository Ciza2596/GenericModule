using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindingExample : MonoBehaviour
{
    [SerializeField]
    private PlayerInput _playerInput;
    
    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private Button _button;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerInput.actions["Confirm"].PerformInteractiveRebinding();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
