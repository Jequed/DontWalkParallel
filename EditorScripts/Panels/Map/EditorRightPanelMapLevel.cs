using UnityEngine;
using UnityEngine.UI;

public class EditorRightPanelMapLevel : EditorView
{
    [SerializeField]
    private GameObject selectedItemSection;

    [SerializeField]
    private InputField nameInputField;

    [SerializeField]
    private InputField difficultyInputField;

    [SerializeField]
    private Button RemoveLevelButton;

    [SerializeField]
    private Button AddLevelButton;

    protected override void Start()
    {
        base.Start();

        selectedItemSection.SetActive(false);

        nameInputField.onEndEdit.AddListener(NameInputField_OnEndEdit);
        difficultyInputField.onEndEdit.AddListener(DifficultyInputField_OnEndEdit);
        RemoveLevelButton.onClick.AddListener(RemoveLevelButton_OnClick);

        AddLevelButton.onClick.AddListener(AddLevelButton_OnClick);

        editor.MapEditor.OnSelectionChanged += Editor_OnSelectionChanged;
    }

    void OnDestroy()
    {
        nameInputField.onEndEdit.RemoveListener(NameInputField_OnEndEdit);
        difficultyInputField.onEndEdit.RemoveListener(DifficultyInputField_OnEndEdit);
        RemoveLevelButton.onClick.RemoveListener(RemoveLevelButton_OnClick);

        AddLevelButton.onClick.RemoveListener(AddLevelButton_OnClick);

        if (editor != null)
            editor.MapEditor.OnSelectionChanged -= Editor_OnSelectionChanged;
    }

    private void Editor_OnSelectionChanged(DWPObject previuosItem, DWPObject newItem)
    {
        var mapLevel = newItem as MapLevel;

        selectedItemSection.SetActive(mapLevel != null);

        if (mapLevel != null)
        {
            nameInputField.text = mapLevel.LevelName;
            difficultyInputField.text = mapLevel.Difficulty.ToString();
        }
    }

    private void NameInputField_OnEndEdit(string value)
    {
        var mapLevel = editor.MapEditor.SelectedItem as MapLevel;

        if (mapLevel != null)
            mapLevel.LevelName = value;
    }
    private void DifficultyInputField_OnEndEdit(string value)
    {
        var mapLevel = editor.MapEditor.SelectedItem as MapLevel;

        if (mapLevel != null)
        {
            int difficulty = 0;
            if (int.TryParse(value, out difficulty))
            {
                if (difficulty < MapLevel.MinDifficulty)
                    difficulty = MapLevel.MinDifficulty;
                if (difficulty > MapLevel.MaxDifficulty)
                    difficulty = MapLevel.MaxDifficulty;
                
                difficultyInputField.text = difficulty.ToString();

                mapLevel.Difficulty = difficulty;
            }
        }
    }

    private void RemoveLevelButton_OnClick()
    {
        var level = editor.MapEditor.SelectedItem as MapLevel;

        if (level != null)
        {
            editor.MapContainer.Levels.Remove(level);
            GameObject.Destroy(level.gameObject);
        }
    }


    private void AddLevelButton_OnClick()
    {
        var level = editor.MapContainer.AddLevel();
        level.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, level.transform.position.z);
    }
}