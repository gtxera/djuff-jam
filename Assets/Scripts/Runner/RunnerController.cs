using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RunnerController : MonoBehaviour
{
    private Rigidbody2D _rb2d;

    [SerializeField] private float _defaultDecay = 10f;

    [SerializeField] private float _shipRadius;
    [SerializeField] private Vector2 _centerOffset;

    private bool _running;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        RunnerManager.Instance.RunnerStarted += OnRunnerStarted;
        RunnerManager.Instance.RunnerGameOver += OnRunnerEnded;
        _running = true;
    }

    private void OnDestroy()
    {
        RunnerManager.Instance.RunnerStarted -= OnRunnerStarted;
        RunnerManager.Instance.RunnerGameOver -= OnRunnerEnded;
    }

    private void OnRunnerStarted()
    {
        _running = true;
        Debug.Log("running");
    }

    private void OnRunnerEnded()
    {
        _running = false;
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
