using TMPro;
using UnityEngine;

public class PropertyDropDown : PropertyUI
{
    public TMP_Dropdown dropdown;

    public GameObject[] toSelect;
    private int selected;

    private void Start()
    {
        if (material != null) dropdown.value = (int)material.GetFloat(propertyID);
        dropdown.onValueChanged.AddListener(SetListProperty);
        dropdown.onValueChanged.AddListener(SelectObject);
    }

    private void SelectObject(int index)
    {
        if (index < 0 || index >= toSelect.Length) return;
        toSelect[selected].SetActive(false);
        selected = index;
        toSelect[selected].SetActive(true);
    }
}