using TMPro;

public class PropertyDropDown : PropertyUI
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown.value = (int)material.GetFloat(propertyID);
        dropdown.onValueChanged.AddListener(SetListProperty);
    }
}