using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PassiveParallax : MonoBehaviour
{
    float length;
    float startPos;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(parallaxEffect, rb.velocity.y);

        if (transform.position.x > startPos + length)
        {
            transform.position = new Vector2(startPos, transform.position.y);
        }
    }
}
