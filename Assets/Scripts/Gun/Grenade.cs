using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class Grenade : MonoBehaviour
{
    public Action OnTick;
    public Action OnExplode;

    [SerializeField] private GameObject _grenadeVFXPrefab;
    
    [SerializeField] private float _launchForce = 20f;
    [SerializeField] private float _torqueAmount = 2f;
    [Header("Explotion and Damage")]
    [SerializeField] private float _explotionRadius = 1f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private int _damageAmount = 3;
    [Header("Tick and Beep")]
    [SerializeField] [Range(1, 3)]  private float _explotionTime = 2f;
    [SerializeField] private int _amountOfTicks = 3;
    [SerializeField] private float _tickTime = 0.1f; 
    

    private Vector2 _fireDirection;
    private Gun _gun;
    private Rigidbody2D _rigidBody;
    private Light2D _tickingLight;
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _tickingLight = GetComponentInChildren<Light2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        _rigidBody.AddForce(_fireDirection * _launchForce, ForceMode2D.Impulse);
        _rigidBody.AddTorque(_torqueAmount, ForceMode2D.Impulse);
        StartCoroutine(GrenadeRoutine());
    }

    private void OnEnable()
    {
        OnTick += TickLight;
        OnTick += AudioManager.Instance.Grenade_OnTick;
        OnExplode += ExplodeVFX;
        OnExplode += DamageEnemies;
        OnExplode += GrenadeScreenShake;
        OnExplode += AudioManager.Instance.Grenade_OnExplode;
    }

    private void OnDisable()
    {
        OnTick -= TickLight;
        OnTick -= AudioManager.Instance.Grenade_OnTick;
        OnExplode -= ExplodeVFX;
        OnExplode -= DamageEnemies;
        OnExplode -= GrenadeScreenShake;
        OnExplode -= AudioManager.Instance.Grenade_OnExplode;
    }

    public void Init(Gun gun, Vector2 bulletSpawnPos, Vector2 mousePos)
    {
        _gun = gun;
        transform.position = bulletSpawnPos;
        _fireDirection = (mousePos - bulletSpawnPos).normalized;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<IDamageable>() != null)
        {
            Explode();
        }
    }

    private IEnumerator GrenadeRoutine()
    {
        int ticks = 0;
        while (ticks < _amountOfTicks)
        {
            yield return new WaitForSeconds(_explotionTime / (_amountOfTicks * 2));
            ticks += 1;
            OnTick?.Invoke();
            yield return new WaitForSeconds(_explotionTime / (_amountOfTicks * 2));
        }
        Explode();
    }

    private void TickLight()
    {
        StartCoroutine(TickLightRoutine());
    }

    private IEnumerator TickLightRoutine()
    {
        _tickingLight.enabled = true;
        yield return new WaitForSeconds(_tickTime);
        _tickingLight.enabled = false;
    }

    private void Explode()
    {
        OnExplode?.Invoke();
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    private void ExplodeVFX()
    {
        Instantiate(_grenadeVFXPrefab, transform.position, transform.rotation);
    }

    private void DamageEnemies()
    {
        Collider2D[] enemyHits = Physics2D.OverlapCircleAll(transform.position, _explotionRadius, _enemyLayerMask);
        foreach (Collider2D collider in enemyHits)
        {
            IDamageable iDamageable = collider.gameObject.GetComponent<IDamageable>();
            iDamageable?.TakeDamage(_fireDirection, _damageAmount);
        }
    }

    private void GrenadeScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }
}
