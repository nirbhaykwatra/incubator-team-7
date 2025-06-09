using System;
using System.Collections.Generic;
using System.IO;
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
        wnd.titleContent = new GUIContent("GymTool");
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
        selectionGroup.text = "Remove Gyms";
        selectionGroup.style.paddingTop = 10f;
        selectionGroup.style.paddingBottom = 10f;
        selectionGroup.style.paddingLeft = 10f;
        selectionGroup.style.paddingRight = 10f;
        selectionGroup.style.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
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
        listView.style.backgroundColor = new Color(25f, 25f, 25f, 0.2f);
        selectionGroup.Add(listView);
        
        Label selectionLabel = new Label();
        selectionLabel.name = "selectionLabel";
        selectionLabel.text = "Selected Gym: ";
        selectionLabel.style.marginTop = 10f;
        selectionLabel.style.marginBottom = 10f;
        selectionGroup.Add(selectionLabel);
        
        Button removeGymButton = new Button();
        removeGymButton.name = "removeGymButton";
        removeGymButton.text = "Remove Gym";
        selectionGroup.Add(removeGymButton);
        
        GroupBox createGroup = new GroupBox();
        createGroup.name = "createGroup";
        createGroup.text = "Create Gym";
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
        createGroup.Add(gymName);

        Button createGymButton = new Button();
        createGymButton.name = "createGymButton";
        createGymButton.text = "Create Gym";
        createGroup.Add(createGymButton);
        
        listView.selectedIndicesChanged += (selectedIndices) =>
        {
            selectionLabel.text = $"Selected Gym:  {listView.selectedItem}";
            
            selectedScene = listView.selectedItem.ToString();

            // Note: selectedIndices can also be used to get the selected items from the itemsSource directly or
            // by using listView.viewController.GetItemForIndex(index).
        };
        
        root.Add(selectionGroup);
        root.Add(createGroup);
        
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
        }
    }

    private void CreateGym(ClickEvent clickEvent)
    {
        VisualElement root = rootVisualElement;
        TextField gymName = root.Q<TextField>("gymName");
        if (gymName.value == "") return;
        Scene newGym = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        Directory.CreateDirectory($"Assets/Internment/Scenes/Gyms/{gymName.value}");
        EditorSceneManager.SaveScene(newGym, $"Assets/Internment/Scenes/Gyms/{gymName.value}/{gymName.value}.unity");
        Debug.Log($"Created Gym: {gymName.value}");
        
        AssetDatabase.Refresh();
        gymName.value = "";
        ListView listView = root.Q<ListView>("gymList");
        listView.itemsSource = GetGymNames();
        listView.Rebuild();

        EditorSceneManager.OpenScene($"Assets/Internment/Scenes/Gyms/{gymName.value}/{gymName.value}.unity");
    }
    
    private void RemoveGym(ClickEvent clickEvent)
    {
        VisualElement root = rootVisualElement;
        if (selectedScene == "") return;
        if (!Directory.Exists($"Assets/Internment/Scenes/Gyms/{selectedScene}")) return;
        Directory.Delete($"Assets/Internment/Scenes/Gyms/{selectedScene}", true);
        File.Delete($"Assets/Internment/Scenes/Gyms/{selectedScene}.meta");
        Debug.Log($"Deleted Gym: {selectedScene}");
        
        AssetDatabase.Refresh();
        ListView listView = root.Q<ListView>("gymList");
        listView.itemsSource = GetGymNames();
        listView.Rebuild();
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
}
