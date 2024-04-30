using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    [SerializeField] private Transform _MovePositionTransform;

    private GameObject _Player;

    private PlayerDetection _playerDetection;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _Player = GameObject.FindGameObjectWithTag("Player");

        _playerDetection = GetComponent<PlayerDetection>();
    }

    private void Update()
    {
        if (!_playerDetection.PlayerDetected)
        {
            _navMeshAgent.destination = _MovePositionTransform.position;
        }       

        if (_playerDetection.PlayerDetected)
        {
            StopAllCoroutines();
            _navMeshAgent.destination = _Player.transform.position;
            //transform.LookAt(_Player.transform);
        }
    }
}
