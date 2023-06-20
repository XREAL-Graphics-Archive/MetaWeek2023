using UnityEngine.UI;

public class PropertySlider : PropertyUI
{
    public Slider slider;

    private void Start()
    {
        if (material != null) slider.value = material.GetFloat(propertyID);
        slider.onValueChanged.AddListener(SetFloatProperty);
    }
}