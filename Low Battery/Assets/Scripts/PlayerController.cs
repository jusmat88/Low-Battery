using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform handpivot;
    Rigidbody2D rigidBody;
    Vector3 mouseDirection;
    [SerializeField] private Transform aimReticle;
    [SerializeField] private float batteryLife;
    [SerializeField] private float grableDistance;
    [SerializeField] private float grableStrength;
    private bool canMove = true;
    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private LayerMask GrableMask;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int health = 5;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(handpivot.position, grableDistance);
    }

    private void Start() => Initialize();

    private void Update()
    {
        if (!canMove) { return; }
        AimHand();
        if (Input.GetMouseButtonUp(0)) { ShootGrabler(); }

        if (batteryLife > 0)
        {
            batteryLife -= Time.deltaTime;
            batteryText.text = batteryLife.ToString("f0") + " %";
        }
        else { canMove = false; }
    }

    private void FixedUpdate()
    {
        if (!canMove) { return; }
        RaycastHit2D rayHit = Physics2D.Raycast(handpivot.position, mouseDirection.normalized, grableDistance, GrableMask);
        if (rayHit.collider != null) { aimReticle.transform.position = rayHit.point; }
        else { aimReticle.transform.position = new Vector2(100, 100); }
    }

    private void LateUpdate()
    {
        if (!canMove) { return; }
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + new Vector3(0, 2, -10), 3 * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == enemyLayer)
        {
            print("Hit enemy");
            if (rigidBody.velocity == Vector2.zero) { ReturnToStart(); }
            else { KnockOutEnemy(collision.gameObject); }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MemoryStick>())
        {
            //Do stuff
        }
    }

    private void Initialize()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        healthText.text = health.ToString();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void AimHand()
    {
        Vector3 mousePos = Input.mousePosition;
        mouseDirection = mousePos - handpivot.position;
        mouseDirection.x = mouseDirection.x - Screen.width * 0.5f;
        mouseDirection.y = mouseDirection.y - Screen.height * 0.5f;
        float angle = Mathf.Atan2(mouseDirection.x, mouseDirection.y) * Mathf.Rad2Deg;
        handpivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 - angle));
    }

    private void ShootGrabler()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(handpivot.position, mouseDirection.normalized, grableDistance, GrableMask);
        if (rayHit.collider != null) { Grable(); }
    }

    private void Grable()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(mouseDirection.normalized * grableStrength, ForceMode2D.Impulse);
    }

    private void Damage()
    {
        health -= 1;
        healthText.text = health.ToString();
        if (health >= 0)
        {
            //Play Death Animation
            canMove = false;
        }
    }

    private void ReturnToStart()
    {
        //Play Animation
        Damage();
    }

    private void KnockOutEnemy(GameObject enemy)
    {
        print("knocking out enemy");
        enemy.GetComponent<Enemy>().Knockout();
    }
}
