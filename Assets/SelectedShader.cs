using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Shader 시도</para>
/// <para>
///     Mesh1에 대해서만 Shader 적용해봤을 때 선체에 Outline이 걸리진 않았음.
///     그리고 OnMouseDown, OnMouseUp으로 작동하기 위해서는 기준 되는 컴포넌트에 Collider가 걸려 있어야 함.
/// </para>
/// </summary>
public class SelectedShader : MonoBehaviour
{
    Material outline;

    Renderer renderers;
    readonly List<Material> materials = new();

    private void Start ()
    {
        outline = new Material(Shader.Find("Custom/OutlineShader"));
    }

    private void OnMouseDown ()
    {
        renderers = this.GetComponent<Renderer>();

        materials.Clear();
        materials.AddRange(renderers.sharedMaterials);
        materials.Add(outline);

        renderers.materials = materials.ToArray();
    }

    private void OnMouseUp ()
    {
        renderers = this.GetComponent<Renderer>();

        materials.Clear();
        materials.AddRange(renderers.sharedMaterials);
        materials.Remove(outline);

        renderers.materials = materials.ToArray();
    }
}
