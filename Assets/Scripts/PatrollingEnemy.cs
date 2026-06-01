using System.Collections;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour, IDataPersistence
{
    [Header("Components")]
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private float speed;
    [SerializeField] private float waitingTime;
    [SerializeField] private int score;
    [SerializeField] private bool drawGismosHelper = false;

    [SerializeField, ReadOnly] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private Animator _animator;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _collider;
    private BoxCollider2D _triggerCollider;
    private AudioSource _audio;
    private SpriteRenderer _sprite;
    private Transform currentPoint;

    private bool isDead;
    private bool isWaiting;
    private bool isGamePaused = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
        _triggerCollider = GetComponent<BoxCollider2D>();
        _audio = GetComponent<AudioSource>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        currentPoint = pointA.transform;
        GameEventsManager.instance.OnGamePaused += GamePaused;

    }
    void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.OnGamePaused -= GamePaused;
        }
    }
    private void GamePaused()
    {
        isGamePaused = !isGamePaused;
    }

    void FixedUpdate()
    {
        if (!isDead && !isGamePaused)
        {
            FacePosition();
            MoveTowards();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }


    }
    private void FacePosition()
    {
        //Changing target point
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            currentPoint = pointA.transform;
            StartCoroutine(WaitCoroutine(waitingTime, false));          //Face to default (left)
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            currentPoint = pointB.transform;
            StartCoroutine(WaitCoroutine(waitingTime, true));          //Face to opposite (right)
        }
    }

    private IEnumerator WaitCoroutine(float waitSecond, bool faceRight)
    {
        isWaiting = true;
        _animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitSecond);
        _sprite.flipX = faceRight;
        isWaiting = false;
    }

    private void MoveTowards()
    {
        if (!isWaiting)
        {
            if (currentPoint == pointB.transform)
            {
                _animator.SetBool("isWalking", true);
                _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
            }

            if (currentPoint == pointA.transform)
            {
                _animator.SetBool("isWalking", true);
                _rb.linearVelocity = new Vector2(-speed, _rb.linearVelocity.y);
            }

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead)
        {
            if (collision.collider.CompareTag("Player"))
            {
                PlayerController player = collision.collider.GetComponent<PlayerController>();
                player.GetHurt();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                player.KillJump();
                StartCoroutine(DieCoroutine(0.3f));
            }
        }
    }

    private IEnumerator DieCoroutine(float time)
    {
        isDead = true;
        _animator.SetBool("isDying", true);
        _collider.enabled = false;
        _triggerCollider.enabled = false;
        AudioSource.PlayClipAtPoint(_audio.clip, transform.position);
        GameEventsManager.instance.EnemyDeath(score);
        yield return new WaitForSeconds(time);
        _sprite.gameObject.SetActive(false);
    }


    private void OnDrawGizmos()
    {
        if (drawGismosHelper)
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
            Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
            Gizmos.DrawLine(transform.position + new Vector3(0f, 0.1f, 0f), transform.position - new Vector3(0f, 0.1f, 0f));
        }
    }

    public void LoadData(GameData data)
    {
        data.enemiesKilled.TryGetValue(id, out isDead);
        if (isDead)
        {
            _sprite.gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.enemiesKilled.ContainsKey(id))
        {
            data.enemiesKilled.Remove(id);
        }
        data.enemiesKilled.Add(id, isDead);
    }


}
