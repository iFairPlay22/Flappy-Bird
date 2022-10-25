using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    float initialY;

    [Header("Movement")]
    [SerializeField]
    [Range(50f, 100f)]
    float xSpeed = 3f;

    [SerializeField]
    [Range(1f, 10f)]
    float ySpeed = 3f;
    
    [SerializeField]
    [Range(1f, 75f)]
    float amplitude = 30f;

    float t = 0f;
    Vector2 size;

    void Awake()
    {
        size = GetComponent<SpriteRenderer>().size * transform.localScale;
    }

    void Start()
    {
        transform.position += new Vector3(size.x, size.y, 0) / 2;
        transform.eulerAngles = 180f * Vector3.up;
        initialY = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x - xSpeed * Time.deltaTime, 
            initialY + amplitude * Mathf.Cos(t * ySpeed),
            transform.position.z
        );
        t += Time.deltaTime % 360;
    }
}
