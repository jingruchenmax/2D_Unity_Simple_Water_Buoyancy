using UnityEngine;

public class WaterBody : MonoBehaviour
{
    [SerializeField, Tooltip("Check this if you want to set Surface level of the water by yourself")]
    bool customSurfaceLevel = false;
    [SerializeField, Tooltip("Surface level of waterbody in Y axis")]
    float surfaceLevel = 0f;

    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    //Get Surface level of the water
    public float GetYBound()
    {
        if (!customSurfaceLevel) surfaceLevel = coll.bounds.max.y;
        return surfaceLevel;
    }
}
