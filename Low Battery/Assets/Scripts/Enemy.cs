using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;
    [SerializeField] private float reactionDistance;
    [SerializeField] private float catchDistance;
    [SerializeField] private Transform player;
    private const string IDLE = "Idle";
    private const string CHASING = "Chasing";
    private const string CATCHING = "Catching";
    [SerializeField] private string currentState;

    bool isChasing = false;
    bool isCatching = false;
    bool isIdle = true;

    bool isConcious = true;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, reactionDistance);
    }

    private void Start() => Initialize();

    private void FixedUpdate()
    {
        if (!isConcious) { return; }
        Vector3 playerDirection = ((player.position + new Vector3(0, 0.25f, 0)) - transform.position).normalized;
        RaycastHit2D ray = Physics2D.Raycast(transform.position + new Vector3(0,0.25f,0), playerDirection, reactionDistance);
        
        float distance = Vector3.Distance(transform.position, player.position);

        Debug.DrawRay(transform.position + new Vector3(0, 0.15f, 0), playerDirection * reactionDistance);

        if (ray.collider.GetComponent<PlayerController>())
        {
            if (distance < reactionDistance)
            {
                if (distance > catchDistance)
                {
                    ChangeState(CHASING);
                }
                
                if (distance <= catchDistance)
                {
                    ChangeState(CATCHING);
                }
            }
        }
        else
        {
            ChangeState(IDLE);
        }
    }

    private void Initialize() {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        ChangeState(IDLE);
        player = FindObjectOfType<PlayerController>().transform;
    }

    public void Knockout() {
        //Play Knockout Animation
        rigidBody.bodyType = RigidbodyType2D.Static;
        boxCollider.enabled = false;
        isConcious = false;
    }

    IEnumerator IdleAction()
    {
        isIdle = true;
        //Play Idle Animation
        yield return new WaitForSeconds(2f);
        StartCoroutine(IdleAction());

    }

    IEnumerator ChaseAction()
    {
        isChasing = true;
        yield return new WaitForSeconds(1f);
    }

    IEnumerator CatchAction()
    {
        //Play Catch Animation
        isCatching = true;
        yield return new WaitForSeconds(1f);
    }

    private void ChangeState(string _newState)
    {
        if (_newState == currentState) { return; }
        currentState = _newState;
        StopAllCoroutines();

        switch (currentState)
        {
            case IDLE:
                StartCoroutine(IdleAction());
                break;
            case CHASING:
                StartCoroutine(ChaseAction());
                break;
            case CATCHING:
                StartCoroutine(CatchAction());
                break;
            default:
                return;
        }
    }
}
