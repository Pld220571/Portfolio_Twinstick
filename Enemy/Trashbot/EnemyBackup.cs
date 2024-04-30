using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBackup : MonoBehaviour
{
    private PlayerDetection _playerDetection;
    private Light _spotlight;

    [SerializeField] private float _Radius;

    private void Start()
    {
        _playerDetection = GetComponent<PlayerDetection>();
    }

    private void Update()
    {
        if (_playerDetection.PlayerDetected)
            BackupRadar();
    }

    private void BackupRadar()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _Radius);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject.name);
            if (hitCollider.name == "Trashbot")
            {
                Debug.Log("Call Enemies!");
                hitCollider.GetComponent<PlayerDetection>().PlayerDetected = true;
                //hitCollider.GetComponent<Light>().color = Color.red;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _Radius);
    }

    //private void BackupRadar()
    //{
    //    for (int i = 0; i < _Enemies.Length; i++)
    //    {
    //        if (_Enemies[i].PlayerDetected)
    //        {
    //            for (int o = 0; o < _Enemies.Length; o++)
    //            {
    //                _Enemies[o].PlayerDetected = true;
    //            }
    //        }
    //    }
    //}
}
