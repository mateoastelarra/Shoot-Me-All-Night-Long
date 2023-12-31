using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    public int StartingHealth => _startingHealth;
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }

    public GameObject SplatterPrefab => _splatterPrefab;
    public GameObject DeathVFXPrefab => _deathVFXPrefab;

    public static Action<Health> OnDeath;

    [SerializeField] private GameObject _splatterPrefab;
    [SerializeField] private GameObject _deathVFXPrefab;
    [SerializeField] private int _startingHealth = 3;

    private Knockback _knockback;
    private Flash _flash;
    private Health _health;
    private int _currentHealth;

    private void Awake()
    {
        _knockback = GetComponent<Knockback>();
        _flash = GetComponent<Flash>();
        _health = GetComponent<Health>();
        ResetHealth();
    }

    public void ResetHealth() {
        _currentHealth = _startingHealth;
    }

    public void TakeDamage(int amount) {
        _currentHealth -= amount;

        if (_currentHealth <= 0) {
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(Vector2 damageSourceDirection, int damageAmount, float knockbackTrhust)
    {
        _health.TakeDamage(damageAmount);
        _knockback?.GetKnockedBack(damageSourceDirection, knockbackTrhust);
    }

    public void TakeHit()
    {
        _flash.StartFlash();
    }

}
