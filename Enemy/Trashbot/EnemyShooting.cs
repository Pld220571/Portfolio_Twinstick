using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] private float _FireRate;
    [SerializeField] private float _FireBurstRate;
    [SerializeField] private int _LaserAmount;
    [SerializeField] private GameObject _Laser;
    [SerializeField] private Transform _LaserExitPoint;
    //[SerializeField] private AudioSource shootSoundEffect;

    private float _fireTime;
    private GameObject _player;

    [Header("Chase Player")]
    [SerializeField] private float _MaxDistance;
    [SerializeField] private float _MinDistance = 0.5f;

    private NavMeshAgent _agent;

    [Header("Player Detection")]
    private PlayerDetection _playerDetection;

    [Header("Animation")]
    [SerializeField] private float _AnimationDuration;

    private Animator _animator;
    private float _animationTime;

    private void Start()
    {
        _playerDetection = GetComponent<PlayerDetection>();

        _player = GameObject.FindGameObjectWithTag("Player");

        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        float dist = Vector3.Distance(_player.transform.position, transform.position);
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        Vector3 desiredDist = _player.transform.position - (direction * _MinDistance);

        if (_playerDetection.PlayerDetected)
        {
            if (dist <= _MaxDistance)
            {
                transform.LookAt(_player.transform.position);

                //Trashbot Shoot To Walk Animation Transition
                _animator.SetBool("Shoot", true);
                _animator.SetBool("Walk", false);

                _fireTime += Time.deltaTime;
                if (_fireTime >= _FireRate)
                {
                    StartCoroutine(FireBurst());
                    _fireTime = 0;
                }
            }
            else
            {
                //Trashbot Walkt To Shoot Animation Transition
                _animator.SetBool("Shoot", false);
                _animator.SetBool("Walk", true);

                _agent.SetDestination(desiredDist);
            }
        }

        //Idle Animation
        else
        {
            _animationTime += Time.deltaTime;
            if (_animationTime > _AnimationDuration)
            {
                float RandomBlend = Random.Range(0, 11);
                _animator.SetFloat("Blend", RandomBlend);
                Debug.Log("number " + RandomBlend);

                _animationTime = 0;
            }
        }
    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < _LaserAmount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(_FireBurstRate);
        }

        void Shoot()
        {
            //FindObjectOfType<Audio>().PlayLaserShot();
            Instantiate(_Laser, _LaserExitPoint.position, Quaternion.Euler(0, 0, 0));
            //shootSoundEffect.Play();
        }
    }
}
