using UnityEngine;
using UnityEngine.UI;

public class EditorDialogMessage : EditorDialog
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private Button button;

    public Text Text
    {
        get
        {
            return text;
        }
    }

    void Start()
    {
        button.onClick.AddListener(Button_OnClick);
    }

    void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
    }

    private void Button_OnClick()
    {
        editor.HideDialog();
    }
}