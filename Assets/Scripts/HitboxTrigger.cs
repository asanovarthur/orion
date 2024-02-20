using System;
using UnityEngine;

public class HitboxTrigger : MonoBehaviour
{
    private AudioSource _attackerAudio;

    private AudioClip _punchConnectedSound;

    private float _sameLevelDistanceY = 0.05f;

    private void Start() {
        _punchConnectedSound = gameObject.GetComponentInParent<CharacterSounds>().PunchConnectedSound;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Animator victimAnimator = collision.gameObject.GetComponent<Animator>();

        float distanceY = Math.Abs(collision.gameObject.transform.position.y - gameObject.transform.position.y);

        if (distanceY <= _sameLevelDistanceY) {
            victimAnimator.SetTrigger("GetHurt_trig");

            if (collision.gameObject.name == "Player") {
                collision.gameObject.GetComponent<PlayerHealthManager>().TakeDamage(10f);
            }

            if (collision.gameObject.tag == "Enemy") {
                collision.gameObject.GetComponent<EnemyBehaviour>().GetHurt();
            }

            _attackerAudio = gameObject.GetComponentInParent<AudioSource>();
            _attackerAudio.PlayOneShot(_punchConnectedSound);
        }
    }
}
