using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform posA, posB;
    [SerializeField] float speed;
    [SerializeField] float waitDuration;
    [SerializeField] private bool drawGismosHelper = false;

    private Vector3 targetPos;

    private PlayerController playerController;
    private Rigidbody2D _rb;
    private Vector3 moveDirection;
    private bool isWaiting;
    private bool isGamePaused = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        targetPos = posB.position;
        isWaiting = false;
        CalculateDirection();
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

    void Update()
    {
        if (Vector2.Distance(transform.position, posA.position) < 0.05f && !isWaiting)
        {
            moveDirection = Vector2.zero;
            targetPos = posB.position;
            isWaiting = true;
            StartCoroutine(WaitAndCalculateDirection(waitDuration));
        }
        if (Vector2.Distance(transform.position, posB.position) < 0.05f && !isWaiting)
        {
            moveDirection = Vector2.zero;
            targetPos = posA.position;
            isWaiting = true;
            StartCoroutine(WaitAndCalculateDirection(waitDuration));
        }

    }

    void FixedUpdate()
    {

        if (isGamePaused)
        {
            _rb.linearVelocity = Vector2.zero;
        }
        else
        {
            _rb.linearVelocity = moveDirection * speed;
        }

    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.SetIsOnPlatform(true, _rb);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController.SetIsOnPlatform(false, _rb);
        }
    }


    private IEnumerator WaitAndCalculateDirection(float second)
    {
        yield return new WaitForSeconds(second);
        CalculateDirection();
        yield return new WaitForSeconds(0.5f);
        isWaiting = false;
    }

    private void CalculateDirection()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void OnDrawGizmos()
    {
        if (drawGismosHelper)
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
            Gizmos.DrawWireSphere(posA.transform.position, 0.5f);
            Gizmos.DrawWireSphere(posB.transform.position, 0.5f);
            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            Gizmos.DrawLine(posA.transform.position, posB.transform.position);
            Gizmos.DrawLine(transform.position + new Vector3(0f, 0.1f, 0f), transform.position - new Vector3(0f, 0.1f, 0f));
        }

    }

}
