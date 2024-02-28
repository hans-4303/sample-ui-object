using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableShip
{
    private GameObject eachShip;
    private List<MeshRenderer> currentMeshRenderers;
    private Canvas eachCanvas;

    public GameObject EachShip
    {
        get { return eachShip; }
        set { eachShip = value; }
    }

    public List<MeshRenderer> CurrentMeshRenderers
    {
        get { return currentMeshRenderers; }
        set { currentMeshRenderers = value; }
    }

    public Canvas EachCanvas
    {
        get { return eachCanvas; }
        set { eachCanvas = value; }
    }

    public SelectableShip (GameObject currentShip)
    {
        eachShip = currentShip;
        currentMeshRenderers = currentShip.GetComponentsInChildren<MeshRenderer>(true).ToList();
        eachCanvas = currentShip.GetComponentInChildren<Canvas>(true);
    }
}

public class PlayerSelect : MonoBehaviour
{
    /// <summary>
    /// <para>LayerMask�� �����ϰ� �ٱ����� Ship ���̾� �ۼ�</para>
    /// </summary>
    public LayerMask toDirectObject;

    /// <summary>
    /// <para>���̴� ��� ���</para>
    /// </summary>
    private Material shader;

    /// <summary>
    /// <para>����ĳ��Ʈ ��� ���</para>
    /// </summary>
    private RaycastHit hit;

    private readonly List<SelectableShip> selectableShips = new();

    /// <summary>
    /// <para>Start �� ���̴� �Ҵ�</para>
    /// </summary>
    private void Start ()
    {
        shader = new Material(Shader.Find("Custom/OutlineShader"));
    }

    /// <summary>
    /// <para>������Ʈ �� ��Ŭ������ ������Ʈ ����</para>
    /// </summary>
    private void Update ()
    {
        if (Input.GetMouseButtonDown(0)) SelectObject();

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
        }
    }

    /// <summary>
    /// <para>
    ///     1. ���� ī�޶󿡼� ���콺 ��ġ�� ���� ���� �߻�
    /// </para>
    /// <para>
    ///     2. ����, ��� ������Ʈ, ��Ÿ�, ��ǥ ���̾� ������ ���� �׷��� ���� ���� ����
    /// </para>
    /// <para>
    ///     3. ��ǥ �����ϰ� tempShip ������Ʈ�� ���ٸ� ��� ������Ʈ�� �ö��̴� - ���� ������Ʈ�� ����,
    ///     �������� tempShip ������Ʈ�� �ڽ� Renderer ������Ʈ �������� ��,
    ///     Renderer ������Ʈ ������ ��ȸ�ϸ鼭 materials�� ���� ������ �� ���̴� �߰�.
    /// </para>
    /// <para>
    ///     4. ��ǥ �������� ������ tempShip ������Ʈ�� �ִٸ�
    ///     �������� tempShip ������Ʈ�� �ڽ� Renderer ������Ʈ �������� ��,
    ///     Renderer ������Ʈ ������ ��ȸ�ϸ鼭 materials�� ���� ������ �����ϰ� ���̴� ����
    /// </para>
    /// </summary>
    private void SelectObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, toDirectObject))
        {
            GameObject hitObject = hit.collider.gameObject;

            var selectedShip = selectableShips.FirstOrDefault(el => el.EachShip == hitObject);

            if (selectedShip != null)
            {
                Debug.Log("���� ����");
                DeselectObject(selectedShip);
            }
            else
            {
                Debug.Log("����");
                SelectableShip newSelected = new(hitObject);
                ApplyShaderAndEnableUI(newSelected);
                selectableShips.Add(newSelected);
            }
        }
    }

    private void ApplyShaderAndEnableUI (SelectableShip selected)
    {
        foreach (MeshRenderer meshRenderer in selected.CurrentMeshRenderers)
        {
            List<Material> materials = new(meshRenderer.sharedMaterials)
            {
                shader
            };
            meshRenderer.materials = materials.ToArray();
        }
        if (selected.EachCanvas) selected.EachCanvas.gameObject.SetActive(true);
    }

    private void DeselectObject (SelectableShip selected)
    {
        foreach (MeshRenderer meshRenderer in selected.CurrentMeshRenderers)
        {
            List<Material> materials = new(meshRenderer.sharedMaterials);
            materials.Remove(shader);
            meshRenderer.materials = materials.ToArray();
        }
        if (selected.EachCanvas) selected.EachCanvas.gameObject.SetActive(false);

        selectableShips.Remove(selected);
    }
}
