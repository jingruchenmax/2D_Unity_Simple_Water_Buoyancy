using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Buoyancy : MonoBehaviour
{
    [SerializeField, Tooltip("Increase value to make object more buoyant, default 8.")]
    float buoyantForce = 8f;
    [SerializeField, Tooltip("Value 0 means no additional Buoyant Force underwater, 1 means Double buoyant Force underwater (underwater pressure)"),
    Range(0f, 1f)]
    float depthPower = 1f;
    [SerializeField, Tooltip("Center of Mass on Y axis (kind of), default 0.")]
    float offsetY = 0f;
    [SerializeField, Tooltip("Tag of the Water Body")]
    string waterVolumeTag = "Water";
    [SerializeField, Tooltip("Force to apply when the object is pushed down")]
    float pushForce = 10f;
    [SerializeField, Tooltip("Friction to apply when the object is in the air")]
    float airFriction = 0.1f;
    [SerializeField, Tooltip("Friction to apply when the object is underwater")]
    float waterFriction = 1f;

    private Rigidbody2D rb;
    private Collider2D coll;
    private WaterBody waterBody;
    private float yBound;
    private bool isWaterBodySet;
    private int waterCount;
    private TouchOrClickHandler touchOrClickHandler;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        touchOrClickHandler = FindObjectOfType<TouchOrClickHandler>();
    }

    private void Update()
    {
        if (waterCount == 0)
        {
            waterBody = null;
            isWaterBodySet = false;
        }

        if (touchOrClickHandler != null)
        {
            ApplyPushForce(touchOrClickHandler.isPressed, touchOrClickHandler.pressedTime);
        }
    }

    private void ApplyPushForce(bool isPressed, float pressedTime)
    {
        if (isPressed)
        {
            rb.AddForce(new Vector2(0f, -pushForce * pressedTime), ForceMode2D.Force);
        }
    }

    private void FixedUpdate()
    {
        if (isWaterBodySet)
        {
            float objectYValue = coll.bounds.center.y + offsetY;
            yBound = waterBody.GetYBound();
            if (objectYValue < yBound)
            {
                float buoyantForceMass = buoyantForce * rb.mass;
                float underWaterBuoyantForce = Mathf.Clamp01((yBound - objectYValue) * depthPower);
                float buoyancy = buoyantForceMass + (buoyantForceMass * underWaterBuoyantForce);
                rb.AddForce(new Vector2(0f, buoyancy));

                ApplyFriction(waterFriction);

                Debug.Log("Applying buoyancy force: " + buoyancy);
            }
            else
            {
                ApplyFriction(airFriction);
            }
        }
    }

    private void ApplyFriction(float friction)
    {
        Vector2 velocity = rb.velocity;
        velocity.x *= (1 - friction * Time.fixedDeltaTime);
        velocity.y *= (1 - friction * Time.fixedDeltaTime);
        rb.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D water)
    {
        if (water.CompareTag(waterVolumeTag)) waterCount++;
    }

    private void OnTriggerStay2D(Collider2D water)
    {
        if (water.CompareTag(waterVolumeTag))
        {
            if (transform.position.x < water.bounds.max.x
            && transform.position.x > water.bounds.min.x)
            {
                if (waterBody != null && !ReferenceEquals(waterBody.gameObject, water.gameObject))
                {
                    waterBody = null;
                    isWaterBodySet = false;
                }

                if (!isWaterBodySet)
                {
                    waterBody = water.GetComponent<WaterBody>();
                    if (waterBody != null) isWaterBodySet = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D water)
    {
        if (water.CompareTag(waterVolumeTag)) waterCount--;
    }
}
