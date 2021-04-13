using System.Collections;
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
    [SerializeField] private float speed;
    public GameObject net;

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

        float distance = Vector3.Distance(transform.position, player.position);
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
        else
        {
            ChangeState(IDLE);
        }

        Vector2 playerDir = (player.position - transform.position).normalized;
        if (isChasing && isConcious)
        {
            rigidBody.velocity = new Vector2(playerDir.x * speed, rigidBody.velocity.y);
        }
    }

    private void Initialize()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        ChangeState(IDLE);
        player = FindObjectOfType<PlayerController>().transform;
    }

    public void Knockout()
    {
        //Play Knockout Animation
        rigidBody.bodyType = RigidbodyType2D.Static;
        boxCollider.enabled = false;
        isConcious = false;
    }

    IEnumerator IdleAction()
    {
        isChasing = false;
        isIdle = true;
        //Play Idle Animation
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        yield return new WaitForSeconds(2f);
        StartCoroutine(IdleAction());

    }

    IEnumerator ChaseAction()
    {
        isChasing = true;
        yield return new WaitForSeconds(1f);
        isChasing = false;
        StartCoroutine(ChaseAction());
    }

    IEnumerator CatchAction()
    {
        if (isConcious) { 
        isChasing = false;
        rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        //Play Catch Animation
        var direction = (player.transform.position - transform.position).normalized;
        var catchNet = Instantiate(net, transform.position+direction, Quaternion.identity);
        catchNet.GetComponent<Net>().direction = direction;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        catchNet.GetComponent<Net>().rotation = Quaternion.Euler(new Vector3(0,0,90 - angle));
        isCatching = true;
        yield return new WaitForSeconds(1f);
        isCatching = false;
        StartCoroutine(CatchAction());
        }
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
