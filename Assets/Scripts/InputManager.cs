using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    #region Events 
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;

    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion

    public static InputManager Instance { get; private set; }

    private TouchControls _touchControls;
    [SerializeField]
    private Camera _mainCamera;

    private void Awake()
    {
        _touchControls = new TouchControls();
        _mainCamera = FindObjectOfType<Camera>();

        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void OnEnable() => _touchControls.Enable();
    private void OnDisable() => _touchControls.Disable();

    private void Start()
    {
        _touchControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _touchControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch == null) return;

        OnStartTouch(_touchControls.Touch.PrimaryPosition.ReadValue<Vector2>(),
            (float)context.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch == null) return;

        OnEndTouch(_touchControls.Touch.PrimaryPosition.ReadValue<Vector2>(), 
            (float)context.time);
    }
}
