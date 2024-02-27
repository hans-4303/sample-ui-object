using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUICanvas : MonoBehaviour
{
    private GameObject cam;
    [SerializeField]
    private GameObject worldObject;
    [SerializeField]
    private Vector3 rotationOffset;
    [SerializeField]
    private Vector3 positionOffset;

    private void Start()
    {
        cam = Camera.main.gameObject;
    }

    private void LateUpdate()
    {
        if (cam != null)
        {
            this.transform.rotation = Quaternion.Euler(rotationOffset) * cam.transform.rotation;
            this.transform.position = worldObject.transform.position + positionOffset;
        }
    }
}
