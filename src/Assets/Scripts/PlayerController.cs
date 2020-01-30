using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;

    private Rigidbody2D rb;
    private GameController gc;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Flap();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gc.GameOver();
        }
    }

    public void Flap()
    {
        rb.AddForce(Vector2.up * Force);
    }
}
