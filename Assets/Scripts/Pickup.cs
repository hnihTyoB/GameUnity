using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private enum PickUpType
    {
        GoldCoin,
        StaminaGlobe,
        HealthGlobe,
        LightFragment,
        Battery,
        CourageFragment,
        ShieldFragment,
    }
    [SerializeField] private PickUpType pickUpType;
    [SerializeField] private float pickUpDistance = 5f;
    [SerializeField] private float accelartionRate = .2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;
    private Vector3 moveDir;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        StartCoroutine(AnimCurveSpawnRoutine());
    }

    private void Update()
    {
        Vector3 playerPos = PlayerController.Instance.transform.position;

        if (Vector3.Distance(transform.position, playerPos) < pickUpDistance)
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += accelartionRate;
        }
        else
        {
            moveDir = Vector3.zero;
            moveSpeed = 0;
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir * moveSpeed * Time.deltaTime;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            DetectPickupType();
            Destroy(gameObject);
        }
    }
    private IEnumerator AnimCurveSpawnRoutine()
    {
        Vector2 startPoint = transform.position;
        float randomX = transform.position.x + Random.Range(-2f, 2f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);

        Vector2 endPoint = new Vector2(randomX, randomY);

        float timePassed = 0f;

        while (timePassed < popDuration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);
            yield return null;
        }
    }
    private void DetectPickupType() {
        switch (pickUpType)
        {
            case PickUpType.GoldCoin:
                EconomyManager.Instance.UpdateCurrentGold();
                break;
            case PickUpType.HealthGlobe:
                PlayerHealth.Instance.HealPlayer();           
                break;
            case PickUpType.StaminaGlobe:
                Stamina.Instance.RefreshStamina();
                break;
            case PickUpType.LightFragment:
                // Restore stamina (represents mental energy)
                Stamina.Instance.RefreshStamina();
                // Could also give bonus effect in future
                break;
            case PickUpType.Battery:
                // Add battery to BatteryManager
                if (BatteryManager.Instance != null)
                {
                    BatteryManager.Instance.AddBattery(1f);
                    Debug.Log("Battery picked up (via Pickup.cs) - added to BatteryManager");
                }
                else
                {
                    Debug.LogWarning("BatteryManager not found! Battery pickup failed.");
                    // Fallback to stamina
                    Stamina.Instance.RefreshStamina();
                }
                break;
            case PickUpType.CourageFragment:
                // Give courage to face bullying
                Stamina.Instance.RefreshStamina(); // Restore mental energy
                // TODO: Could add temporary courage buff
                break;
            case PickUpType.ShieldFragment:
                // Give protection against bullying
                PlayerHealth.Instance.HealPlayer(); // Restore health
                // TODO: Could add temporary shield buff
                break;
        }
    }
}
