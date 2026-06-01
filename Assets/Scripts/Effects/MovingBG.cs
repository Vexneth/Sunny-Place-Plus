using UnityEngine;

public class MovingBG : MonoBehaviour
{
    [Range(-1f, 1f)]
    [SerializeField] float speed;

    private Material mat;
    private float distance;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        distance += Time.deltaTime * speed;
        mat.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
