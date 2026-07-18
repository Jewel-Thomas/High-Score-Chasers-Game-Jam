using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireDecouple : MonoBehaviour
{
    [SerializeField] private Transform decoupleTyre;
    [SerializeField] private WheelCollider runningtyreCollider;
    [SerializeField] private MeshRenderer runningTyreMesh;
    private bool isCoupled = true;

    private void Start()
    {
        GameInput.Instance.OnMisc += GameInput_OnMisc;
    }

    private void GameInput_OnMisc(object sender, System.EventArgs e)
    {
        Decouple(isCoupled);
        isCoupled = !isCoupled;
    }

    private void Decouple(bool value)
    {
        MeshRenderer dcpTyreMesh = decoupleTyre.GetComponent<MeshRenderer>();
        Collider dcpTyreCollider = decoupleTyre.GetComponent<Collider>();
        Rigidbody dcpTyreRb = decoupleTyre.GetComponent<Rigidbody>();

        if (value) decoupleTyre.SetParent(null);
        else
        {
            decoupleTyre.SetParent(runningTyreMesh.transform);
            decoupleTyre.localPosition = new Vector3(0, 0, 0);
        }

        dcpTyreMesh.enabled = value;
        dcpTyreCollider.enabled = value;
        dcpTyreRb.isKinematic = !value;

        runningTyreMesh.enabled = !value;
        runningtyreCollider.gameObject.SetActive(!value);
    }
}
