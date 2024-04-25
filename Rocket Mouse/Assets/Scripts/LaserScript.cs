using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    private bool isLaserOn = true;

    SpriteRenderer spriteRenderer;
    public Sprite laserOnSprite;
    public Sprite laserOffSprite;
    private Collider2D collider2d;
    public float rotationSpeed;

    public float interval;
    private float timeUntilNextToggle;

    private void Start()
    {
        timeUntilNextToggle = interval;
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        timeUntilNextToggle -= Time.fixedDeltaTime;
        if (timeUntilNextToggle <= 0)
        {
            isLaserOn = !isLaserOn;
            collider2d.enabled = isLaserOn;
            if (isLaserOn)
                spriteRenderer.sprite = laserOnSprite;
            else
                spriteRenderer.sprite = laserOffSprite;

            timeUntilNextToggle = interval;
        }

        transform.RotateAround(
            transform.position,
            Vector3.forward,
            rotationSpeed * Time.fixedDeltaTime);
    }
}
