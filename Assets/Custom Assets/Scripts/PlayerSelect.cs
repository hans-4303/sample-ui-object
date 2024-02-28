using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

            // Q. 활성화된 오브젝트 VS 비활성화된 오브젝트
            tempShip = hit.collider.gameObject;

            // 활성화된 오브젝트에서는 GetComponentsInChildren을 통해 바로 조회 및 조작 가능했음
            meshRenderers = tempShip.GetComponentsInChildren<MeshRenderer>().ToList();

            // 또 배마다 월드 스페이스 캔버스 하나 씩 두니까 아래 같이 찾을 수 있음
            canvas = tempShip.GetComponentInChildren<Canvas>();

            // 그런데 비활성화된 오브젝트는 위와 같이 GetComponent로 찾을 수 없었음

            // GameObject test = canvas.GetComponentInChildren<ObjectUI>().gameObject;
            // Debug.Log("Null 뜨면 방법 없음" + test);

            // transform.Find("이름").gameObject;로 찾았을 때는 활성, 비활성 가리지 않고 잘 찾았음
            // 비활성화된 오브젝트는 무조건 이런 방식으로 이름 적으며 찾아야 하는지 알고 싶음
            GameObject table = canvas.transform.Find("TableLayout").gameObject;

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
            canvas.enabled = false;

            GameObject table = canvas.transform.Find("TableLayout").gameObject;

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
