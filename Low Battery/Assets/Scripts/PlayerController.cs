using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform handpivot;
    Rigidbody2D rigidBody;
    Vector3 mouseDirection;
    [SerializeField] private Transform aimReticle;
    [SerializeField] private Transform wire;
    [SerializeField] private float batteryLife;
    [SerializeField] private float grableDistance;
    [SerializeField] private float grableStrength;
    private bool canMove = true;
    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private LayerMask GrableMask;
    [SerializeField] private int health = 5;
    private bool isAirborn = false;
    private bool isCharging = false;
    public int memoryStickAmount = 0;

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
            if (!isCharging)
            {
                batteryLife -= Time.deltaTime;
                batteryText.text = batteryLife.ToString("f0") + " %";
            }
        }
        else { canMove = false; StartCoroutine(Death()); }
    }

    private void FixedUpdate()
    {
        if (!canMove) { return; }
        RaycastHit2D rayHit = Physics2D.Raycast(handpivot.position, mouseDirection.normalized, grableDistance, GrableMask);
        if (rayHit.collider != null) { 
            aimReticle.transform.position = rayHit.point;

            if (Input.GetMouseButton(0))
            {
                wire.transform.position = Vector3.Lerp(handpivot.position, aimReticle.position, 0.5f);
                float distance = Vector3.Distance(handpivot.position, aimReticle.position);
                wire.transform.localScale = new Vector3(distance, 0.2f, 0);
                wire.transform.rotation = handpivot.rotation;
                grableStrength += Time.deltaTime * 3;
                aimReticle.transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
                if (grableStrength > 20)
                {
                    grableStrength = 20;
                    aimReticle.transform.localScale = new Vector3(4, 4, 4);
                }
            }
        }
        else { aimReticle.transform.position = new Vector2(100, 100);
            wire.transform.position = new Vector3(1000, 0, 0);
            grableStrength = 10;
            wire.transform.localScale = new Vector3(0, 0, 0);
            aimReticle.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
    }

    private void LateUpdate()
    {
        if (!canMove) { return; }
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + new Vector3(0, 0, -10), 3 * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAirborn)
        {
            if (collision.gameObject.GetComponent<Enemy>())
            {
                KnockOutEnemy(collision.gameObject);
            }
        }

        if (collision.gameObject.layer == GrableMask)
        {
            isAirborn = false;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            if (rigidBody.velocity == Vector2.zero) { Damage(); ReturnToStart(); }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isAirborn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MemoryStick")
        {
            memoryStickAmount += 1;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Spikes")
        {
            Damage();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Battery")
        {
            isCharging = true;
            batteryLife += 0.1f;
            batteryText.text = batteryLife.ToString("f0") + " %";
            if (batteryLife > 100)
            {
                batteryLife = 100;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Battery")
        {
            isCharging = false;
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
        isAirborn = true;
    }

    private void Damage()
    {
        if (!isAirborn)
        {
            health -= 1;
            healthText.text = health.ToString();
            if (health <= 0)
            {
                health = 0;
                StartCoroutine(Death());
                canMove = false;

            }
        }
    }

    private void KnockOutEnemy(GameObject enemy)
    {
        print("knocking out enemy");
        enemy.GetComponent<Enemy>().Knockout();
    }

    private void ReturnToStart()
    {
        transform.position = new Vector3(0.23f, -2.95f, 0);
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        //Play Death Animation
        SceneManager.LoadScene(3);
    }
}
