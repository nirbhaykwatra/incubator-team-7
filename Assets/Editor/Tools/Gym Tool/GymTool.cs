using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GymTool : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private string selectedScene;

    [MenuItem("Tools/Gym Tool")]
    public static void ShowGymToolWindow()
    {
        GymTool wnd = GetWindow<GymTool>();
        wnd.minSize = new Vector2(300, 300);
        wnd.titleContent = new GUIContent("Gym Tool");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        root.style.paddingTop = 10f;
        root.style.paddingBottom = 10f;
        root.style.paddingLeft = 10f;
        root.style.paddingRight = 10f;
        
        GroupBox selectionGroup = new GroupBox();
        selectionGroup.name = "selectionGroup";
        selectionGroup.text = "Existing Gyms";
        selectionGroup.style.paddingTop = 10f;
        selectionGroup.style.paddingBottom = 10f;
        selectionGroup.style.paddingLeft = 10f;
        selectionGroup.style.paddingRight = 10f;
        selectionGroup.style.borderTopWidth = 1f;
        selectionGroup.style.borderBottomWidth = 1f;
        selectionGroup.style.borderLeftWidth = 1f;
        selectionGroup.style.borderRightWidth = 1f;
        selectionGroup.style.borderTopColor = new Color(0f, 0f, 0f, 0.5f);
        selectionGroup.style.borderBottomColor = new Color(0f, 0f, 0f, 0.5f);
        selectionGroup.style.borderLeftColor = new Color(0f, 0f, 0f, 0.5f);
        selectionGroup.style.borderRightColor = new Color(0f, 0f, 0f, 0.5f);

        // VisualElements objects can contain other VisualElement following a tree hierarchy.

        Func<VisualElement> makeItem = () => new Label();
        
        Action<VisualElement, int> bindItem = (e, i) => ((Label)e).text = GetGymNames()[i];
        
        ListView listView = new ListView();
        listView.name = "gymList";
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemsSource = GetGymNames();
        listView.selectionType = SelectionType.Single;
        listView.style.paddingTop = 10f;
        listView.style.paddingBottom = 10f;
        listView.style.paddingLeft = 10f;
        listView.style.paddingRight = 10f;
        listView.style.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
        selectionGroup.Add(listView);
        
        Label selectionLabel = new Label();
        selectionLabel.name = "selectionLabel";
        selectionLabel.text = "Selected Gym: ";
        selectionLabel.style.marginTop = 10f;
        selectionLabel.style.marginBottom = 10f;
        selectionLabel.visible = false;
        selectionGroup.Add(selectionLabel);
        
        Button openGymButton = new Button();
        openGymButton.name = "openGymButton";
        openGymButton.text = "Open Gym";
        selectionLabel.style.marginTop = 10f;
        selectionLabel.style.marginBottom = 10f;
        selectionGroup.Add(openGymButton);
        
        Button removeGymButton = new Button();
        removeGymButton.name = "removeGymButton";
        removeGymButton.text = "Remove Gym";
        selectionLabel.style.marginBottom = 10f;
        selectionGroup.Add(removeGymButton);
        
        GroupBox createGroup = new GroupBox();
        createGroup.name = "createGroup";
        createGroup.text = "Create Gym";
        createGroup.style.paddingTop = 10f;
        createGroup.style.paddingBottom = 10f;
        createGroup.style.paddingLeft = 10f;
        createGroup.style.paddingRight = 10f;
        createGroup.style.borderTopWidth = 1f;
        createGroup.style.borderBottomWidth = 1f;
        createGroup.style.borderLeftWidth = 1f;
        createGroup.style.borderRightWidth = 1f;
        createGroup.style.borderTopColor = new Color(0f, 0f, 0f, 0.5f);
        createGroup.style.borderBottomColor = new Color(0f, 0f, 0f, 0.5f);
        createGroup.style.borderLeftColor = new Color(0f, 0f, 0f, 0.5f);
        createGroup.style.borderRightColor = new Color(0f, 0f, 0f, 0.5f);
        
        TextField gymName = new TextField();
        gymName.name = "gymName";
        gymName.value = "";
        gymName.label = "Name";
        gymName.style.marginBottom = 2f;
        createGroup.Add(gymName);
        
        Button createGymButton = new Button();
        createGymButton.name = "createGymButton";
        createGymButton.text = "Create Gym";
        createGroup.Add(createGymButton);
        
        listView.selectedIndicesChanged += (selectedIndices) =>
        {
            if (listView.selectedItem != null)
            {
                selectionLabel.text = $"Selected Gym:  {listView.selectedItem}";
                selectedScene = listView.selectedItem.ToString();
            }
            else
            {
                selectionLabel.text = "Selected Gym: ";
                selectedScene = "";
            }
            

            // Note: selectedIndices can also be used to get the selected items from the itemsSource directly or
            // by using listView.viewController.GetItemForIndex(index).
        };
        
        root.Add(createGroup);
        root.Add(selectionGroup);
        
        SetupButtonHandler();
    }

    private void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var buttons = root.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button button)
    {
        switch (button.name)
        {
            case "removeGymButton":
                button.RegisterCallback<ClickEvent>(RemoveGym);
                break;
            case "createGymButton":
                button.RegisterCallback<ClickEvent>(CreateGym);
                break;
            case "openGymButton":
                button.RegisterCallback<ClickEvent>(OpenGym);
                break;
        }
    }

    private void CreateGym(ClickEvent clickEvent)
    {
        VisualElement root = rootVisualElement;
        TextField gymName = root.Q<TextField>("gymName");
        if (gymName.value == "") return;

        if (AssetDatabase.AssetPathExists($"Assets/Internment/Scenes/Gyms/{gymName.value}/{gymName.value}.unity"))
        {
            EditorUtility.DisplayDialog("Error", "Gym already exists!", "OK");
            return;
        }
        
        Scene activeScene = SceneManager.GetActiveScene();
        
        SceneTemplateAsset neutralLightSetup = (SceneTemplateAsset)AssetDatabase.LoadAssetAtPath("Assets/Settings/SceneTemplates/NeutralLightSetup.scenetemplate", typeof(SceneTemplateAsset));
        Directory.CreateDirectory($"Assets/Internment/Scenes/Gyms/{gymName.value}");
        InstantiationResult newScene = SceneTemplateService.Instantiate(neutralLightSetup, false, $"Assets/Internment/Scenes/Gyms/{gymName.value}/{gymName.value}.unity");
        AddSceneToEditorBuildSettings($"Assets/Internment/Scenes/Gyms/{gymName.value}/{gymName.value}.unity");
        EditorSceneManager.CloseScene(activeScene, true);
        Debug.Log($"Created Gym: {gymName.value}");
        
        AssetDatabase.Refresh();
        gymName.value = "";
        ListView listView = root.Q<ListView>("gymList");
        listView.itemsSource = GetGymNames();
        listView.Rebuild();
        
    }
    
    private void RemoveGym(ClickEvent clickEvent)
    {
        if (!EditorUtility.DisplayDialog("Remove Gym", $"Are you sure you want to delete '{selectedScene}?' THIS IS AN IRREVERSABLE PROCESS.", "Yes", "No")) return;
        VisualElement root = rootVisualElement;
        if (selectedScene == "")
        {
            EditorUtility.DisplayDialog("Error", "No scene has been selected!", "OK");
            return;
        }
        if (!Directory.Exists($"Assets/Internment/Scenes/Gyms/{selectedScene}"))
        {
            EditorUtility.DisplayDialog("Error", $"The selected scene '{selectedScene}' does not exist.", "OK");
            return;
        }
        RemoveSceneFromEditorBuildSettings($"Assets/Internment/Scenes/Gyms/{selectedScene}/{selectedScene}.unity");
        Directory.Delete($"Assets/Internment/Scenes/Gyms/{selectedScene}", true);
        File.Delete($"Assets/Internment/Scenes/Gyms/{selectedScene}.meta");
        Debug.Log($"Deleted Gym: {selectedScene}");
        
        AssetDatabase.Refresh();
        ListView listView = root.Q<ListView>("gymList");
        listView.ClearSelection();
        selectedScene = "";
        listView.itemsSource = GetGymNames();
        listView.Rebuild();
    }

    private void OpenGym(ClickEvent clickEvent)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneAsset sceneToOpen = (SceneAsset)AssetDatabase.LoadAssetAtPath($"Assets/Internment/Scenes/Gyms/{selectedScene}/{selectedScene}.unity", typeof(SceneAsset));

        if (activeScene.name == sceneToOpen.name)
        {
            EditorUtility.DisplayDialog("Error", "This gym is already open!", "OK");
            return;
        }
        
        if (!EditorUtility.DisplayDialog("Open Gym", $"Are you sure you want to open '{selectedScene}?'", "Yes", "No")) return;
        
        VisualElement root = rootVisualElement;
        if (selectedScene == "")
        {
            EditorUtility.DisplayDialog("Error", "No gym has been selected!", "OK");
            return;
        }
        if (!Directory.Exists($"Assets/Internment/Scenes/Gyms/{selectedScene}"))
        {
            EditorUtility.DisplayDialog("Error", $"The selected scene '{selectedScene}' does not exist.", "OK");
            return;
        }

        if (EditorUtility.DisplayDialog("Open Gym", $"Do you want to save the current scene?", "Yes", "No"))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.SaveScene(activeScene);
                Debug.Log($"Saved scene {activeScene.name}");
            }
            EditorSceneManager.OpenScene($"Assets/Internment/Scenes/Gyms/{selectedScene}/{selectedScene}.unity");
        }
        else
        {
            EditorSceneManager.OpenScene($"Assets/Internment/Scenes/Gyms/{selectedScene}/{selectedScene}.unity");
            EditorSceneManager.CloseScene(activeScene, true);
        }
    }

    private List<string> GetGymNames()
    {
        List<string> m_GymNames;
        m_GymNames = new List<string>();
        
        string[] guids = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Internment/Scenes/Gyms" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string assetName = Path.GetFileNameWithoutExtension(path);
            m_GymNames.Add(assetName);
        }
        
        return m_GymNames;
    }
    
    private void AddSceneToEditorBuildSettings(string scenePath)
    {
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();
        EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(scenePath, true);
        editorBuildSettingsScenes.Add(buildScene);

        // Set the active platform or build profile scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        AssetDatabase.SaveAssets();
    }
    
    private void RemoveSceneFromEditorBuildSettings(string scenePath)
    {
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();

        for (int i = 0; i < editorBuildSettingsScenes.Count; i++)
        {
            if (editorBuildSettingsScenes[i].path == scenePath)
            {
                editorBuildSettingsScenes.RemoveAt(i);
            }
        }
        
        // Set the active platform or build profile scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        AssetDatabase.SaveAssets();
    }
}
