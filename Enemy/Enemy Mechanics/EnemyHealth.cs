using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IBurnable, IDamageable
{
    [SerializeField] private PlayerInfo _playerInfo;

    [SerializeField] private int _EnemyHealth;
    [SerializeField] private bool _IsBurning;
    public int Health { get => _EnemyHealth; set => _EnemyHealth = value; }
    public bool IsBurning { get => _IsBurning; set => _IsBurning = value; }
    private Coroutine _burnCoroutine;
    public delegate void DeathEvent(EnemyHealth enemy);
    public event DeathEvent OnDeath;

    [SerializeField] private Slider _HealthBarSlider;

    //Loot
    [SerializeField] private LootTable _LootTable;

    //Player Detection
    private PlayerDetection _playerDetection;

    private void Start()
    {
        _playerDetection = GetComponent<PlayerDetection>();
    }

    private void Update()
    {
        _HealthBarSlider.value = _EnemyHealth;

        if (_EnemyHealth <= 0)
        {
            Destroy(gameObject);

            //Loot
            Loot loot = _LootTable.GetDrop();
            Instantiate(loot.LootItem, transform.position, transform.rotation);

            _EnemyHealth--;
            _playerInfo._ultimatePoints += 5;
                Destroy(gameObject);
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            Health = 0;
            OnDeath?.Invoke(GetComponent<EnemyHealth>());
            StopBurning();
        }
    }
    public void StartBurning(int DamagePerSecond)
    {
        IsBurning = true;
        if (_burnCoroutine != null)
        {
            StopCoroutine(_burnCoroutine);
        }
        _burnCoroutine = StartCoroutine(Burn(DamagePerSecond));
    }
    private IEnumerator Burn(int DamagePerSecond)
    {
        float minTimeToDamage = 1f / DamagePerSecond;
        WaitForSeconds wait = new WaitForSeconds(minTimeToDamage);
        int damagePerTick = Mathf.FloorToInt(minTimeToDamage) + 1;
        TakeDamage(damagePerTick);
        while (IsBurning)
        {
            yield return wait;
            TakeDamage(damagePerTick);
        }
    }
    public void StopBurning()
    {
        IsBurning = false;
        if (_burnCoroutine != null)
        {
            StopCoroutine(_burnCoroutine);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBullet")
        {
            _EnemyHealth -= _playerInfo._meleeDamage;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullet")
        {
            _EnemyHealth -= collision.gameObject.GetComponent<PlayerBullet>().damage;

            _playerDetection.PlayerDetected = true;
        }
        
        if (_EnemyHealth <= 0)
        {
            _EnemyHealth = 0;         
        }
    }
}
