using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("= Settings =")]
    [SerializeField] private Vector2 parallaxPower;
    [SerializeField] private Vector2 parallaxOffset;

    private GameObject player;

    private PlayerController playerController;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerController.IsPlayerHurt())
            ParallaxEffect(parallaxPower);
    }

    void ParallaxEffect(Vector2 parallaxPower)
    {
        var playerpos = player.transform.position;
        var parallaxPos = new Vector2(playerpos.x * (parallaxPower.x), playerpos.y * (parallaxPower.y));
        transform.position = parallaxPos + parallaxOffset;
    }
}
