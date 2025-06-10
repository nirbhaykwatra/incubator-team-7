using System.IO;
using TMPro;
using UIComponents;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopulateGymMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonPrefab;

    private void Awake()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Internment/Scenes/Gyms" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string assetName = Path.GetFileNameWithoutExtension(path);

            GameObject button = Instantiate(_buttonPrefab, transform);
            button.name = assetName;
            Button childButtonComponent = button.GetComponentInChildren<Button>();
            GameObject childButton = childButtonComponent.gameObject;
            SceneHandler buttonSceneHandler = childButton.AddComponent<SceneHandler>();
            
            TextMeshProUGUI childButtonText = childButton.GetComponentInChildren<TextMeshProUGUI>();
            childButtonText.text = assetName;
            
            childButtonComponent.onClick.AddListener(() => buttonSceneHandler.LoadScene(assetName));

        }
    }
}
