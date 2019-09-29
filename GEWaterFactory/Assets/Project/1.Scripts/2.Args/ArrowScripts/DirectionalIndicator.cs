using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ArrowObjectPool))]
[RequireComponent(typeof(BoxObjectPool))]
public class DirectionalIndicator : MonoBehaviour
{

    /// <summary>
    /// The cursor gameobject that acts as the centre of the screen.
    /// </summary>
    public GameObject cursor;

    /// <summary>
    /// The distance of the cursor gameobject from the main camera.
    /// The size of the indicators changes based on this distance as well.
    /// </summary>
    [Range(2f, 20f)]
    public float cursorDistanceFromCamera = 20f;

    /// <summary>
    /// A value to set the inner boundary values from the view frustrum edges of the main camera.
    /// </summary>
    [Range(-0.3f, 0.3f)]
    public float targetSafeFactor = 0.1f;

    /// <summary>
    /// The distance of the arrow indicators from the cursor.
    /// </summary>
    [Range(0.3f, 5f)]
    public float arrowDistanceFromCursor = 2f;

    private Camera mainCamera;
    private string targetTag = "Target";

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        cursor.transform.SetParent(mainCamera.transform, true); // parent the cursor to the main camera.
        cursor.transform.localPosition = new Vector3(0, 0, cursorDistanceFromCamera); // set the initital postion of the cursor.
    }

    void Update()
    {
      //  DeactivateAllIndicators();
    }
    private Vector3 LastFrameCameraPos;

    private Vector3 CurrentFrameCameraPos;

    private float LastRotate;

    private float CurrentRotate;

    void LateUpdate()
    {
        DrawIndicators();
      
    }

    /// <summary>
    /// Draws all the indicators.
    /// </summary>
    private void DrawIndicators()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        List<Target> targets = new List<Target>();
        objects.ToList().ForEach(obj =>
        {
            Target target = obj.GetComponent<Target>();
            if (target) { targets.Add(target); }
        });

        foreach (Target target in targets)
        {
            Vector3 camToObjectDirection = target.transform.position - mainCamera.transform.position;
            camToObjectDirection.Normalize();

            if (target.NeedBoxIndicator && IsTargetVisible(target))
            {
                Indicator indicator = BoxObjectPool.current.GetPooledObject();
                indicator.SetColor(target.TargetColor);
                indicator.transform.position = mainCamera.transform.position + camToObjectDirection * cursorDistanceFromCamera;
                indicator.transform.LookAt(mainCamera.transform);
                indicator.Activate(true);
            }
            else if (target.NeedArrowIndicator && !target.Ishitme/*!IsTargetVisible(target)*/)
            {
                Indicator arrow = ArrowObjectPool.Instance.GetPooledObject();
                Quaternion defaultRotation = arrow.DefaultRotation;
                arrow.SetColor(target.TargetColor);
                Vector3 position;
                Quaternion rotation;
                GetArrowIndicatorPositionAndRotation(camToObjectDirection, out position, out rotation);
                arrow.transform.position = position;
                arrow.transform.rotation = rotation * defaultRotation;
                //arrow.Activate(true);
            }
        }
    }

    /// <summary>
    /// Deactivate all the indicators.
    /// </summary>
    private void DeactivateAllIndicators()
    {
        BoxObjectPool.current.DeactivateAllPooledObjects();
        ArrowObjectPool.Instance.DeactivateAllPooledObjects();
    }

    /// <summary>
    /// Return true if the target's mesh is within the Main Camera's view frustums.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsTargetVisible(Target target)
    {
        Vector3 targetViewportPosition = mainCamera.WorldToViewportPoint(target.transform.position);
        return (targetViewportPosition.z > 0 &&
            targetViewportPosition.x > targetSafeFactor &&
            targetViewportPosition.x < 1 - targetSafeFactor &&
            targetViewportPosition.y > targetSafeFactor &&
            targetViewportPosition.y < 1 - targetSafeFactor
            );
    }

    /// <summary>
    /// Gets the arrow indicator's position and rotation.
    /// </summary>
    /// <param name="camToObjectDirection"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    private void GetArrowIndicatorPositionAndRotation(Vector3 camToObjectDirection, out Vector3 position, out Quaternion rotation)
    {
        // Save the cursor transform position in a variable.
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward;

        // Project the camera to target direction onto the screen plane.
        Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * mainCamera.transform.forward);
        cursorIndicatorDirection.Normalize();

        // If the direction is 0, set the direction to the right.
        // This will only happen if the camera is facing directly away from the target.
        if (cursorIndicatorDirection == Vector3.zero)
        {
            cursorIndicatorDirection = mainCamera.transform.right;
        }

        // The final position is translated from the center of the screen along this direction vector.
        position = origin + cursorIndicatorDirection * arrowDistanceFromCursor;

        // Find the rotation from the facing direction to the target object.
        rotation = Quaternion.LookRotation(
            mainCamera.transform.forward,
            cursorIndicatorDirection);
    }
}
