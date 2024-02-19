using System;
using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameObject _player;

    private Animator _animator;
   
    private AudioSource _enemyAudio;
    private AudioClip _punchFlySound;

    private float _speed = 0.5f;
    
    private float _hitCooldownInSeconds = 2f;
    private float _hurtDisabledTimeInSeconds = 1f; 
    
    private float _punchFlyVolume = 0.5f;

    private float _sameLevelDistanceX = 0.3f;
    private float _sameLevelDistanceY = 0.05f;
    
    private bool _canHit = true;
    private bool _shouldPause;
    private bool _canMove = true;

    private int _hitCooldownCoroutineCalls;

    void Start()
    {
        _player = GameObject.Find("Player");

        _animator = GetComponent<Animator>();
        _enemyAudio = GetComponent<AudioSource>();
    
        _punchFlySound = GetComponent<CharacterSounds>().PunchFlySound;
    }

    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // TODO: учесть разницу по оси Y
        // TODO: учесть близость героя с остальными врагами - не подходить, если уже есть кто-то рядом

        var positionLeftToPlayer = new Vector2(_player.transform.position.x - 0.25f, _player.transform.position.y);
        var positionRightToPlayer = new Vector2(_player.transform.position.x + 0.25f, _player.transform.position.y);

        var distanceToLeft = Vector2.Distance(transform.position, positionLeftToPlayer);
        var distanceToRight = Vector2.Distance(transform.position, positionRightToPlayer);

        var destination = distanceToLeft > distanceToRight ? positionRightToPlayer : positionLeftToPlayer;

        bool shouldMove = (Math.Abs(transform.position.x - _player.transform.position.x) > _sameLevelDistanceX) || 
            (Math.Abs(transform.position.y - _player.transform.position.y) > _sameLevelDistanceY);
        
        _animator.SetFloat("Speed_f", _speed * Convert.ToInt16(shouldMove && _canMove));
        
        if (shouldMove && _canMove) {
            if (_shouldPause)
                PauseMovement();

            transform.position = Vector2.MoveTowards(transform.position, destination, _speed * Time.deltaTime);
            transform.localScale = new Vector3(
                transform.position.x < _player.transform.position.x ? -1 : 1,
                transform.localScale.y,
                transform.localScale.z
                );
        } else
        {
            _shouldPause = true;

            if (!shouldMove && _canHit) 
                Hit();
        }
    }

    private void PauseMovement() {
        StartCoroutine(PauseCoroutine(2f));
    }

    private IEnumerator PauseCoroutine(float time) {
        _canMove = false;
        yield return new WaitForSeconds(time);
        _canMove = true;
        _shouldPause = false;
    }

    private void Hit() {
        _animator.SetTrigger("Punch_trig");
        _enemyAudio.PlayOneShot(_punchFlySound, _punchFlyVolume);
        StartCoroutine(HitCooldownCoroutine(_hitCooldownInSeconds));
    }

    public void GetHurt() {
        StartCoroutine(HitCooldownCoroutine(_hurtDisabledTimeInSeconds));
    }

    public IEnumerator HitCooldownCoroutine(float timeInSeconds = 0) {
        _hitCooldownCoroutineCalls++;
        _canHit = false;
        yield return new WaitForSeconds(timeInSeconds);
        _hitCooldownCoroutineCalls--;
        if (_hitCooldownCoroutineCalls == 0)
            _canHit = true;
    }
}
