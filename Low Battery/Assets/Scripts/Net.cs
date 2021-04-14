using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    private float speed = 10f;
    float timeAlive = 0;
    Rigidbody2D rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.AddForce(transform.TransformDirection(Vector2.right) * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (timeAlive < 1)
        {
            timeAlive += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            Destroy(this.gameObject);
        }
    }
}
