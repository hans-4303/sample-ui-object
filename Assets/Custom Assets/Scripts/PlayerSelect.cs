using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <para>������Ʈ ���ð� ���͸� ���� Ŭ���� ����� Ȱ���� �� �־�� ��</para>
/// </summary>
public class Ship
{
    /// <summary>
    /// <para>�� �� ������Ʈ�� �谡 �����ϴ� �޽� ������, ĵ������ ���е�</para>
    /// </summary>
    private GameObject eachShip;
    private List<MeshRenderer> currentMeshRenderers;
    private Canvas eachCanvas;

    /// <summary>
    /// <para>Prop���� ���ֱ�</para>
    /// </summary>
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

    /// <summary>
    /// <para>������ �ۼ�, �ٽ� �� ������Ʈ �ް� �ڽ� ������Ʈ �� ������Ʈ �޾ƿ���</para>
    /// <para>
    ///     !! GetComponentInChildren || GetComponentInChildren �޼��忡 �μ��� true, false �ѱ� �� ����.
    ///     false Ȥ�� �ۼ� ���� �� ��Ȱ��ȭ�� ������Ʈ�� ������Ʈ�� ������.
    ///     true �ۼ� �� ��Ȱ��ȭ�� ������Ʈ�� ������Ʈ�� �����.
    ///     �׷��� GameObject.Find("������Ʈ �̸� 1").transform.Find("������Ʈ �̸� 2").gameObject; ���� ������ �ʿ� ������
    /// </para>
    /// </summary>
    /// <param name="currentShip">�� ������Ʈ -> �ڽ� ������Ʈ �� ������Ʈ ��ȸ ����</param>
    public Ship (GameObject currentShip)
    {
        eachShip = currentShip;
        currentMeshRenderers = currentShip.GetComponentsInChildren<MeshRenderer>(true).ToList();
        eachCanvas = currentShip.GetComponentInChildren<Canvas>(true);
    }
}

/// <summary>
/// <para>
///     �Ʒ� ���� ���� �� ����, ��ȣ�ۿ��� �и��ϰ� �ϴ� �� ���� �����ϸ�
///     ������Ʈ ���� �ٲٰ� ���̾� Ȥ�� �±׷� �����ؾ� ��.
/// </para>
/// <para>
///     ��(�ڽ� �ö��̴� �� ���̾� Ȥ�� �±� ����) > �޽� �׷� | ���� �����̽� ĵ����(��Ȱ��)�� ¥��
///     Raycast Ÿ�� ���� > �ڽ� ������Ʈ ���� �� ���� �� ��
/// </para>
/// </summary>
public class PlayerSelect : MonoBehaviour
{
    /// <summary>
    /// <para>����ĳ��Ʈ ��� ���</para>
    /// </summary>
    private RaycastHit hit;

    /// <summary>
    /// <para>LayerMask�� �����ϰ� �ٱ����� ���̾� �ۼ�</para>
    /// </summary>
    public LayerMask toDirectShip;
    public LayerMask toDirectSafetyZone;

    public GameObject TBMPanel;

    /// <summary>
    /// <para>������ �� �ִ� ��� ����Ʈ</para>
    /// </summary>
    private readonly List<Ship> selectableShips = new();
    public Material overlayMat;

    private void Start ()
    {
        if(TBMPanel != null && TBMPanel.activeSelf)
        {
            TBMPanel.SetActive(false);
        }
    }

    /// <summary>
    /// <para>������Ʈ �� ��Ŭ������ ������Ʈ ����</para>
    /// </summary>
    private void Update ()
    {
        if (Input.GetMouseButtonDown(0)) SelectObject();
    }

