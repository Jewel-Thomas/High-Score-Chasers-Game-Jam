using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerActions playerActions;
    private Vector2 inputVector;
    private float smoothTime = 10;
    private bool isClutchApplied;

    public event EventHandler OnMisc;

    private enum InputSystemType
    {
        OLD_INPUT_SYSTEM,
        NEW_INPUT_SYSTEM
    }

    [SerializeField] private InputSystemType inputSystemType;

    private void Awake()
    {
        Instance = this;
        playerActions = new PlayerActions();

        playerActions.Car.Enable();
        playerActions.Car.Space.performed += Space_performed;
    }

    private void Space_performed(InputAction.CallbackContext obj)
    {
        OnMisc?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        UpdateClutchApplied();
    }

    public void UpdateClutchApplied()
    {
        isClutchApplied = playerActions.Car.Clutch.IsPressed();
    }

    public bool IsClutchApplied()
    {
        return isClutchApplied;
    }

    public Vector2 CarMovementInputNormalized()
    {
        switch (inputSystemType)
        {
            default:
            case InputSystemType.OLD_INPUT_SYSTEM:
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");
                inputVector = new Vector2(horizontalInput, verticalInput);
                return inputVector;
            case InputSystemType.NEW_INPUT_SYSTEM:
                Vector2 inputVectorRaw = playerActions.Car.Move.ReadValue<Vector2>();
                inputVector = Vector2.Lerp(inputVector, inputVectorRaw, Time.deltaTime * smoothTime);
                if (inputVector.sqrMagnitude < 0.0001f)
                    inputVector = Vector2.zero;
                return inputVector;
        }
    }
}
