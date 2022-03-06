using UnityEngine;
using UnityEngine.InputSystem;

// Ensure the component is present on the gameobject the script is attached to
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    const string CONTROL_SCHEME_GAMEPAD = "Gamepad";
    const string CONTROL_SCHEME_KEYBOARD_MOUSE = "Keyboard&Mouse";

    // Inspector values
    public float movementSpeed = 115f;
    // public DebugUI debugUI;

    // Readable Values
    public ControlScheme CurrentControlScheme { get; private set; }

    // Dynamic World Set
    private InteractableActor _closestInteractable;

    // Internal Members
    private Vector2 _targetVelocity;
    private Vector2 _inputDirection;
    private Animator _anim;
    private Rigidbody2D _rigidbody2D;

    /* Unity Methods */

    void Awake()
    {
        // Setup Rigidbody for frictionless top down movement and dynamic collision
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _rigidbody2D.isKinematic = false;
        _rigidbody2D.angularDrag = 0.0f;
        _rigidbody2D.gravityScale = 0.0f;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        Animate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            if (_closestInteractable)
            {
                _closestInteractable.HidePromptInstructions();
            }

            _closestInteractable = collision.GetComponent<InteractableActor>();
            _closestInteractable.ShowPromptInstructions(CurrentControlScheme);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interaction"))
        {
            // Avoid edge case where closest interaction changed before exitting old interaction
            if (collision.gameObject == _closestInteractable.gameObject)
            {
                _closestInteractable.HidePromptInstructions();
                _closestInteractable = null;
            }
        }
    }

    /* Input Events */

    public void OnInteractDialogue(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (_closestInteractable && _closestInteractable.IsActorType(typeof(DialogueActor)))
            {
                _closestInteractable.Interact();
            }
        }
    }

    public void OnControlsChanged(PlayerInput input)
    {
        switch (input.currentControlScheme)
        {
            case CONTROL_SCHEME_GAMEPAD:
                CurrentControlScheme = ControlScheme.Gamepad;
                break;
            case CONTROL_SCHEME_KEYBOARD_MOUSE:
                CurrentControlScheme = ControlScheme.KeyboardMouse;
                break;
            default:
                CurrentControlScheme = ControlScheme.Gamepad;
                break;
        }

        if (_closestInteractable)
        {
            _closestInteractable.ShowPromptInstructions(CurrentControlScheme);
        }
        print($"Control Scheme: {input.currentControlScheme}");
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 moveInput = value.ReadValue<Vector2>();

        moveInput = SnapVector2Dir(moveInput);

        // debugUI.SetInputValue(moveInput);

        _targetVelocity = moveInput.normalized;

        _targetVelocity = Vector2.Scale(_targetVelocity, new Vector2(1, 0.6f));

        // debugUI.SetAngleValueFromMovement(_targetVelocity);

        // debugUI.SetMagnitude(_targetVelocity.magnitude);

        if (_targetVelocity.magnitude >= 0.1) {
            _inputDirection = moveInput;
        }
    }

    /* User Methods */

    void Move()
    {        
        // Set rigidbody velocity
        // Multiply the target by deltaTime to make movement speed consistent across different framerates
        _rigidbody2D.velocity = (_targetVelocity * movementSpeed) * Time.deltaTime; 
    }

    void Animate()
    {
        _anim.SetFloat("AnimMoveX", _inputDirection.x);
        _anim.SetFloat("AnimMoveY", _inputDirection.y);
        _anim.SetFloat("AnimMoveMagnitude", _targetVelocity.magnitude);
    }

    /// <summary>
    /// Snaps directional Input (magnitudes from 0-1F) to 8 directions or Vector2.zero if below Dead,
    /// - keeps magnitude
    /// - axis dead uses magnitude
    /// - option to scale values from zero after dead has been exceeded rather than its actual value. Set "axisScaleFromZero" to true
    /// - option to smooth snapping. Set "axisSmoothSnapMode" to .Smooth https://forum.unity3d.com/threads/8-way-direction-joystick-only.438758/#post-2840450
    /// </summary>
    ///
    private enum    Vector2SnapMode { Discrete, Smooth }
    
    //parameters
    private float            axisDead            =  0.4F;    //radius of axis dead, from 0 to zero
    private float            smoothConst            = -7.5F;    //some factor that controls smoothing, everything less than -7.5 makes values overshoot
    private    bool            axisScaleFromZero    =  true;  
    private    Vector2SnapMode    axisSmoothSnapMode    =  Vector2SnapMode.Smooth;
    
    private Vector2 SnapVector2Dir(Vector2 raw){
    
        Vector2 vec = raw;
    
        float sign            = Mathf.Sign(raw.x * Vector2.up.y - raw.y * Vector2.up.x);
        float angle            = Vector2.Angle(Vector2.up, raw) *sign;
    
        float clampedAngle = 0F;
    
        if(axisSmoothSnapMode == Vector2SnapMode.Smooth) {
            clampedAngle    = smoothConst * Mathf.Sin(angle * Mathf.Deg2Rad * 16) + angle;
        }else{
            clampedAngle    = Mathf.Round( angle/22.5F )*22.5F;
        }

        vec = new Vector2( Mathf.Sin(clampedAngle *Mathf.Deg2Rad), Mathf.Cos(clampedAngle *Mathf.Deg2Rad) ).normalized *raw.magnitude;
    
        //Debug
        // print(angle.ToString("F2")+"\t-> "+clampedAngle.ToString("F2"));
        // Vector3 origin = cameraTrans.position+cameraTrans.forward*20F;
        // Debug.DrawLine(origin +cameraTrans.right *-6F, origin +cameraTrans.right * 6F,            Color.white, 0F, false);    //herz
        // Debug.DrawLine(origin +cameraTrans.up *-6F, origin +cameraTrans.up * 6F,                Color.white, 0F, false);    //vert
        // Debug.DrawLine(origin, origin + (cameraTrans.right * raw.x+cameraTrans.up * raw.y) *5F, Color.cyan, 0F, false);
        // Debug.DrawLine(origin, origin + (cameraTrans.right * vec.x+cameraTrans.up * vec.y) *5F, Color.magenta, 0F, false);
    
        
        //Magnitude Clamping and Dead
        float mag = vec.magnitude;
        if(mag < axisDead){
            vec = Vector3.zero;
        }else{
            if(axisScaleFromZero)    { vec = vec.normalized *Mathf.InverseLerp(axisDead, 1F, mag);    }    //start from zero after exceeding axis dead
            else                    { vec = vec.normalized *Mathf.Clamp(mag, axisDead, 1F);            }    //start from axis dead
        }
        
        return vec ;
    
    }
}

