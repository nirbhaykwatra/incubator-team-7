using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SceneHandler = UIComponents.SceneHandler;

public class PopulateGymMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonPrefab;
    
    [SerializeField]
    private GymList _gymList;

    private void Awake()
    {
        foreach (string gym in _gymList.gymList)
        {
            GameObject button = Instantiate(_buttonPrefab, transform);
            button.name = gym;
            Button childButtonComponent = button.GetComponentInChildren<Button>();
            GameObject childButton = childButtonComponent.gameObject;
            SceneHandler buttonSceneHandler = childButton.AddComponent<SceneHandler>();
            
            TextMeshProUGUI childButtonText = childButton.GetComponentInChildren<TextMeshProUGUI>();
            childButtonText.text = gym;
            
            childButtonComponent.onClick.AddListener(() => buttonSceneHandler.LoadScene(gym));

        }
    }
}
