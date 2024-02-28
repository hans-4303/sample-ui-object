using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedShaderHuge : MonoBehaviour
{
    private RaycastHit hit;

    Material shader;
    readonly List<Material> materials = new();

    private bool isSelected = false;

    private void Start ()
    {
        shader = new Material(Shader.Find("Custom/OutlineShader"));
    }

    private void Update ()
    {
        // if (Input.GetMouseButtonDown(0)) SelectObject();

        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
        //}
    }

    /// <summary>
    /// <para></para>
    /// </summary>
    private void SelectObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if(isSelected == false)
            {
                Renderer[] childrens = this.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in childrens)
                {
                    materials.Clear();
                    materials.AddRange(r.sharedMaterials);
                    materials.Add(shader);

                    r.materials = materials.ToArray();
                    isSelected = true;
                }
            }
            else
            {
                Renderer[] childrens = this.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in childrens)
                {
                    materials.Clear();
                    materials.AddRange(r.sharedMaterials);
                    materials.Remove(shader);

                    r.materials = materials.ToArray();
                    isSelected = false;
                }
            }
        }
    }
}
