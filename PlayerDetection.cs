using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] private Light _Spotlight;
    [SerializeField] private float _ViewDistance;

    private float _viewAngle;
    private Transform _player;

    [SerializeField] private LayerMask _ViewMask;

    [HideInInspector] public bool PlayerDetected;

    private Color _origianlSpotlightColor;

    private void Start()
    {
        _viewAngle = _Spotlight.spotAngle;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        _origianlSpotlightColor = _Spotlight.color;
    }

    private void Update()
    {
        CanSeePlayer();
    }

    private void CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, _player.position) < _ViewDistance)
        {
            Vector3 distToPlayer = (_player.position - transform.position).normalized;

            float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, distToPlayer);
            if (angleBetweenEnemyAndPlayer < _viewAngle / 2f)
                if (!Physics.Linecast(transform.position, _player.position, _ViewMask))
                    PlayerDetected = true;
            if (PlayerDetected)
                _Spotlight.color = Color.red;
            else
                _Spotlight.color = _origianlSpotlightColor;
        }
        return;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * _ViewDistance);
    }

    //    public float DetectionRadius = 10.0f;
    //    public float DetectionAngle = 90.0f;

    //    public bool PlayerDetected;

    //    [SerializeField] private LayerMask _Enviroment;

    //    void Update()
    //    {
    //        LookForPlayer();
    //    }

    //    private PlayerMovement LookForPlayer()
    //    {
    //        if (PlayerMovement.instance == null)
    //        {
    //            return null;
    //        }

    //        Vector3 enemyPos = transform.position;
    //        Vector3 toPlayer = PlayerMovement.instance.transform.position - enemyPos;
    //        toPlayer.y = 0;



    //        RaycastHit hit;

    //        if (!Physics.Raycast(transform.position, transform.forward, out hit, toPlayer.magnitude, _Enviroment))      
    //        {
    //            if (toPlayer.magnitude <= DetectionRadius)
    //            {
    //                if (Vector3.Dot(toPlayer.normalized, transform.forward) >
    //                    Mathf.Cos(DetectionAngle * 0.5f * Mathf.Deg2Rad))
    //                {
    //                    Debug.Log("Player has been detected!");
    //                    PlayerDetected = true;
    //                    return PlayerMovement.instance;
    //                }
    //            }
    //        }

    //        return null;
    //    }

    //#if UNITY_EDITOR
    //    private void OnDrawGizmosSelected()
    //    {
    //        Color c = new Color(0.8f, 0, 0, 0.4f);
    //        UnityEditor.Handles.color = c;

    //        Vector3 rotadedForward = Quaternion.Euler(0, -DetectionAngle * 0.5f, 0) * transform.forward;

    //        UnityEditor.Handles.DrawSolidArc(
    //            transform.position,
    //            Vector3.up,
    //            rotadedForward,
    //            DetectionAngle,
    //            DetectionRadius);
    //    }
    //#endif
}