    /// <summary>
    /// <para>
    ///     1. ���� ī�޶󿡼� ���콺 ��ġ�� ���� ���� �߻�
    /// </para>
    /// <para>
    ///     2. ����, ��� ������Ʈ, ��Ÿ�, ��ǥ ���̾� ������ ���� �׷��� ���� ���� ����
    /// </para>
    /// <para>
    ///     3. ��ǥ �����ϰ� ���õ� �� ������Ʈ�� ���� ����Ʈ�� �ִٸ� ���� ���� �޼��� ȣ��
    /// </para>
    /// <para>
    ///     4. ��ǥ �����ϰ� ���õ� �� ������Ʈ�� ���� ����Ʈ�� ���ٸ� �� �ν��Ͻ� ���� �޼��� ȣ�� �� ����Ʈ �߰�
    /// </para>
    /// <para>
    ///     5. ��ǥ �������� �ʰ� ����Ʈ�� �߰��� �� �ִٸ� �ϰ� ������ ����,
    ///     List ��Ҹ� �����ϱ� ������ �� for������ ������
    /// </para>
    /// </summary>
    private void SelectObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, toDirectShip))
        {
            GameObject hitObject = hit.collider.gameObject;

            // FirstOrDefault : ���ǿ� �´� ù ��� Ȥ�� �⺻ �� ��ȯ, �ν��Ͻ��ϱ� �⺻ ���� Null�� �� ����
            var selectedShip = selectableShips.FirstOrDefault(el => el.EachShip == hitObject);

            if (selectedShip != null)
            {
                DeselectObject(selectedShip);
            }
            else
            {
                Ship newSelected = new(hitObject);
                ApplyShaderAndEnableUI(newSelected);
                selectableShips.Add(newSelected);
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, toDirectSafetyZone) && TBMPanel != null)
        {
            TBMPanel.SetActive(!TBMPanel.activeSelf);
        }
        else
        {
            if(selectableShips.Count > 0)
            {
                for(int i = selectableShips.Count - 1; i >= 0; i--)
                {
                    DeselectObject(selectableShips[i]);
                }
            }
            if (TBMPanel.activeSelf)
            {
                TBMPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// <para>���̴� ���� �� UI Ȱ�� �޼���</para>
    /// <para>
    ///     1. ���õ� �� �޽� �������� Prop�� �����ؼ� ��ȸ, ������ ������ Material List �ٷ�� Start���� ������ shader�� ����.
    /// </para>
    /// <para>
    ///     2. 1 ���������� List�� ToArray();�� ������.
    ///     ���õ� �� > �� �޽� ������ > materials�� �����ϱ� ������ �� �ܵ����� �ʵ峪 ����� ������ �ʾƵ� ����.
    /// </para>
    /// <para>
    ///     3. ���� ������ �迡 ĵ������ �ִٸ� EachCanvas Prop�� ���� Ȱ��ȭ ������ �� ����.
    /// </para>
    /// </summary>
    /// <param name="selected">������ �� �ִ� �踦 ����</param>
    private void ApplyShaderAndEnableUI (Ship selected)
    {
        foreach (MeshRenderer meshRenderer in selected.CurrentMeshRenderers)
        {
            List<Material> materials = new(meshRenderer.sharedMaterials)
            {
                overlayMat
                // shader
            };
            meshRenderer.materials = materials.ToArray();
        }
        if (selected.EachCanvas) selected.EachCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// <para>���̴� ���� �� UI ��Ȱ�� �޼���</para>
    /// <para>
    ///     1. ���õ� �� �޽� �������� Prop�� �����ؼ� ��ȸ, ������ ������ Material List �ٷ�� Start���� ������ shader�� ����.
    /// </para>
    /// <para>
    ///     2. 1 ���������� List�� ToArray();�� ������.
    ///     ���õ� �� > �� �޽� ������ > materials�� �����ϱ� ������ �� �ܵ����� �ʵ峪 ����� ������ �ʾƵ� ����.
    /// </para>
    /// <para>
    ///     3. ���� ������ �迡 ĵ������ �ִٸ� EachCanvas Prop�� ���� ��Ȱ��ȭ ������ �� ����.
    /// </para>
    /// <para>
    ///     4. List���� ���õ� ��Ҹ� ������,
    ///     ���� �� ������ ���� ���� ���� �� �ٸ� ���� ������ ���� �ϰ� ���� ��� ������.
    /// </para>
    /// </summary>
    /// <param name="selected"></param>
    private void DeselectObject (Ship selected)
    {
        foreach (MeshRenderer meshRenderer in selected.CurrentMeshRenderers)
        {
            List<Material> materials = new(meshRenderer.sharedMaterials);
            materials.Remove(overlayMat);
            meshRenderer.materials = materials.ToArray();
        }
        if (selected.EachCanvas) selected.EachCanvas.gameObject.SetActive(false);

        selectableShips.Remove(selected);
    }
}
