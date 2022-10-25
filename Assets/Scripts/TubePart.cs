using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TubePart : MonoBehaviour
{
    [Header("Generation")]
    Vector2 spriteSize;

    void Awake()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
    }

    public void CreateTubePart(Vector2 newSize, bool topDirection = true)
    {
        Vector2 newScale     = newSize / spriteSize;
        transform.localScale = new Vector3(newScale.x, newScale.y, 1);

        Vector2 newPosition = newSize / 2;
        transform.position  = transform.position + new Vector3(newPosition.x, newPosition.y, 0);

        if (!topDirection)
        {
            transform.eulerAngles = 180f * Vector3.right;
        }
    }
}
