using TMPro;

public class PropertyDropDown : PropertyUI
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        if (material != null) dropdown.value = (int)material.GetFloat(propertyID);
        dropdown.onValueChanged.AddListener(SetListProperty);
    }
}