using System;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameObject _player;

    private Animator _animator;

    private float _speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");

        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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

        bool shouldMove = (Math.Abs(transform.position.x - _player.transform.position.x) > 0.25f) || (Math.Abs(transform.position.y - _player.transform.position.y) > 0.05f);
        
        _animator.SetFloat("Speed_f", _speed * Convert.ToInt16(shouldMove));
        
        if (shouldMove) {
            transform.position = Vector2.MoveTowards(transform.position, destination, _speed * Time.deltaTime);
            transform.localScale = new Vector3(
                transform.position.x < _player.transform.position.x ? -1 : 1,
                transform.localScale.y,
                transform.localScale.z
                );
        } else
        {
            Hit();
        }
    }

    private void Hit() {
        // TODO: проиграть анимацию удара; если попал - ударить еще раз; если не попал - FollowPlayer()
        _animator.SetTrigger("Punch_trig");
    }
}
