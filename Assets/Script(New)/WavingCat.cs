using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WavingCat : 
MonoBehaviour,
IPointerDownHandler,
IPointerUpHandler

{
    [SerializeField] private List<AudioClip> meowAudio;

    [SerializeField] private AudioSource audioSource;

    private Vector3 originalScale;
    private Coroutine bounceCoroutine;
    private IEnumerator Bounce()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Vector3 squashed = new Vector3(
            originalScale.x * 1.1f,
            originalScale.y * 0.9f,
            originalScale.z
        );

        // Squash
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);

            transform.localScale = Vector3.Lerp(
                originalScale,
                squashed,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        // Return to normal
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);

            transform.localScale = Vector3.Lerp(
                squashed,
                originalScale,
                t
            );

            yield return null;
        }

        transform.localScale = originalScale;
        bounceCoroutine = null;
    }

    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (bounceCoroutine != null) {
            StopCoroutine(bounceCoroutine);
        }

        bounceCoroutine = StartCoroutine(Bounce());

        audioSource.PlayOneShot(meowAudio[Random.Range(0, meowAudio.Count)]);

        Debug.Log("CAT");
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
