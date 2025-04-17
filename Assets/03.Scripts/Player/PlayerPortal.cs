using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortal : PortalTraveller {
    Vector3 velocity;
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms ();
    }

}