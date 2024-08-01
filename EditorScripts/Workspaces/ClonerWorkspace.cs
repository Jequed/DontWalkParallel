using UnityEngine;
using UnityEngine.UI;

public class ClonerWorkspace : EditorView
{
	[SerializeField]
	private InputField rateInputField;

	[SerializeField]
	private InputField patternInputField;

	[SerializeField]
	private InputField delayInputField;

    [SerializeField]
    private Toggle fillPathToggle;

    private PersonCloner Cloner
	{
		get
		{
			return editor.LevelEditor.SelectedItem.GetComponent<PersonCloner>();
		}
	}

	protected override void Start()
	{
		base.Start();

		rateInputField.text = Cloner.SpawnRate.ToString();
		patternInputField.text = Cloner.Pattern;
		delayInputField.text = Cloner.Delay.ToString();
        fillPathToggle.isOn = Cloner.FillPath;

		rateInputField.onValueChanged.AddListener(RateInfputField_OnValueChanged);
		patternInputField.onValueChanged.AddListener(PatterInfputField_OnValueChanged);
		delayInputField.onValueChanged.AddListener(DelayInfputField_OnValueChanged);
        fillPathToggle.onValueChanged.AddListener(FillPathToggle_OnValueChanged);
    }

	void OnDestroy()
	{
		rateInputField.onValueChanged.RemoveListener(RateInfputField_OnValueChanged);
		patternInputField.onValueChanged.RemoveListener(PatterInfputField_OnValueChanged);
		delayInputField.onValueChanged.RemoveListener(DelayInfputField_OnValueChanged);
        fillPathToggle.onValueChanged.RemoveListener(FillPathToggle_OnValueChanged);
    }

	private void RateInfputField_OnValueChanged(string value)
	{
		Cloner.SpawnRate = float.Parse(value);
	}
	private void PatterInfputField_OnValueChanged(string value)
	{
		Cloner.Pattern = value;
	}
	private void DelayInfputField_OnValueChanged(string value)
	{
		Cloner.Delay = float.Parse(value);
	}
    private void FillPathToggle_OnValueChanged(bool value)
    {
        Cloner.FillPath = value;
    }
}