using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRadarHandler : MonoBehaviour
{
    [SerializeField] private Image _normalBlip;
    [SerializeField] private Image _upBlip;
    [SerializeField] private Image _downBlip;
    [SerializeField] private float _fadeSpeed = 0.003f;
    
    private float lerpAmount;

    private void Awake()
    {
        _normalBlip.color = new Color(255, 255, 255, 0.0f);
        _upBlip.color = new Color(255, 255, 255, 0.0f);
        _downBlip.color = new Color(255, 255, 255, 0.0f);
    }

    public void PingResource()
    {
        _normalBlip.color = new Color(255, 255, 255, 1.0f);
        StartCoroutine(PingUI(_normalBlip));
    }

    public void PingResourceUp()
    {
        _upBlip.color = new Color(255, 255, 255, 1.0f);
        StartCoroutine(PingUI(_upBlip));
    }

    public void PingResourceDown()
    {
        _downBlip.color = new Color(255, 255, 255, 1.0f);
        StartCoroutine(PingUI(_downBlip));
    }

    IEnumerator PingUI(Image blip)
    {
        lerpAmount=1;

        while(lerpAmount>0)
        {
            Color color = blip.color;
            color.a = Mathf.Lerp(0,1,lerpAmount);
            blip.color = color;
            lerpAmount -= _fadeSpeed;
            yield return null;
        }
    }
}
