using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions _actions;
    private InputAction _move;
    private InputAction _hit;

    [SerializeField] float _horizontalInput;
    private float _verticalInput;

    private readonly float _speed = 1f;
    private readonly float _cameraMovingSpeedModifier = 5f;
    private readonly float _cameraStoppedSpeedModifier = 0.8f;
    private float _scaleX = 1f;
    private float _timeSinceHit;
    private readonly float _hitAnimationTime = 0.375f;

    private bool _canAct = true;
    private bool _canHit = true;
    private bool _cameraShouldMove;

    private readonly float _walkableAreaDistanceX = 3.2f;
    private float _minAllowedY = -0.8f;
    private float _maxAllowedY = 0.25f;

    private GameObject _camera;

    private AudioSource _playerAudio;
    [SerializeField] AudioClip HitSound;

    private Animator _animator;

    private void Awake()
    {
        _actions = new PlayerInputActions();
    }

    void Start()
    {
        // „тобы можно было атаковать с самого начала игры, не дожида€сь несуществующего кулдауна
        _timeSinceHit += _hitAnimationTime;

        _animator = GetComponent<Animator>();
        _playerAudio = GetComponent<AudioSource>();

        _camera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        _timeSinceHit += Time.deltaTime;

        HandleMovement();
        HandleHit();
    }

    private void AdjustLookupDirection()
    {
        if (_horizontalInput > 0)
            _scaleX = 1;
        else if (_horizontalInput < 0)
            _scaleX = -1;
        else _scaleX = transform.localScale.x;
        transform.localScale = new Vector3(_scaleX, transform.localScale.y, transform.localScale.z);
    }

    private int CanMoveVertically()
    {
        if (transform.position.y >= _maxAllowedY && _verticalInput > 0)
            return 0;

        if (transform.position.y <= _minAllowedY && _verticalInput < 0)
            return 0;

        return 1;
    }

    private int CanMoveHorizontally()
    {
        if (PositionLeftBound > transform.position.x && _horizontalInput < 0) return 0;

        return 1;
    }

    private float PositionLeftBound => _camera.transform.position.x - (_walkableAreaDistanceX / 2);

    private void HandleMovement()
    {
        if (!_canAct) return;

        _horizontalInput = _move.ReadValue<Vector2>().x;
        _verticalInput = _move.ReadValue<Vector2>().y;

        _animator.SetFloat("Speed_f", _speed * Convert.ToInt16(MoveButtonsPressed));

        AdjustLookupDirection();

        _cameraShouldMove = _horizontalInput > 0 && _camera.GetComponent<MoveCamera>().CanMoveRight;

        if (_cameraShouldMove)
            _camera.GetComponent<MoveCamera>().MoveRight();

        if (MoveButtonsPressed)
            transform.Translate(
                _horizontalSpeed,
                _verticalSpeed, 
                transform.position.z
            );
    }

    private float _horizontalSpeed =>
        (_horizontalInput > 0 ? 1 : _horizontalInput) * // „тобы нивелировать разницу в скорости между горизонтальным движением и движением под углом
        _speed *
        (_cameraShouldMove ? _cameraMovingSpeedModifier : _cameraStoppedSpeedModifier) * //  огда движетс€ камера, визуально скорость меньше
        Time.deltaTime *
        CanMoveHorizontally();

    private float _verticalSpeed =>
        _verticalInput *
        _speed *
        _cameraStoppedSpeedModifier * //  амера никогда не движетс€ по вертикали
        Time.deltaTime *
        CanMoveVertically();

    private bool MoveButtonsPressed => _horizontalInput != 0 || _verticalInput != 0;

    private void HandleHit()
    {
        if (!_canAct) return;

        float hitBtnPressed = _hit.ReadValue<float>();
        if (hitBtnPressed == 0)
        {
            _canHit = true;
            return;
        }

        if (_canHit && !MoveButtonsPressed && Convert.ToBoolean(hitBtnPressed) && (_timeSinceHit > _hitAnimationTime))
        {
            _animator.SetTrigger("Punch_trig");
            _playerAudio.PlayOneShot(HitSound);
            _timeSinceHit = 0;
            _canHit = false;

            StartCoroutine(RecoverAfterAction(_hitAnimationTime));
        }
    }

    IEnumerator RecoverAfterAction(float time)
    {
        _canAct = false;

        yield return new WaitForSeconds(time);
        _canAct = true;
    }

    private void OnEnable()
    {
        _move = _actions.Player.Move;
        _move.Enable();

        _hit = _actions.Player.Hit;
        _hit.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _hit.Disable();
    }
}
