using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PassiveParallax : MonoBehaviour
{
    float length;
    float startPos;
    Transform cam;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(parallaxEffect, rb.velocity.y);
        float restPos = cam.position.x *  - parallaxEffect;
        float distance = rb.velocity.x;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (restPos > startPos + length)
        {
            startPos += length;
        }
        else if (restPos < startPos - length)
        {
            startPos -= length;
        }
    }
}
