using UnityEngine;

public class FlyingEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float height;
    [SerializeField] float updateTimer;

    private float currentTime;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {

        if (currentTime >= updateTimer)
        {
            float newPosY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
            transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
            currentTime = 0.0f;
        }
        currentTime += Time.deltaTime;

    }


}
