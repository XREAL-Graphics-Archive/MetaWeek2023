using UnityEngine.UI;

public class PropertyColorPicker : PropertyUI
{
    public FlexibleColorPicker colorPicker;

    private void Start()
    {
        colorPicker.color = material.GetColor(propertyID);
        colorPicker.onColorChange.AddListener(SetColorProperty);
    }
}