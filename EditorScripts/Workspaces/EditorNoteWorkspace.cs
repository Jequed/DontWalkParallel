using UnityEngine;
using UnityEngine.UI;

public class EditorNoteWorkspace : EditorView
{
    [SerializeField]
    private InputField textInputField;

    private EditorNote Note
    {
        get
        {
            return editor.LevelEditor.SelectedItem.GetComponent<EditorNote>();
        }
    }

    protected override void Start()
    {
        base.Start();
        
        textInputField.text = Note.Text;

        textInputField.onValueChanged.AddListener(TextInputField_OnValueChanged);
    }

    void OnDestroy()
    {
        textInputField.onValueChanged.RemoveListener(TextInputField_OnValueChanged);
    }

    private void TextInputField_OnValueChanged(string value)
    {
        Note.Text = value;
    }
}