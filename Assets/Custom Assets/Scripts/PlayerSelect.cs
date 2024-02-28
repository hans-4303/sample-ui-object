using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SelectableShips
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

    public SelectableShips (GameObject currentShip)
    {
        eachShip = currentShip;
        currentMeshRenderers = currentShip.GetComponentsInChildren<MeshRenderer>(true).ToList();
        eachCanvas = currentShip.GetComponentInChildren<Canvas>(true);
    }
}

public class PlayerSelect : MonoBehaviour
{
    /// <summary>
    /// <para>LayerMask를 지정하고 바깥에서 Ship 레이어 작성</para>
    /// </summary>
    public LayerMask toDirectObject;

    /// <summary>
    /// <para>셰이더 적용 대상 오브젝트</para>
    /// </summary>
    private GameObject tempShip;

    /// <summary>
    /// <para>셰이더 등록 멤버</para>
    /// </summary>
    private Material shader;

    /// <summary>
    /// <para>배 선택 시 메시 렌더러 받아올 멤버</para>
    /// </summary>
    private List<MeshRenderer> meshRenderers = new();

    /// <summary>
    /// <para>배 선택 시 캔버스 렌더러 받아올 멤버</para>
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// <para>렌더러 돌면서 셰이더 추가하거나 지울 멤버</para>
    /// </summary>
    private readonly List<Material> materials = new();

    /// <summary>
    /// <para>레이캐스트 대상 멤버</para>
    /// </summary>
    private RaycastHit hit;

    /// <summary>
    /// <para>Start 시 셰이더 할당</para>
    /// </summary>
    private void Start ()
    {
        shader = new Material(Shader.Find("Custom/OutlineShader"));
    }

    /// <summary>
    /// <para>업데이트 시 왼클릭으로 오브젝트 선택</para>
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
    ///     1. 메인 카메라에서 마우스 위치를 향해 광선 발생
    /// </para>
    /// <para>
    ///     2. 광선, 대상 오브젝트, 사거리, 목표 레이어 만족할 때와 그렇지 않을 때로 구분
    /// </para>
    /// <para>
    ///     3. 목표 만족하고 tempShip 오브젝트가 없다면 대상 오브젝트의 컬라이더 - 게임 오브젝트에 접근,
    ///     렌더러는 tempShip 오브젝트의 자식 Renderer 컴포넌트 모음으로 둠,
    ///     Renderer 컴포넌트 모음을 순회하면서 materials에 기존 렌더러 및 셰이더 추가.
    /// </para>
    /// <para>
    ///     4. 목표 만족하지 않으며 tempShip 오브젝트가 있다면
    ///     렌더러는 tempShip 오브젝트의 자식 Renderer 컴포넌트 모음으로 둠,
    ///     Renderer 컴포넌트 모음을 순회하면서 materials에 기존 렌더러 보존하고 셰이더 제거
    /// </para>
    /// </summary>
    private void SelectObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, toDirectObject))
        {
            if (tempShip != null) return;

            tempShip = hit.collider.gameObject;
            meshRenderers = tempShip.GetComponentsInChildren<MeshRenderer>().ToList();
            canvas = tempShip.GetComponentInChildren<Canvas>();

            GameObject table = canvas.GetComponentInChildren<ObjectUI>(true).gameObject;

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                materials.Clear();
                materials.AddRange(meshRenderer.sharedMaterials);
                materials.Add(shader);

                meshRenderer.materials = materials.ToArray();
            }

            table.SetActive(true);
        }
        else
        {
            if (tempShip == null) return;

            meshRenderers = tempShip.GetComponentsInChildren<MeshRenderer>().ToList();
            canvas = tempShip.GetComponentInChildren<Canvas>();
            GameObject table = canvas.GetComponentInChildren<ObjectUI>(true).gameObject;

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                materials.Clear();
                materials.AddRange(meshRenderer.sharedMaterials);
                materials.Remove(shader);

                meshRenderer.materials = materials.ToArray();
            }

            table.SetActive(false);

            tempShip = null;
        }
    }
}
