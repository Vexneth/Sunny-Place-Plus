using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour, IDataPersistence
{
    [SerializeField] int score;

    [SerializeField, ReadOnly] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private bool isCollected;
    private AudioSource _audio;
    private Animator _animator;
    private SpriteRenderer _sprite;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isCollected && collision.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        isCollected = true;
        _animator.SetBool("isCollected", true);
        AudioSource.PlayClipAtPoint(_audio.clip, transform.position);
        GameEventsManager.instance.CollectableCollected(score);
        StartCoroutine(CollectCoroutine());
    }

    private IEnumerator CollectCoroutine()
    {
        yield return new WaitForSeconds(0.28f);
        _sprite.gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        data.collectablesCollected.TryGetValue(id, out isCollected);
        if (isCollected)
        {
            _sprite.gameObject.SetActive(false);
        }
    }
    
    public void SaveData(GameData data)
    {
        if (data.collectablesCollected.ContainsKey(id))
        {
            data.collectablesCollected.Remove(id);
        }
        data.collectablesCollected.Add(id, isCollected);
    }

}
