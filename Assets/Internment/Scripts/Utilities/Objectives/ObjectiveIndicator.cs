using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicator : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image objectiveDot;
    [SerializeField] private float dotSize = 20f;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float pulseSpeed = 2f;

    [Header("Distance Settings")]
    [SerializeField] private float maxDisplayDistance = 500f; // Increased default
    [SerializeField] private float radarRadius = 100f; // Radius of radar display in pixels
    [SerializeField] private bool alwaysShowObjective = true; // New option

    [Header("Colors")]
    [SerializeField] private Color nearColor = Color.yellow;
    [SerializeField] private Color farColor = Color.red;
    [SerializeField] private Color offScreenColor = Color.white;

    private Transform playerTransform;
    private Camera radarCamera;
    private RectTransform radarRect;
    private ObjectiveManager objectiveManager;

    private void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Find radar camera
        radarCamera = GameObject.Find("RadarCamera")?.GetComponent<Camera>();
        if (radarCamera == null)
        {
            // Try to find by tag or component
            RadarCamera radarCamComponent = FindFirstObjectByType<RadarCamera>();
            if (radarCamComponent != null)
            {
                radarCamera = radarCamComponent.GetComponent<Camera>();
            }
        }

        // Get radar rect transform
        radarRect = GetComponentInParent<RectTransform>();

        // Get objective manager
        objectiveManager = ObjectiveManager.Instance;

        // Subscribe to objective changes
        if (objectiveManager != null)
        {
            objectiveManager.OnObjectiveChanged.AddListener(OnObjectiveChanged);
            objectiveManager.OnAllObjectivesCompleted.AddListener(OnAllObjectivesCompleted);
        }

        // Initially hide if no objective
        if (objectiveDot != null)
            objectiveDot.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerTransform == null || radarCamera == null || objectiveManager == null)
            return;

        GameObject targetObject = objectiveManager.GetCurrentTargetObject();
        if (targetObject == null)
        {
            if (objectiveDot != null)
                objectiveDot.gameObject.SetActive(false);
            return;
        }

        // Make sure dot is visible
        if (objectiveDot != null && !objectiveDot.gameObject.activeSelf)
            objectiveDot.gameObject.SetActive(true);

        UpdateObjectiveIndicator(targetObject.transform);
    }

    private void UpdateObjectiveIndicator(Transform target)
    {
        // Calculate direction from player to target
        Vector3 directionToTarget = target.position - playerTransform.position;
        float distanceToTarget = directionToTarget.magnitude;

        // Project onto horizontal plane for radar calculation
        Vector3 flatDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);
        float horizontalDistance = flatDirection.magnitude;
        flatDirection.Normalize();

        // Convert world position to radar position
        Vector3 targetWorldPos = target.position;
        Vector3 viewportPos = radarCamera.WorldToViewportPoint(targetWorldPos);

        // Check if target is within radar view
        bool isInRadarView = viewportPos.x >= 0 && viewportPos.x <= 1 &&
                            viewportPos.y >= 0 && viewportPos.y <= 1 &&
                            viewportPos.z > 0; // Make sure it's in front of camera

        // Remove distance check - always show if in radar view
        if (isInRadarView)
        {
            // Target is visible on radar - position the dot at target location
            Vector2 radarPos = new Vector2(
                (viewportPos.x - 0.5f) * radarRect.rect.width,
                (viewportPos.y - 0.5f) * radarRect.rect.height
            );

            objectiveDot.rectTransform.anchoredPosition = radarPos;

            // Color based on distance
            if (!alwaysShowObjective && distanceToTarget > maxDisplayDistance)
            {
                // Hide if too far and not always showing
                objectiveDot.gameObject.SetActive(false);
                return;
            }

            // Use on-screen color
            float colorT = Mathf.Clamp01(distanceToTarget / maxDisplayDistance);
            objectiveDot.color = Color.Lerp(nearColor, farColor, colorT);
        }
        else
        {
            // Target is outside radar view - position dot at edge

            // Calculate angle in world space
            float worldAngle = Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg;

            // Get player's Y rotation
            float playerYRotation = playerTransform.eulerAngles.y;

            // Calculate relative angle
            float relativeAngle = worldAngle - playerYRotation;

            // Convert to radians for position calculation
            float radAngle = relativeAngle * Mathf.Deg2Rad;

            // Position dot at edge of radar (circular edge)
            float edgeDistance = radarRadius * 0.9f; // Slightly inside the edge
            Vector2 edgePos = new Vector2(
                Mathf.Sin(radAngle) * edgeDistance,
                Mathf.Cos(radAngle) * edgeDistance
            );

            objectiveDot.rectTransform.anchoredPosition = edgePos;

            // Use off-screen color
            objectiveDot.color = offScreenColor;
        }

        // Add pulsing effect for visibility
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float currentScale = Mathf.Lerp(minScale, maxScale, pulse);
        objectiveDot.rectTransform.localScale = Vector3.one * currentScale;

        // Add alpha pulsing
        Color currentColor = objectiveDot.color;
        currentColor.a = Mathf.Lerp(0.6f, 1f, pulse);
        objectiveDot.color = currentColor;
    }

    private void OnObjectiveChanged(ObjectiveItem newObjective)
    {
        // Could update UI to show new objective info
        Debug.Log($"New objective: {newObjective?.itemName}");
    }

    private void OnAllObjectivesCompleted()
    {
        // Hide the indicator
        if (objectiveDot != null)
            objectiveDot.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (objectiveManager != null)
        {
            objectiveManager.OnObjectiveChanged.RemoveListener(OnObjectiveChanged);
            objectiveManager.OnAllObjectivesCompleted.RemoveListener(OnAllObjectivesCompleted);
        }
    }
}
