using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _jumpInterval = 4f;
    [SerializeField] private float _changeDirectionInterval = 3f;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private float _knockBackThrust = 25f;

    private Rigidbody2D _rigidBody;
    private Movement _movement;
    private ColorChanger _colorChanger;
    

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movement = GetComponent<Movement>();
        _colorChanger = GetComponent<ColorChanger>();
    }

    private void Start() {
        StartCoroutine(ChangeDirection());
        StartCoroutine(RandomJump());
    }

    public void Init(Color color)
    {
        _colorChanger.SetDefaultColor(color);
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            float currentXDirection = UnityEngine.Random.Range(0, 2) * 2 - 1; 
            _movement.SetCurrentDirection(currentXDirection);// 1 or -1 
            yield return new WaitForSeconds(_changeDirectionInterval);
        }
    }

    private IEnumerator RandomJump() 
    {
        while (true)
        {
            yield return new WaitForSeconds(_jumpInterval);
            float randomDirection = Random.Range(-1, 1);
            Vector2 jumpDirection = new Vector2(randomDirection, 1f).normalized;
            _rigidBody.AddForce(jumpDirection * _jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHit player = collision.gameObject.GetComponent<PlayerHit>();
        if (!player || player.IsImmune) { return; }

        IHitable iHitable = collision.gameObject.GetComponent<IHitable>();
        iHitable?.TakeHit();

        IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
        iDamageable?.TakeDamage(transform.position, _damageAmount, _knockBackThrust);

        PlayerHit.OnPlayerHit?.Invoke();
    }

}
