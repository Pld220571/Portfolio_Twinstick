using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    [Header("Push Player")]
    [SerializeField] private float _Speed;
    [SerializeField] private float _HitForce;
    [SerializeField] private float _PushRadius;

    [Header("Rotation Animation")]
    [SerializeField] private float _PrepareToRotate;
    [SerializeField] private GameObject _Head;
    //[SerializeField] private Transform _From;
    //[SerializeField] private Transform _To;

    private float timeCount = 0.0f;

    //private float _prepareToRotate;
    private PlayerDetection _playerDetection;
    private Animator _animator;

    private void Start()
    {
        _playerDetection = GetComponentInParent<PlayerDetection>();

        _animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        DoPush();

        //Animation
        SearchPlayer();
    }

    void DoPush()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _PushRadius);

        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.GetComponent<CharacterController>())
            {
                CharacterController pushedBody = pushedObject.GetComponent<CharacterController>();

                // Get direction from your postion toward the object you wish to push
                var direction = pushedBody.transform.position - transform.position;
                Vector3 force = new Vector3(direction.x, 0, direction.z);
                Debug.Log("Hit" + force);

                // Normalization is important, to have constant unit!
                pushedBody.Move(_HitForce * force);
            }
        }

    }

    //Animation
    private void SearchPlayer()
    {
        if (_playerDetection.PlayerDetected)    
        {
            StartCoroutine("StartBladeRotation", 0);
            //_animator.SetTrigger("Spotted");

            //_prepareToRotate += Time.deltaTime;
            //if (_prepareToRotate > _TimeToRotate)
            //{
            //    transform.Rotate(0.0f, 0.0f, 360 * _Speed * Time.deltaTime);
            //}
        }
    }

    IEnumerator StartBladeRotation()
    {
        //Quaternion to = Quaternion.Euler(-45, 0, 0);
        //_Head.transform.rotation = Quaternion.Slerp(transform.rotation, to, Time.deltaTime);

        //_Head.transform.Rotate(45, 0, 0);
        _animator.SetTrigger("Spotted");
        yield return new WaitForSeconds(_PrepareToRotate);
        transform.Rotate(0.0f, 0.0f, 360 * _Speed * Time.deltaTime);
    }
}
