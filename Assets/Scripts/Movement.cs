using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {
    
    private static readonly int ForwardSpeed = Animator.StringToHash("ForwardSpeed");

    public float speed = 3f;
    public float minSpeed = 1f;
    
    public float groundDistance = 1f;
    public int groundLayer;
    public Transform groundChecker;
    
    [SerializeField] private AnimationCurve inputSpeedCurve;
    
    private CharacterController _cc;
    private Animator _animator;

    private Vector3 _direction;
    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    
    private bool _isGrounded;
    private Vector3 _lastMoveDirection;

    private void Start() {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        float _speed = inputSpeedCurve.Evaluate(_direction.magnitude);
        
        Vector3 _velocity = _direction.normalized * _speed;

        // LocalVelocity = transform.InverseTransformVector(_velocity);
        
        print($"[BEFORE] Direction: {_direction} Magnitude: {_direction.magnitude} Normalized: {_direction.normalized}, _speed: {_speed}");
        
        // if (LocalVelocity.z < minSpeed) {
        //     _velocity = _velocity.normalized * minSpeed;
        // }
        _velocity.y += Physics.gravity.y;
        
        _isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundLayer,
            QueryTriggerInteraction.Ignore);
        
        if (_isGrounded && Velocity.y < 0) {
            _velocity.y = 0f;
        }
        
        Velocity = _velocity;
        LocalVelocity = transform.InverseTransformVector(Velocity);
        _animator.SetFloat(ForwardSpeed, LocalVelocity.z);
        
        print($"[AFTER] LocalVelocity: {LocalVelocity}, LocalVelocity.magnitude: {LocalVelocity.magnitude}");
        
        _cc.Move(Velocity * Time.deltaTime);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lastMoveDirection),
            Time.deltaTime * 40f);
    }

    private void Update() {
    }

    public void Move(Vector3 direction) {
        _direction = direction;

        if (direction.magnitude > 0.01f) {
            _lastMoveDirection = direction;
        }
    }
    
}
