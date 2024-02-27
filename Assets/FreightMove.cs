using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreightMove : MonoBehaviour
{
    [HideInInspector]
    public bool isSelected = false;

    //[HideInInspector]
    //public float speed = 5.0f;
    //[HideInInspector]
    //public bool isMoving = false;
    //[HideInInspector]
    //public bool isDown = true;
    //[HideInInspector]
    //public bool isCheckSelected = false;

    //[SerializeField]
    //private float freightMoveTime = 25.0f;
    //[SerializeField]
    //private float freightMoveSpeed = 5.0f;

    //public float rayLength;

    //private RaycastHit hit;
    //private readonly Vector3 freightDown = Vector3.down;
    //private readonly Vector3 freightUp = Vector3.up;

    //private GameObject selectOutline;

    //private void Start ()
    //{
    //    rayLength = 1.414f / 2.0f * transform.localScale.y + 0.5f;

    //    selectOutline = this.transform.Find("Outline") ? this.transform.Find("Outline").gameObject : null;
    //    if(selectOutline) selectOutline.SetActive(false);
    //}

    //private void Update ()
    //{
    //    Debug.DrawRay(transform.position, freightDown * rayLength, Color.red);

    //    if(isDown) this.transform.Translate(0, -Time.deltaTime * speed, 0);

    //    if (isSelected == true)
    //    {
    //        Debug.DrawLine(this.transform.position, Input.mousePosition * 1000, Color.red);
    //    }
    //    else
    //    {
    //        selectOutline.SetActive(isCheckSelected == false);
    //    }

    //    if(Input.GetMouseButtonDown(0) && isMoving == false && isCheckSelected == true)
    //    {
    //        Vector3 mousePoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    //        Vector3 blockPoint = Camera.main.WorldToViewportPoint(this.transform.position);

    //        Vector3 moveDir = ( mousePoint.x < blockPoint.x ) ? Vector3.left : Vector3.right;

    //        this.StartCoroutine(MoveFreightTime(moveDir));
    //    }
    //}

    //private IEnumerator MoveFreightTime (Vector3 dir)
    //{
    //    isMoving = true;

    //    float elapsedTime = 0.0f;

    //    Vector3 currentPosition = transform.position;
    //    Vector3 targetPosition = currentPosition + dir;

    //    while (elapsedTime < freightMoveTime)
    //    {
    //        this.transform.position = Vector3.Lerp(currentPosition, targetPosition, elapsedTime / freightMoveTime);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    transform.position = targetPosition;

    //    isMoving = false;
    //}

    //private void OnTriggerEnter (Collider col)
    //{
    //    if (Physics.Raycast(transform.position, freightDown, out hit, rayLength))
    //    {
    //        isDown = false;

    //        float height = hit.transform.position.y + hit.transform.localScale.y / 2.0f + transform.localScale.y / 2.0f;
    //        transform.position = new Vector3(transform.position.x, height, transform.position.z);

    //        Debug.Log("point " + hit.point + "/ distance " + hit.distance + "/ name " + hit.collider.name);
    //    }
    //}
}
