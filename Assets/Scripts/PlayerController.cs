using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("= Components =")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask onlyGroundLayers;
    [SerializeField] private CinemachineCamera cmCamera;


    [Header("= Speed Settings =")]
    [SerializeField] float speedMax;
    [SerializeField] float speedUpMultiplier;
    [SerializeField] float speedDownMultiplier;
    [SerializeField] float jumpPower;
    [SerializeField] float jumpAllowDelayTime;
    [SerializeField] float jumpCoyoteTime;

    private float jumpCoyoteTimeCounter;



    [Header("= Collusion =")]
    [SerializeField] private float groundLength;
    [SerializeField] private float horizontalLength;
    [SerializeField] private Vector2 horizontalColliderSize;
    [SerializeField] private Vector2 horizontalColliderOffset;

    [SerializeField] private Vector3 colliderOffset;
    [SerializeField] private float colliderXSetting;
    [SerializeField] private Boolean drawAllGizmos;
    [SerializeField] private Boolean drawGroundRaycast;
    [SerializeField] private Boolean drawBoxRaycast;


    private String currentLevel;
    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;
    private Animator _animator;
    private CapsuleCollider2D _collider;

    private Vector2 currentSpeedVector = Vector2.zero;
    private Coroutine jumpBuffer;
    private GameObject currentPlatform;


    private bool onGround = false;
    private bool rightBoxcast;
    private bool leftBoxcast;
    private bool rightInput;
    private bool leftInput;
    private bool jumpInput;
    private bool downInput;
    private bool pauseInput;
    private bool isClickingAny;
    private bool isClickingBoth;
    private bool isOnPlatform;
    private bool isHurt = false;
    private bool isGamePaused = false;
    private bool isPlayerPaused = false;
    private Rigidbody2D platformRb;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();
    }
    void Start()
    {
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
        HandleRaycasts();
        HandleInputs();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
        }
    }

    private void HandleRaycasts()
    {
        Vector3 firstLinePoint = transform.position + colliderOffset;
        firstLinePoint.x += colliderXSetting;
        Vector3 secondLinePoint = transform.position + colliderOffset;
        secondLinePoint.x -= colliderXSetting;

        onGround = Physics2D.Raycast(firstLinePoint, Vector2.down, groundLength, groundLayers) ||
                    Physics2D.Raycast(secondLinePoint, Vector2.down, groundLength, groundLayers);

        if (onGround)
        {
            jumpCoyoteTimeCounter = jumpCoyoteTime;
        }
        else
        {
            jumpCoyoteTimeCounter -= Time.deltaTime;
        }


        Vector2 boxcastOrigin = new Vector2(transform.position.x + horizontalColliderOffset.x, transform.position.y + horizontalColliderOffset.y);

        rightBoxcast = Physics2D.BoxCast(boxcastOrigin, horizontalColliderSize, 0f, Vector2.right, horizontalLength, onlyGroundLayers);
        leftBoxcast = Physics2D.BoxCast(boxcastOrigin, horizontalColliderSize, 0f, Vector2.left, horizontalLength, onlyGroundLayers);
    }

    private void HandleInputs()
    {
        if (isHurt || isPlayerPaused)
        {
            rightInput = false;
            leftInput = false;
            downInput = false;
            pauseInput = false;
            isClickingBoth = false;
            isClickingAny = false;
            return;
        }
        pauseInput = Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame;
        if (pauseInput)
        {
            GameEventsManager.instance.PauseKeyPressed();
        }

        if (isGamePaused)
        {
            return;
        }
        rightInput = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        leftInput = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        bool rawDownInput = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
        downInput = rawDownInput && !rightInput && !leftInput;


        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            if (jumpBuffer != null)
                StopCoroutine(jumpBuffer);
            jumpBuffer = StartCoroutine(JumpBuffer(jumpAllowDelayTime));
        }



        isClickingBoth = rightInput && leftInput;
        isClickingAny = rightInput || leftInput;
    }

    //Keeps jump input active briefly, allowing jumps pressed x second ago just before landing to trigger.
    private IEnumerator JumpBuffer(float timeInSeconds)
    {
        jumpInput = true;
        yield return new WaitForSeconds(timeInSeconds);
        jumpInput = false;
    }

    private void HandleMovement()
    {
        if (isHurt)
        {
            return;
        }
        if (isGamePaused)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0f;
            return;
        }
        else
        {
            _rb.gravityScale = 4f;
        }
        HandleHorizontalMovement();
        HandleVerticalMovement();
        HandleCrouching();
        if (isOnPlatform)
        {
            _rb.linearVelocity = new Vector2(currentSpeedVector.x + platformRb.linearVelocity.x, _rb.linearVelocity.y);
        }
        else
        {
            _rb.linearVelocity = new Vector2(currentSpeedVector.x, _rb.linearVelocity.y);
        }


    }

    private void HandleVerticalMovement()
    {
        if (jumpInput && jumpCoyoteTimeCounter > 0f)
        {
            StartCoroutine(JumpSqueeze(0.85f, 1.15f, 0.1f));
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * 10 * jumpPower, ForceMode2D.Impulse);
            jumpInput = false;
            jumpCoyoteTimeCounter = 0f;
        }

    }

    private void HandleHorizontalMovement()
    {
        //Slow down and stop
        if (!isClickingAny || isClickingBoth)
        {
            if (Math.Abs(currentSpeedVector.x) <= speedDownMultiplier)
            {
                currentSpeedVector.x = 0;
            }
            else
            {
                currentSpeedVector.x = (Math.Abs(currentSpeedVector.x) - speedDownMultiplier) * Math.Sign(currentSpeedVector.x);
            }
        }

        if (rightInput)
        {
            currentSpeedVector += Vector2.right * speedUpMultiplier;
        }

        if (leftInput)
        {
            currentSpeedVector += Vector2.left * speedUpMultiplier;
        }

        if (Math.Abs(currentSpeedVector.x) >= speedMax)
        {
            currentSpeedVector.x = speedMax * Math.Sign(currentSpeedVector.x);
        }

        //if right boxcast hits, stop going right
        if (rightBoxcast && currentSpeedVector.x > 0)
        {
            currentSpeedVector.x = 0;
        }
        //if left boxcast hits, stop going left
        if (leftBoxcast && currentSpeedVector.x < 0)
        {
            currentSpeedVector.x = 0;
        }
    }

    private void HandleCrouching()
    {
        if (downInput && currentPlatform != null)
        {
            StartCoroutine(DisableCollusion());
        }
    }

    private IEnumerator DisableCollusion()
    {
        CompositeCollider2D platformCollider = currentPlatform.GetComponent<CompositeCollider2D>();

        Physics2D.IgnoreCollision(_collider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(_collider, platformCollider, false);
    }
    private void HandleAnimation()
    {
        if (isGamePaused)
        {
            return;
        }

        _animator.SetBool("IsWalking", false);
        _animator.SetBool("IsCrouching", false);

        if (leftInput && !rightInput)
        {
            //if walking left:
            _renderer.flipX = true;
            _animator.SetBool("IsWalking", true);
        }
        if (!leftInput && rightInput)
        {
            //if walking right:
            _renderer.flipX = false;
            _animator.SetBool("IsWalking", true);
        }
        if (jumpInput)
        {
            //if jumping
            _animator.SetInteger("VerticalSign", 1);
        }
        else if (_rb.linearVelocityY >= -1 && _rb.linearVelocityY <= 1)
        {
            //if not moving horizontally
            _animator.SetInteger("VerticalSign", 0);
        }
        if (_rb.linearVelocity.y < -1)
        {
            //if going down/falling
            _animator.SetInteger("VerticalSign", -1);
        }
        if (downInput)
        {
            _animator.SetBool("IsCrouching", true);
        }


    }

    //Squeezes char model when jumping
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }
    }

    public void SetIsOnPlatform(bool bl, Rigidbody2D rb)
    {
        if (bl)
        {
            isOnPlatform = true;
            platformRb = rb;
        }
        else
        {
            isOnPlatform = false;
            platformRb = null;
        }
    }

    public void KillJump()
    {
        StartCoroutine(JumpSqueeze(0.85f, 1.15f, 0.1f));
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        _rb.AddForce(Vector2.up * 8 * jumpPower, ForceMode2D.Impulse);
        jumpInput = false;
    }

    public void GetHurtWithoutAnim()
    {
        isHurt = true;
    }
    public void GetHurt()
    {
        isHurt = true;
        _collider.enabled = false;

        cmCamera.Target.TrackingTarget = null;
        StartCoroutine(GetHurtAnimation(0.5f));
        GameEventsManager.instance.PlayerDeath();
    }
    private IEnumerator GetHurtAnimation(float waitingTime)
    {
        var gravity = _rb.gravityScale;
        _animator.SetBool("IsHurt", true);
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
        yield return new WaitForSeconds(waitingTime);
        _rb.gravityScale = gravity;
        _rb.AddForce(Vector2.up * 8 * jumpPower, ForceMode2D.Impulse);
    }

    public bool IsPlayerHurt()
    {
        return isHurt;
    }
    public void PausePlayer()
    {
        isPlayerPaused = !isPlayerPaused;
    }
    public void ChangeLevel(String levelName)
    {
        currentLevel = levelName;
    }
    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
        currentLevel = data.currentLevel;
    }

    public void SaveData(GameData data)
    {

        if (isHurt)
        {
            data.playerPosition = new Vector3(19, 4, 0);
            data.IsPlayerHurt = true;
        }
        else
        {
            data.playerPosition = transform.position;
        }

        data.currentLevel = currentLevel;
    }

    //Draws gizmos of ground check lines with red 
    private void OnDrawGizmos()
    {
        if (drawAllGizmos || drawGroundRaycast)
        {
            Gizmos.color = Color.red;
            Vector3 firstLinePoint = transform.position + colliderOffset;
            firstLinePoint.x += colliderXSetting;
            Vector3 secondLinePoint = transform.position + colliderOffset;
            secondLinePoint.x -= colliderXSetting;

            Vector3 downOffset = Vector3.down * groundLength;

            Gizmos.DrawLine(firstLinePoint, firstLinePoint + downOffset);
            Gizmos.DrawLine(secondLinePoint, secondLinePoint + downOffset);
        }

        if (drawAllGizmos || drawBoxRaycast)
        {
            Color gizmoColor = Color.aliceBlue;
            if (rightBoxcast)
            {
                gizmoColor = Color.softRed;
            }
            Vector2 rightBoxcastOrigin = new Vector2(transform.position.x + horizontalLength + horizontalColliderOffset.x, transform.position.y + horizontalColliderOffset.y);
            drawGizmosRectangle(rightBoxcastOrigin, horizontalColliderSize, gizmoColor);

            gizmoColor = Color.aliceBlue;
            if (leftBoxcast)
            {
                gizmoColor = Color.softRed;
            }
            Vector2 leftBoxcastOrigin = new Vector2(transform.position.x - horizontalLength + horizontalColliderOffset.x, transform.position.y + horizontalColliderOffset.y);
            drawGizmosRectangle(leftBoxcastOrigin, horizontalColliderSize, gizmoColor);


        }
    }

    private void drawGizmosRectangle(Vector2 origin, Vector2 size, Color color)
    {

        Vector2 solUstKose = new Vector2(origin.x - size.x / 2, origin.y + size.y / 2);
        Vector2 sagUstKose = new Vector2(origin.x + size.x / 2, origin.y + size.y / 2);
        Vector2 solAltKose = new Vector2(origin.x - size.x / 2, origin.y - size.y / 2);
        Vector2 sagAltKose = new Vector2(origin.x + size.x / 2, origin.y - size.y / 2);

        Gizmos.color = color;
        Gizmos.DrawLine(solUstKose, sagUstKose);
        Gizmos.DrawLine(sagUstKose, sagAltKose);
        Gizmos.DrawLine(sagAltKose, solAltKose);
        Gizmos.DrawLine(solAltKose, solUstKose);
    }
}
