using UnityEngine;
using UnityEngine.InputSystem;

//Author: LuisJa
//Ref: https://docs.unity3d.com/6000.2/Documentation/ScriptReference/CharacterController.Move.html

public class PlayerController404 : MonoBehaviour
{

    private float playerSpeed = 5.0f;
    private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Input Actions")]
    public InputActionReference moveAction; // expects Vector2
    public InputActionReference jumpAction; // expects Button

    public Transform camTransform;
    public InputSystem_Actions map;


    Vector2 input_ejemplo;

    private void Awake()
    {
        controller = gameObject.AddComponent<CharacterController>();

        map.Player.Move.performed += ctx => { input_ejemplo = ctx.ReadValue<Vector2>(); };

        map.Player.Move.performed += OnMove;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input_ejemplo = context.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        ////---------------------------

        Vector3 camforward = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized;

        Vector3 camright = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;

        // Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 move = (camforward * input.y + camright * input.x);

        if (move.sqrMagnitude > 1.0f)
        {
            move = move.normalized;
        }


        //---------------------------

        /* Vector3 move = new Vector3(input.x, 0, input.y);
         move = Vector3.ClampMagnitude(move, 1f);*/

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        // Jump
        if (jumpAction.action.triggered && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);
    }
}
