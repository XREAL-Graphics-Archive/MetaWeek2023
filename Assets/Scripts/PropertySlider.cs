using UnityEngine.UI;

public class PropertySlider : PropertyUI
{
    public Slider slider;

    private void Start()
    {
        slider.value = material.GetFloat(propertyID);
        slider.onValueChanged.AddListener(SetFloatProperty);
    }
}