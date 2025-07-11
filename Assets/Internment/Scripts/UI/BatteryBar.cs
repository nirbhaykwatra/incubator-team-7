using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BatteryBar : MonoBehaviour
{
    [Tooltip("Reference to your Battery component")]
    [SerializeField] private Battery _battery;
    [SerializeField] private Image _fillImage;

    void Awake()
    {
        _fillImage = GetComponent<Image>();

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            _battery = player.GetComponent<Battery>();

        if (_battery == null)
            Debug.LogError("BatteryBar: no Player Battery found!");
    }

    void Update()
    {
        if (_battery == null)
            return;

        //Debug.Log($"Battery filled: {_battery.CurrentLevel} / {_battery.Capacity}");
        float t = Mathf.Clamp01(_battery.CurrentLevel / _battery.Capacity);
        _fillImage.color = Color.Lerp(Color.gray, Color.green, Mathf.Pow(_battery.CurrentLevel / _battery.Capacity, 2));
        _fillImage.fillAmount = t;
    }
}
