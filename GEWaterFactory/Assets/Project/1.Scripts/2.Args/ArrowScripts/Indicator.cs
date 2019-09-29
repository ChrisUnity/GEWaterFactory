using UnityEngine;

/// <summary>
/// Attach this script to the box and arrow indicator prefabs.
/// </summary>
public class Indicator : MonoBehaviour
{
    private MeshRenderer indicatorRenderer;
    private Material indicatorMaterial;
    private Quaternion defaultRotation;

    /// <summary>
    /// Gets and sets the indicator's default rotation.
    /// </summary>
    public Quaternion DefaultRotation
    {
        get
        {
            return defaultRotation;
        }
        set
        {
            if (defaultRotation != value)
            {
                defaultRotation = value;
            }
        }
    }

    /// <summary>
    /// Gets if the indicator is currently active in hierarchy.
    /// </summary>
    public bool Active
    {
        get
        {
            return transform.gameObject.activeInHierarchy;
        }
    }

    void Awake()
    {
        indicatorRenderer = transform.GetComponent<MeshRenderer>();
        if (indicatorRenderer == null)
        {
            indicatorRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        }
        indicatorMaterial = indicatorRenderer.material;
        foreach (Collider collider in transform.gameObject.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in transform.gameObject.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }
    }

    /// <summary>
    /// Sets the color for the indicator.
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color)
    {
        //indicatorMaterial.color = color;
        //indicatorMaterial.SetColor("_TintColor", color);
    }

    /// <summary>
    /// Sets the active status of the indicator in the hierarchy.
    /// </summary>
    /// <param name="value"></param>
    public void Activate(bool value)
    {
        transform.gameObject.SetActive(value);
    }
}
