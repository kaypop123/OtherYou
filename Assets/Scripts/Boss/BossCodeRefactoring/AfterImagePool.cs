using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float afterImageLifetime = 0.3f;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(afterImagePrefab);

            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public void ShowAfterImage(SpriteRenderer source)
    {
        if (source == null)
            return;

        if (pool.Count == 0)
            return;

        GameObject obj = pool.Dequeue();

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("AfterImage SpriteRenderer 없음");
            pool.Enqueue(obj);
            return;
        }

        sr.sprite = source.sprite;
        sr.flipX = source.flipX;

        sr.sortingLayerID = source.sortingLayerID;
        sr.sortingOrder = source.sortingOrder - 1;

        sr.color = new Color(1f, 0.3f, 0.1f, 0.7f);

        obj.transform.position = source.transform.position;
        obj.transform.rotation = source.transform.rotation;
        obj.transform.localScale = source.transform.localScale;

        obj.SetActive(true);

        StartCoroutine(ReturnToPool(obj, sr));
    }

    private IEnumerator ReturnToPool(
        GameObject obj,
        SpriteRenderer sr)
    {
        float elapsed = 0f;

        Color startColor = sr.color;

        while (elapsed < afterImageLifetime)
        {
            elapsed += Time.deltaTime;

            float alpha =
                Mathf.Lerp(
                    startColor.a,0f, elapsed / afterImageLifetime
                );

            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        obj.SetActive(false);

        // 다 사용한 다음에 다시 풀에 반환
        pool.Enqueue(obj);
    }
}