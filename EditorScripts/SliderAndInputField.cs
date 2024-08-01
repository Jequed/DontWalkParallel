using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderAndInputField : MonoBehaviour
{
	public event Action<float> OnValueChanged;

	[SerializeField]
	private Slider slider;

	[SerializeField]
	private InputField inputField;

	private bool lockListeners = false;

	public float Value
	{
		get
		{
			return slider.value;
		}
        set
        {
            ChangeValue(value);
        }
	}

	void Start()
	{
		slider.onValueChanged.AddListener(Slider_OnValueChanged);
		inputField.onEndEdit.AddListener(InputField_OnValueChanged);
	}

	void OnDestroy()
	{
		slider.onValueChanged.RemoveListener(Slider_OnValueChanged);
		inputField.onEndEdit.RemoveListener(InputField_OnValueChanged);
	}

	private void Slider_OnValueChanged(float value)
	{
		ChangeValue(value);
	}
	private void InputField_OnValueChanged(string value)
	{
		ChangeValue(float.Parse(value));
	}

	private void ChangeValue(float value)
	{
		if (lockListeners)
			return;

		lockListeners = true;

		slider.value = value;
		inputField.text = value.ToString();
		if (OnValueChanged != null)
			OnValueChanged(value);

		lockListeners = false;
	}
}