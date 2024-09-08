using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RunnerController : MonoBehaviour
{
    private Rigidbody2D _rb2d;

    [SerializeField] private float _defaultDecay = 10f;

    [SerializeField] private float _shipRadius;
    [SerializeField] private Vector2 _centerOffset;

    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Animator _animator;

    private bool _running;

    private Vector2 _initialPosition;


    private IEnumerator Start()
    {
        while (RunnerManager.Instance is null)
            yield return null;

        _rb2d = GetComponent<Rigidbody2D>();
        RunnerManager.Instance.RunnerStarted += OnRunnerStarted;
        RunnerManager.Instance.RunnerGameOver += OnRunnerEnded;
        RunnerManager.Instance.RunnerWin += OnRunnerWin;
        _sprite.color = PlayerController.Instance.playerSprite.color;
        _running = true;
        _initialPosition = _rb2d.position;
    }

    private void OnDestroy()
    {
        RunnerManager.Instance.RunnerStarted -= OnRunnerStarted;
        RunnerManager.Instance.RunnerGameOver -= OnRunnerEnded;
        RunnerManager.Instance.RunnerWin -= OnRunnerWin;
    }

    private void OnRunnerStarted()
    {
        _running = true;
        _sprite.color = PlayerController.Instance.playerSprite.color;
        _animator.speed = 1f;
    }

    private void OnRunnerEnded()
    {
        _running = false;
        _animator.speed = 0f;
    }

    private void OnRunnerWin()
    {
        _running = false;
        _animator.Play("GameWin");
    }
    
    private void OnRunnerLeave()
    {
        _rb2d.MovePosition(_initialPosition);
    }

    private void FixedUpdate()
    {
        if (!_running)
            return;

        MoveDecay();

        var collider = Physics2D.OverlapCircle(_rb2d.position + _centerOffset, _shipRadius);

        if (!(collider is null))
        {
            var movingObject = collider.GetComponent<MovingObject>();

            movingObject.Collide();
        }
    }

    private void MoveDecay()
    {
        var mousePos = Mouse.current.position;
        var worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var newPos = new Vector2(_rb2d.position.x, Decay(_rb2d.position.y, worldPos.y, _defaultDecay, Time.fixedDeltaTime));

        _rb2d.MovePosition(newPos);
    }

    private float Decay(float a, float b, float decay, float dt)
    {
        return b+(a-b)*Mathf.Exp(-decay*dt);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(_centerOffset.x, _centerOffset.y, 0), _shipRadius);
    }
}
