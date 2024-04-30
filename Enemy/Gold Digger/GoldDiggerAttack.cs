using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class GoldDiggerAttack : MonoBehaviour
{
    [SerializeField] private GameObject _Rock;
    [SerializeField] private Transform[] _RockLauncher;

    [SerializeField] float _LaunchRate;
    [SerializeField] float _LaunchForce;
    [SerializeField] float _UpwardLaunchForce;

    private PlayerDetection _playerDetection;

    private GameObject _player;

    [Header ("Rock Spread")]
    [SerializeField] private Vector2 _SpreadRotationMultiplier; //Verandert de Axis Rotation
    [SerializeField] private float _SpreadAmount;

    [Header("Loot At Player")]
    [SerializeField] private float _LookToPlayerTimer;

    [Header("LaunchCoroutine")]
    [SerializeField] private float _LaunchingTime = 0.01f;
    [SerializeField] private float _PrepareToLaunch = 3;

    private bool _launch;

    [Header("Animation")]
    [SerializeField] private EnemyState _EnemyState;

    private Animator _animator;
    private float _animationTimer;

    [SerializeField] private float _WaitForAnimation;

    private GoldDiggerAudio _goldDiggerAudio;

    private void Start()
    {
        _playerDetection = GetComponent<PlayerDetection>();

        _player = GameObject.FindGameObjectWithTag("Player");

        _animator = GetComponent<Animator>();

        _goldDiggerAudio = GetComponent<GoldDiggerAudio>();
    }

    private void Update()
    {
        if (_playerDetection.PlayerDetected)
        {
            if (!_launch)
            {
                StartCoroutine("LaunchCoroutine");
            }

            StartCoroutine(LookAtOverTime(transform, _player.transform, _LookToPlayerTimer));
        }
    }

    IEnumerator LookAtOverTime(Transform t, Transform target, float dur)
    {
        Quaternion start = t.rotation;
        Quaternion end = Quaternion.LookRotation(target.position - t.position);
        float time = 0f;
        while (time < dur)
        {
            t.rotation = Quaternion.Slerp(start, end, time / dur);
            yield return null;
            time += Time.deltaTime;
        }
    }

    IEnumerator LaunchCoroutine()
    {
        _launch = true;
        int rocksAmount = 1000;
        int rockCount = 0;       
        UpdateAnimatorState(EnemyState.Attack);
        
        while (rockCount < rocksAmount)
        {
            _animationTimer += Time.deltaTime;
            if (_animationTimer >= _WaitForAnimation)
            {
                LaunchRock();
                rockCount++;
            }
            yield return new WaitForSeconds(_LaunchingTime);
        }      
        UpdateAnimatorState(EnemyState.Idle);
        _goldDiggerAudio.StopSound();
        _animationTimer = 0;
        yield return new WaitForSeconds(_PrepareToLaunch);
        _launch = false;
    }

    private void LaunchRock()
    {
        int randomLauncher = UnityEngine.Random.Range(0, _RockLauncher.Length);
        Tuple<Vector3, Quaternion> launcher = MakeRockSpread(
            _SpreadAmount, _RockLauncher[randomLauncher].position, _RockLauncher[randomLauncher].rotation);

        Vector3 positions = launcher.Item1;
        Quaternion rotations = launcher.Item2;
        GameObject rock = Instantiate(_Rock, positions, rotations);
        Rigidbody rockRb = rock.GetComponent<Rigidbody>();

        Vector3 forceToAdd = _RockLauncher[randomLauncher].transform.forward * _LaunchForce
            + transform.up * _UpwardLaunchForce;

        rockRb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    private Tuple<Vector3, Quaternion> MakeRockSpread(float pSpreadAmount, Vector3 pOriginPoint, Quaternion pOriginRotation)
    {
        //positions
        Vector3 randomPos = pOriginPoint + new Vector3(UnityEngine.Random.Range(-pSpreadAmount, pSpreadAmount),
                UnityEngine.Random.Range(-pSpreadAmount, pSpreadAmount), 0);

        //rotations
        Quaternion randomRotation = pOriginRotation * Quaternion.Euler(
            UnityEngine.Random.Range(-pSpreadAmount * _SpreadRotationMultiplier.x, pSpreadAmount * _SpreadRotationMultiplier.x),
                UnityEngine.Random.Range(-pSpreadAmount * _SpreadRotationMultiplier.y, pSpreadAmount * _SpreadRotationMultiplier.y), 0);
        
        return Tuple.Create(randomPos, randomRotation);
    }

    private void UpdateAnimatorState(EnemyState p_enemyState)
    {
        // Updates the internal enemy state variable with the provided EnemyState parameter.
        _EnemyState = p_enemyState;

        // Sets the corresponding animator parameter with the integer representation of the enemy state.
        // This allows the animator controller to transition to the appropriate animation state.
        _animator.SetInteger("EnemyState", (int)_EnemyState);
    }
}
