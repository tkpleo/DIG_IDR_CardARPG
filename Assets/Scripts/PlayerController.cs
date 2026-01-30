using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 360f;

    [SerializeField] private float accelerationVal = 5f;
    [SerializeField] private float decelerationVal = 10f;

    [SerializeField] private float gravity = -9.81f;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 1.5f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashSpeed = 8f;

    [SerializeField] private float animationFinishTime = 0.9f;

    private bool _canDash;
    private bool _isDashing;
    private bool _canAttack;
    private bool _isAttacking;

    private bool _dashInput;
    private bool _attackInput;

    private Animator animator;
    private Vector3 _velocity;
    private float _currentSpeed;
    private InputSystem_Actions _playerInputActions;
    private Vector3 _input;
    private CharacterController _characterController;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();

        _canAttack = true;
        _canDash = true;
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
    }

    private void Update()
    {
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2;
        }

        if (!isGrounded)
        {
            _velocity.y = gravity * Time.deltaTime;
        }

        GetInput();

        Look();
        SpeedCalc();

        Move();
        if (_dashInput && _canDash)
        {
            StartCoroutine(Dash());
        }
        Running();
        if (_attackInput && _canAttack)
        {
            Attack();
        }

        if (_isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinishTime)
        {
            _isAttacking = false;
            animator.SetBool("isAttacking", false);
        }

    }

    private void Attack()
    {
        if (!_isAttacking)
        {
            StartCoroutine(initAttack());
        }
    }

    private IEnumerator initAttack()
    {
        yield return new WaitForSeconds(0.1f);
        _isAttacking = true;
        animator.SetBool("isAttacking", true);

    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        yield return new WaitForSeconds(dashTime);
        _isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    private void SpeedCalc()
    {
        if (_input == Vector3.zero && _currentSpeed > 0)
        {
            _currentSpeed -= decelerationVal * Time.deltaTime;
        }
        else if (_input != Vector3.zero && _currentSpeed < maxSpeed)
        {
            _currentSpeed += accelerationVal * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, maxSpeed);
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 multipliedMatrix = isometricMatrix.MultiplyPoint3x4(_input);

        Quaternion rotation = Quaternion.LookRotation(multipliedMatrix, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
    }

    private void Move()
    {
        if (_isDashing)
        {
            _characterController.Move(transform.forward * dashSpeed * Time.deltaTime);
            return;
        }

        Vector3 mDirection = transform.forward * _currentSpeed * _input.magnitude * Time.deltaTime + _velocity;

        _characterController.Move(mDirection);
    }

    private void Running()
    {
        if (_input == Vector3.zero)
        {
            animator.SetBool("isRunning", false);
            return;
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

    }

    private void GetInput()
    {
        Vector2 input = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(input.x, 0, input.y);
        _dashInput = _playerInputActions.Player.Sprint.IsPressed();
        _attackInput = _playerInputActions.Player.Attack.IsPressed();

        Debug.Log(_input);
    }
}
