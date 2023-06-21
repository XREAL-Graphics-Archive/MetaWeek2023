using UnityEngine;
using UnityEngine.EventSystems;

public class PropertyUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Material material;
    public string property;

    protected int propertyID;

    private void Awake()
    {
        propertyID = Shader.PropertyToID(property);
    }

    protected void SetFloatProperty(float value)
    {
        if (material == null) return;
        material.SetFloat(propertyID, value);
    }

    protected void SetColorProperty(Color value)
    {
        if (material == null) return;
        material.SetColor(propertyID, value);
    }

    protected void SetListProperty(int value)
    {
        if (material == null) return;
        material.SetFloat(propertyID, value);
    }

    public void OnPointerDown()
    {
        Pieces.UIHovering += 1;
    }

    public void OnPointerUp()
    {
        Pieces.UIHovering -= 1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUp();
    }
}