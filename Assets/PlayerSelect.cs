using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    public LayerMask toDirectObject;
    private RaycastHit hit;

    private void Update ()
    {
        if (Input.GetMouseButtonDown(0)) SelectObject();

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
        }
    }

    private void SelectObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //string objectName = hit.collider.gameObject.name;
            //Debug.Log(objectName);

            //if (!hit.transform.TryGetComponent<FreightMove>(out var block)) return;

            //block.isSelected = !block.isSelected;
            //Debug.Log(block.isSelected);

            Debug.Log(hit.collider.gameObject.name);
        }
    }
}
