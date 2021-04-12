using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;

    private void Start() => Initialize();

    private void Initialize() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Knockout() {
        print("Knocked out!");
        //Play Knockout Animation
        rigidBody.bodyType = RigidbodyType2D.Static;
        boxCollider.enabled = false;
    }

    IEnumerator IdleAction()
    {
        yield return new WaitForSeconds(0f);


    }
}
