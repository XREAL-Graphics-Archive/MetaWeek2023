using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeSelect : MonoBehaviour
{
    [SerializeField] Renderer target;
    int pieceNum;

    [SerializeField] Material[] mats;
    [SerializeField] TMP_Text label;
    int index = 0;
    
    void Awake()
    {
        pieceNum = int.Parse(transform.parent.gameObject.name);
        target = GameObject.Find("Pieces").transform.GetChild(pieceNum).GetChild(1).gameObject.GetComponent<Renderer>();
    }

    public void Button(int add)
    {
        index += add;
        if (index < 0 || index == mats.Length) index = (index + mats.Length) % mats.Length;
        print(index);

        target.material = mats[index];
        label.text = "Material " + (char)('A' + index);
    }

    
}
