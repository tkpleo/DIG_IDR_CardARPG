using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;

    [SerializeField] private float gravity = -9.81f;

    [Header("Dodge")]
    [SerializeField] private float dodgeCooldown = 1.5f;
    [SerializeField] private float dodgeTime = 0.5f;
    [SerializeField] private float dodgeSpeed = 7f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackTime = 0.2f;

    [SerializeField] private float animationFinishTime = 0.9f;

    private bool _canDodge;
    private bool _isDodging;
    private bool _dodgeInput;

    private bool _canAttack;
    private bool _isAttacking;
    private bool _attackInput;

    public Camera mainCamera;
    public LayerMask groundLayer;
    public Vector2 mousePosition;

    private Animator animator;
    private Vector3 _velocity;
    private InputSystem_Actions _playerInputActions;
    private Vector3 _input;
    private CharacterController _characterController;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();
        playerAttack = GetComponent<PlayerAttack>();

        _canAttack = true;
        _canDodge = true;
        _isDodging = false;
        _isAttacking = false;
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

        Move();

        

        if (_dodgeInput && _canDodge && _isDodging == false)
        {
            StartCoroutine(Dodge());
        }
        
        if (_attackInput && _canAttack && _isAttacking == false)
        {
            _isAttacking = true;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 lookAtPoint = hitInfo.point;
                Vector3 direction = lookAtPoint - transform.position;
                direction.y = 0;
               

                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = rotation;
                }
                _canAttack = false;
                playerAttack.InitAttack();
                //Debug.DrawLine(ray.origin, hitInfo.point, Color.red);

            }
            StartCoroutine(Attack());
        }

    }
 
    private IEnumerator Attack()
    {
        animator.SetBool("isAttacking", _isAttacking);
        yield return new WaitForSeconds(attackTime);
        _isAttacking = false;
        animator.SetBool("isAttacking", _isAttacking);
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true;

    }

    private IEnumerator Dodge()
    {
        
        _canDodge = false;
        _canAttack = false;
        _isDodging = true;
        animator.SetBool("isDodging", _isDodging);
        yield return new WaitForSeconds(dodgeTime);
        _isDodging = false;
        animator.SetBool("isDodging", _isDodging);
        _canAttack = true;
        yield return new WaitForSeconds(dodgeCooldown);
        _canDodge = true;
    }

    private void Look()
    {

        if (_input == Vector3.zero || _isDodging == true)
        {
            return;
        }
        if (_isAttacking == true)
        {
            return;
        }
        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 multipliedMatrix = isometricMatrix.MultiplyPoint3x4(_input);

        Quaternion rotation = Quaternion.LookRotation(multipliedMatrix, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
    }

    private void Move()
    {
        if (_isDodging == true)
        {
            _characterController.Move(transform.forward * dodgeSpeed * Time.deltaTime);
        }
        if (_isAttacking == true)
        {
            return;
        }
        Vector3 mDirection = transform.forward * speed * _input.magnitude * Time.deltaTime + _velocity;
        _characterController.Move(mDirection);
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
        _dodgeInput = _playerInputActions.Player.Dodge.IsPressed();
        _attackInput = _playerInputActions.Player.Attack.IsPressed();
        mousePosition = _playerInputActions.Player.AttackPos.ReadValue<Vector2>();

        //Debug.Log(_input); Uncomment to see input vector in console
    }
}
