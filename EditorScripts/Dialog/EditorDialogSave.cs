using UnityEngine;
using UnityEngine.UI;

public class EditorDialogSave : EditorDialog
{
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Button button;

    [SerializeField]
    private Button closeButton;

    void Start()
    {
        button.onClick.AddListener(Button_OnClick);
        closeButton.onClick.AddListener(CloseButton_OnClick);
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
    }

    private void Button_OnClick()
    {
        #if UNITY_ANDROID
            editor.SaveLevel(Application.persistentDataPath + "/" + inputField.text + ".txt");
        #else
            editor.SaveLevel(Application.dataPath + "/" + inputField.text + ".txt");
        #endif
        editor.HideDialog();
    }
    private void CloseButton_OnClick()
    {
        editor.HideDialog();
    }
}