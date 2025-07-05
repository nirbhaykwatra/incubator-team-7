using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RadarCooldownBar : MonoBehaviour
{
    [Tooltip("Reference to CharacterRadar")]
    [SerializeField] private CharacterRadar _radar;
    [SerializeField] private Image _fillImage;

    void Awake()
    {
        _fillImage = GetComponent<Image>();
        if (_radar == null)
            _radar = Object.FindFirstObjectByType<CharacterRadar>();
        if (_radar == null)
            Debug.LogError("RadarCooldownBar: no CharacterRadar found!");
    }

    void Update()
    {
        // get normalized cooldown [0бн1]
        float norm = Mathf.Clamp01(
            _radar.LongRadarCooldownTimer /
            _radar.LongRadarCooldownDuration
        );
        _fillImage.fillAmount = 1f - norm;
    }
}