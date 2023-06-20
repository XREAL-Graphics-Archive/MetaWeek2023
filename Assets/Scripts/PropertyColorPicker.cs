using UnityEngine.UI;

public class PropertyColorPicker : PropertyUI
{
    public FlexibleColorPicker colorPicker;

    private void Start()
    {
        if (material != null) colorPicker.color = material.GetColor(propertyID);
        colorPicker.onColorChange.AddListener(SetColorProperty);
    }
}