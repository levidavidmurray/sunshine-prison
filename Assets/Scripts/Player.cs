using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlScheme {
    Gamepad,
    KeyboardMouse
}

[RequireComponent(typeof(Movement))]
public class Player : MonoBehaviour {
    const string CONTROL_SCHEME_GAMEPAD = "Gamepad";
    const string CONTROL_SCHEME_KEYBOARD_MOUSE = "Keyboard&Mouse";

    // Inspector values

    // Readable Values
    public ControlScheme CurrentControlScheme { get; private set; }

    // Dynamic World Set
    private InteractableActor _closestInteractable;
    
    // Internal Members
    private Movement _movement;
    private Animator _animator;
    
    private Vector3 _moveDirection;
    private Vector3 _lastMoveDirection;

    /* Unity Methods */

    private void Start() {
        _movement = GetComponent<Movement>();
        _animator = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter(Collider collision)
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

    private void OnTriggerExit(Collider collision)
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

    public void OnMove(InputAction.CallbackContext context) {
        Vector2 readVector = context.ReadValue<Vector2>();
        Vector3 toConvert = new Vector3(readVector.x, 0, readVector.y);
        _moveDirection = IsoVectorConvert(toConvert);
        _movement.Move(_moveDirection);
    }

    Vector3 IsoVectorConvert(Vector3 vector) {
        Quaternion rotation = Quaternion.Euler(0, 45f, 0);
        Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
        return isoMatrix.MultiplyPoint3x4(vector);
    }
    
}
