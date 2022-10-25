using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MovingPlatform : MonoBehaviour
{
    bool move = false;
    float currentSpeed;
    float maxSpeed;
    float speedAug;

    void Update()
    {
        if (move)
        {
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            transform.position += Vector3.left * currentSpeed * 100 * Time.deltaTime;
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
