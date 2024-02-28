using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Shader �õ�</para>
/// <para>
///     Mesh1�� ���ؼ��� Shader �����غ��� �� ��ü�� Outline�� �ɸ��� �ʾ���.
///     �׸��� OnMouseDown, OnMouseUp���� �۵��ϱ� ���ؼ��� ���� �Ǵ� ������Ʈ�� Collider�� �ɷ� �־�� ��.
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
