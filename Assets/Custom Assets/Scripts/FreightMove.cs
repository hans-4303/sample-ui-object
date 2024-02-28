using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreightMove : MonoBehaviour
{
    [HideInInspector]
    public bool isSelected = false;

    private GameObject selectOutline;

    private void Start ()
    {
        selectOutline = this.transform.Find("Outline").gameObject;
        selectOutline.SetActive(false);
        Debug.Log(selectOutline);
    }

    private void Update ()
    {
        if(isSelected)
        {
            Debug.DrawLine(this.transform.position, Input.mousePosition * 1000, Color.red);
            selectOutline.SetActive(true);
        }
        else
        {
            selectOutline.SetActive(false);
        }
    }
}
