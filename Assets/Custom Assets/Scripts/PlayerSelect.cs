using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// <para>오브젝트 선택과 필터를 위해 클래스 만들고 활용할 수 있어야 함</para>
/// </summary>
public class SelectableShip
{
    /// <summary>
    /// <para>각 배 오브젝트와 배가 포함하는 메시 렌더러, 캔버스로 구분됨</para>
    /// </summary>
    private GameObject eachShip;
    private List<MeshRenderer> currentMeshRenderers;
    private Canvas eachCanvas;

    /// <summary>
    /// <para>Prop으로 빼주기</para>
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
    /// <para>생성자 작성, 핵심 배 오브젝트 받고 자식 오브젝트 및 컴포넌트 받아오기</para>
    /// <para>
    ///     !! GetComponentInChildren || GetComponentInChildren 메서드에 인수로 true, false 넘길 수 있음.
    ///     false 혹은 작성 안할 시 비활성화된 오브젝트의 컴포넌트는 무시함.
    ///     true 작성 시 비활성화된 오브젝트의 컴포넌트도 취급함.
    ///     그래서 GameObject.Find("오브젝트 이름 1").transform.Find("오브젝트 이름 2").gameObject; 같이 접근할 필요 없어짐
    /// </para>
    /// </summary>
    /// <param name="currentShip">배 오브젝트 -> 자식 오브젝트 및 컴포넌트 조회 가능</param>
    public SelectableShip (GameObject currentShip)
    {
        eachShip = currentShip;
        currentMeshRenderers = currentShip.GetComponentsInChildren<MeshRenderer>(true).ToList();
        eachCanvas = currentShip.GetComponentInChildren<Canvas>(true);
    }
}

/// <summary>
/// <para>
///     아래 같이 선택 및 해제, 상호작용을 분명하게 하는 게 좋다 생각하며
///     오브젝트 구조 바꾸고 레이어 혹은 태그로 구분해야 함.
/// </para>
/// <para>
///     배(박스 컬라이더 및 레이어 혹은 태그 포함) > 메시 그룹 | 월드 스페이스 캔버스(비활성)로 짜야
///     Raycast 타겟 인지 > 자식 컴포넌트 접근 등 과정 잘 됨
/// </para>
/// </summary>
public class PlayerSelect : MonoBehaviour
{
    /// <summary>
    /// <para>LayerMask를 지정하고 바깥에서 Ship 레이어 작성</para>
    /// </summary>
    public LayerMask toDirectObject;

    /// <summary>
    /// <para>셰이더 등록 멤버</para>
    /// </summary>
    private Material shader;

    /// <summary>
    /// <para>레이캐스트 대상 멤버</para>
    /// </summary>
    private RaycastHit hit;

    /// <summary>
    /// <para>선택할 수 있는 배들 리스트</para>
    /// </summary>
    private readonly List<SelectableShip> selectableShips = new();

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
    }

    /// <summary>
    /// <para>
    ///     1. 메인 카메라에서 마우스 위치를 향해 광선 발생
    /// </para>
    /// <para>
    ///     2. 광선, 대상 오브젝트, 사거리, 목표 레이어 만족할 때와 그렇지 않을 때로 구분
    /// </para>
    /// <para>
    ///     3. 목표 만족하고 선택된 배 오브젝트가 기존 리스트에 있다면 선택 해제 메서드 호출
    /// </para>
    /// <para>
    ///     4. 목표 만족하고 선택된 배 오브젝트가 기존 리스트에 없다면 새 인스턴스 만들어서 메서드 호출 및 리스트 추가
    /// </para>
    /// <para>
    ///     5. 목표 만족하지 않고 리스트에 추가된 게 있다면 일괄 해제로 인지,
    ///     List 요소를 제거하기 때문에 역 for문으로 제어함
    /// </para>
    /// </summary>
    private void SelectObject ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, toDirectObject))
        {
            GameObject hitObject = hit.collider.gameObject;

            // FirstOrDefault : 조건에 맞는 첫 요소 혹은 기본 값 반환, 인스턴스니까 기본 값이 Null일 수 있음
            var selectedShip = selectableShips.FirstOrDefault(el => el.EachShip == hitObject);

            if (selectedShip != null)
            {
                // Debug.Log("선택 해제");

                DeselectObject(selectedShip);
            }
            else
            {
                // Debug.Log("선택");

                SelectableShip newSelected = new(hitObject);
                ApplyShaderAndEnableUI(newSelected);
                selectableShips.Add(newSelected);
            }
        }
        else
        {
            if(selectableShips.Count > 0)
            {
                for(int i = selectableShips.Count - 1; i >= 0; i--)
                {
                    DeselectObject(selectableShips[i]);
                }
                // Debug.Log("일괄 해제");
            }
        }
    }

    /// <summary>
    /// <para>셰이더 적용 및 UI 활성 메서드</para>
    /// <para>
    ///     1. 선택된 배 메시 렌더러들 Prop에 접근해서 순회, 스코프 내에서 Material List 다루며 Start에서 지정한 shader를 붙임.
    /// </para>
    /// <para>
    ///     2. 1 과정에서의 List를 ToArray();로 대입함.
    ///     선택된 배 > 각 메시 렌더러 > materials에 접근하기 때문에 배 단독으로 필드나 멤버를 가지지 않아도 됐음.
    /// </para>
    /// <para>
    ///     3. 만약 선택한 배에 캔버스가 있다면 EachCanvas Prop을 통해 활성화 시켜줄 수 있음.
    /// </para>
    /// </summary>
    /// <param name="selected">선택할 수 있는 배를 받음</param>
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

    /// <summary>
    /// <para>셰이더 해제 및 UI 비활성 메서드</para>
    /// <para>
    ///     1. 선택된 배 메시 렌더러들 Prop에 접근해서 순회, 스코프 내에서 Material List 다루며 Start에서 지정한 shader를 제거.
    /// </para>
    /// <para>
    ///     2. 1 과정에서의 List를 ToArray();로 대입함.
    ///     선택된 배 > 각 메시 렌더러 > materials에 접근하기 때문에 배 단독으로 필드나 멤버를 가지지 않아도 됐음.
    /// </para>
    /// <para>
    ///     3. 만약 선택한 배에 캔버스가 있다면 EachCanvas Prop을 통해 비활성화 시켜줄 수 있음.
    /// </para>
    /// <para>
    ///     4. List에서 선택된 요소를 제거함,
    ///     선택 및 해제를 통한 단일 적용 및 다른 영역 선택을 통한 일괄 해제 모두 가능함.
    /// </para>
    /// </summary>
    /// <param name="selected"></param>
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
