using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class ObstacleNoise : MonoBehaviour
{
    [Header("Noise Settings")]
    public float noiseAmount = 20f;

    [Header("Sound")]
    public AudioClip hitSound;

    [Header("Hit Control")]
    public float cooldown = 0.5f;

    private AudioSource audioSource;
    private float lastHitTime = -999f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < lastHitTime + cooldown)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            lastHitTime = Time.time;

            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (NoiseManager.Instance != null)
            {
                NoiseManager.Instance.AddNoise(noiseAmount);
            }
        }
    }
}