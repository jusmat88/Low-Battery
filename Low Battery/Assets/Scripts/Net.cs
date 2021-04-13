using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    public Vector2 direction;
    public Quaternion rotation;
    public float speed = 0.001f;
    float timeAlive = 0;

    void Update()
    {
        transform.rotation = rotation;

        transform.Translate(-direction * speed);

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
