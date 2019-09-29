using UnityEngine;

/// <summary>
/// Attach this script to the targets in your scene.
/// </summary>
public class Target : MonoBehaviour
{
    [SerializeField] private Color targetColor;
    [SerializeField] private bool needBoxIndicator = true;
    [SerializeField] private bool needArrowIndicator = true;

    private string targetTag = "Target";


    private bool ishitme = false;

    /// <summary>
    /// Gets the color of indicator for the given target.
    /// </summary>
    public Color TargetColor
    {
        get
        {
            return targetColor;
        }
    }

    /// <summary>
    /// Gets if the box indicator is required for the given target.
    /// </summary>
    public bool NeedBoxIndicator
    {
        get
        {
            return needBoxIndicator;
        }
    }

    /// <summary>
    /// Gets if the arrow indicator is required for the given target.
    /// </summary>
    public bool NeedArrowIndicator
    {
        get
        {
            return needArrowIndicator;
        }
    }

    public bool Ishitme
    {
        get
        {
            return ishitme;
        }

        set
        {
            ishitme = value;
        }
    }

    private void Awake()
    {
        gameObject.tag = targetTag; //Set the tag of the target.
    }


    RaycastHit raycastResult;
    float distance = 20f;
    Ray ray;

    /// <summary>
    /// 射线检测是否看向自己(沙盘上的具体工厂)
    /// </summary>
    private void Update()
    {
        ray.origin = Camera.main.transform.position;
        ray.direction = Camera.main.transform.forward;
        if (Physics.Raycast(ray, out raycastResult))
        {
            if (raycastResult.collider.name.Equals(gameObject.name))
            {
                ishitme = true;
                ArrowObjectPool.Instance.DeactivateAllPooledObjects();
            }
            else
            {
                ishitme = false;
            }

        }
        else {
            ishitme = false;
        }
    }

    private void OnDestroy()
    {
        ishitme = false;
    }
    private void OnDisable()
    {
        ishitme = false;
    }

}
