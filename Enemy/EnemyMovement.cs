using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyAttack
{
    //Shoot,
    Charge,
    Grind
}

public enum EnemyState
{
    Idle,
    Walk,
    Attack
}

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> _Waypoints;
    
    private float _targetDistance;
    private int _currentIndex;
    private NavMeshAgent _agent;
    private PlayerDetection _playerDetection;
    private GameObject _player;

    [SerializeField] private EnemyAttack _EnemyAttack;

    [Header("Trashbot")]
    [SerializeField] private float _Range = 5f;
    [SerializeField] private float _KeepDistance = 2.5f;

    [Header("Beyzor")]
    [SerializeField] private float _PrepareCharge = 2f;
    [SerializeField] private float _ChargeCooldown = 2f;
    [SerializeField] private float _ChargeSpeed = 3.5f;
    [SerializeField] private float _ChargeAcceleration = 8;
    [SerializeField] private float _ChargeDistance = 1.5f;

    private bool _startCharge;

    [Header("Gold Digger")]
    [SerializeField] private float _WalkingAnimationCooldown;

    private float _walkingTimer;

    [SerializeField] private EnemyState _enemyState; 

    private Animator _animator;

    private GoldDiggerAudio _goldDiggerAudio;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _playerDetection = GetComponent<PlayerDetection>();

        _player = GameObject.FindGameObjectWithTag("Player");

        _animator = GetComponent<Animator>();

        _goldDiggerAudio = GetComponent<GoldDiggerAudio>();
    }

    private void Update()
    {
        if (!_playerDetection.PlayerDetected)
        {
            if (_agent.remainingDistance <= _targetDistance)
            {
                //Gold Digger Idle Animation
                UpdateAnimatorState(EnemyState.Idle);
                _goldDiggerAudio.StopSound();

                _walkingTimer += Time.deltaTime;
                if (_walkingTimer >= _WalkingAnimationCooldown)
                {
                    SetNextTarget();
                    _walkingTimer = 0;
                }               
            }
        }
        else
        {
            switch (_EnemyAttack)
            {
                //case EnemyState.Shoot:
                //    RandomMovement();
                //    break;
                case EnemyAttack.Charge:
                    if (_startCharge == false)
                    {
                        StartCoroutine("ChargeAtPlayer");
                    }                   
                    break;
                case EnemyAttack.Grind:
                    _agent.GetComponent<NavMeshAgent>().speed = 0;
                    break;
            }
        }      
    }

    private void SetNextTarget()
    {
        //Gold Digger Walk Animation
        UpdateAnimatorState(EnemyState.Walk);

        _currentIndex %= _Waypoints.Count;

        Vector3 waypoints = new Vector3(
            _Waypoints[_currentIndex].position.x, transform.position.y, _Waypoints[_currentIndex].position.z);

        _agent.SetDestination(waypoints);

        _currentIndex++;
    }    

    //Trashbot Random Movement
    private void RandomMovement()
    {
        transform.LookAt(_player.transform);

        /// <summary>
        /// generates a random point in a sphere around "center" then checks if it actually finds a location.
        /// </summary>
        bool RandomPointInSphere(Vector3 center, float range, int layerMask, out Vector3 result)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range; //generates the location
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, _Range, layerMask)) //checks if there's a valid location point
            {
                result = hit.position;
                return true;
            }
            result = Vector3.zero; //failsafe for if it doesn't find a valid location.
            return false;
        }

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            Vector3 point;           
            bool isFound = RandomPointInSphere(transform.position, _Range, NavMesh.AllAreas, out point);
            Debug.Log("point" + point);

            //checks if the location it generated is too close to the player, and if so, it generates a new location.
            while (Vector3.Distance(point, _player.transform.position) < _KeepDistance)
            {
                isFound = RandomPointInSphere(transform.position, _Range, NavMesh.AllAreas, out point);
            }

            //gives the location to the enemy so it starts going to it
            if (isFound)
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                _agent.SetDestination(point);
            }
        }
    }

    //Beyzor Charge Attack
    private IEnumerator ChargeAtPlayer()
    {
        _startCharge = true;
        _agent.GetComponent<NavMeshAgent>().speed = 0;
        yield return new WaitForSeconds(_PrepareCharge);

        Vector3 playerPos = _player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        Vector3 direction = (playerPos - transform.position).normalized;
        Vector3 endPos = playerPos + (direction * distance * _ChargeDistance);
        //float chargeProgress = 0;

        //while (chargeProgress < 1)
        //{
        //    chargeProgress += Time.deltaTime / _ChargeTime;
        //    transform.position = Vector3.Lerp(transform.position, endPos, chargeProgress);
        //    yield return null;
        //}
        
        _agent.GetComponent<NavMeshAgent>().speed = _ChargeSpeed;
        _agent.GetComponent<NavMeshAgent>().acceleration = _ChargeAcceleration;
        _agent.SetDestination(endPos);
        yield return new WaitForSeconds(_ChargeCooldown);
        _startCharge = false;
    }

    private void OnDrawGizmos()
    {
        if (_Waypoints.Count == 0)
            return;

        Gizmos.color = Color.white;
        foreach (Transform waypoint in _Waypoints)
        {
            Gizmos.DrawSphere(waypoint.position, radius: 0.2f);
        }
    }

    ///<summary>
    /// Updates the animator state of an enemy character based on the provided EnemyState parameter.
    ///</summary>
    ///<param name="p_enemyState">The current state of the enemy character.</param>
    private void UpdateAnimatorState(EnemyState p_enemyState)
    {
        // Updates the internal enemy state variable with the provided EnemyState parameter.
        _enemyState = p_enemyState;

        // Sets the corresponding animator parameter with the integer representation of the enemy state.
        // This allows the animator controller to transition to the appropriate animation state.
        _animator.SetInteger("EnemyState", (int)_enemyState);
    }
}

