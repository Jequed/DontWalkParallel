using System;
using UnityEngine;
using UnityEngine.UI;

public class ViewConeUI : MonoBehaviour
{
    [SerializeField]
    private InputField radiusInputField;

    [SerializeField]
    private InputField arcInputField;

    [SerializeField]
    private InputField turnRateInputField;

    [SerializeField]
    private InputField drainRateInputField;

    private ViewCone viewCone;

    private void Start()
    {
        viewCone = GetComponentInParent<PersonPropertyUI>().Property as ViewCone;

        radiusInputField.text = viewCone.Radius.ToString();
        arcInputField.text = viewCone.Arc.ToString();
        turnRateInputField.text = viewCone.TurnRate.ToString();
        drainRateInputField.text = viewCone.DrainRate.ToString();

        radiusInputField.onValueChanged.AddListener(RadiusInputField_OnValueChanged);
        arcInputField.onValueChanged.AddListener(ArcInputField_OnValueChanged);
        turnRateInputField.onValueChanged.AddListener(TurnRateInputField_OnValueChanged);
        drainRateInputField.onValueChanged.AddListener(DrainRateInputField_OnValueChanged);
    }

    private void RadiusInputField_OnValueChanged(string value)
    {
        try
        {
            viewCone.Radius = float.Parse(value);
        }
        catch (FormatException)
        {
        }
    }

    private void ArcInputField_OnValueChanged(string value)
    {
        try
        {
            viewCone.Arc = float.Parse(value);
        }
        catch (FormatException)
        {
        }
    }

    private void TurnRateInputField_OnValueChanged(string value)
    {
        try
        {
            viewCone.TurnRate = float.Parse(value);
        }
        catch (FormatException)
        {
        }
    }

    private void DrainRateInputField_OnValueChanged(string value)
    {
        try
        {
            viewCone.DrainRate = float.Parse(value);
        }
        catch (FormatException)
        {
        }
    }
}