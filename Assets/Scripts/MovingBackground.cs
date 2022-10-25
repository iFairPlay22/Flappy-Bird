using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(SpriteRenderer))]
public class MovingBackground : MonoBehaviour
{
    bool move = false;
    float currentSpeed;
    float maxSpeed;
    float speedAug;
    RawImage rawImage;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (move)
        {
            rawImage.uvRect = new Rect(rawImage.uvRect.position + Vector2.right * currentSpeed * Time.deltaTime, rawImage.uvRect.size);
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, speedAug * Time.deltaTime);
        }
    }

    public void StartMoving(float _minSpeed, float _maxSpeed, float _speedAugmentation)
    {
        move = true;
        currentSpeed = _minSpeed;
        maxSpeed = _maxSpeed;
        speedAug = _speedAugmentation;
    }

    public void StopMoving()
    {
        move = false;
    }
}
