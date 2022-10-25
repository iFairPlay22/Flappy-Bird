using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SfxManager))]
public class Player : MonoBehaviour
{
    [Header("Introduction")]
    private bool intro = true;
    Vector3 initialPosition;

    [Header("Game")]
    [SerializeField]
    float jumpSpeed = 75f;

    [SerializeField]
    [Range(0f, 25f)]
    float collisionForce = 10f;

    [Header("SFX")]
    [SerializeField]
    AudioClip jumpAudioClip;

    [SerializeField]
    AudioClip dieAudioClip;

    Rigidbody2D rb;
    SfxManager sfxManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sfxManager = GetComponent<SfxManager>();
        initialPosition = transform.position;
    }
    void Update()
    {
        if (intro)
        {
            if (transform.position.y < initialPosition.y)
                Jump();
        }
    }

    public void StartPlaying()
    {
        intro = false;
    }

    public void Jump()
    {
        sfxManager.Play(jumpAudioClip);
        rb.velocity = Vector2.up * Mathf.Sqrt(jumpSpeed * -2f * (Physics.gravity.y * rb.gravityScale));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            Vector2 sum = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
                sum += collision.GetContact(i).normal;
            
            rb.AddForce(sum / collision.contactCount * collisionForce * 100);

            GameManager.Instance.OnDefeat();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Ennemy")
        {
            GameManager.Instance.OnDefeat();
        }
    }

    public void Die()
    {
        sfxManager.Play(dieAudioClip);
    }
}
