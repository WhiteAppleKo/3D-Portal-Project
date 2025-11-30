using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
    
    // 이벤트 선언
    public static event Action<Portal> OnEnablePortal;
    public static event Action<Portal> OnDisablePortal;
    
    [Header ("Main Settings")]
    public Portal linkedPortal;
    public MeshRenderer screen;
    public int recursionLimit = 5;

    [Header ("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    // "Wall" 레이어와 "Player" 레이어 간 충돌 무시 설정
    private int wallLayer;
    private int playerLayer;
    RenderTexture viewTexture;
    Camera portalCam;
    public Camera playerCam;
    Material firstRecursionMat;
    List<PortalTraveller> trackedTravellers;
    MeshFilter screenMeshFilter;

    private bool isAdded = false;
    void Awake () {
        portalCam = GetComponentInChildren<Camera> ();
        portalCam.enabled = false;
        trackedTravellers = new List<PortalTraveller> ();
        screenMeshFilter = screen.GetComponent<MeshFilter> ();
        screen.material.SetInt ("displayMask", 1);
        playerLayer = LayerMask.NameToLayer("Player");
        wallLayer = LayerMask.NameToLayer("Wall");
        
        OnEnablePortal?.Invoke(gameObject.GetComponent<Portal>());
        isAdded = true;
    }
    

    private void OnEnable()
    {
        if (isAdded == false)
        {
            OnEnablePortal?.Invoke(gameObject.GetComponent<Portal>());
            isAdded = true;
        }
        
        // 모든 레이어 간 충돌 활성화
        for (int layerA = 0; layerA < 32; layerA++)
        {
            Physics.IgnoreLayerCollision(wallLayer, layerA, false);
            
        }
    }

    private void OnDisable()
    {
        if (isAdded == true)
        {
            OnDisablePortal?.Invoke(gameObject.GetComponent<Portal>());
            isAdded = false;
        }

        // 모든 레이어 간 충돌을 활성화
        Physics.IgnoreLayerCollision(wallLayer, playerLayer, false);
    }

    void LateUpdate () {
        HandleTravellers ();
    }
    
    void HandleTravellers () {
    
        for (int i = 0; i < trackedTravellers.Count; i++) {
            PortalTraveller traveller = trackedTravellers[i];
            if (traveller == null)
            {
                trackedTravellers.RemoveAt (i);
                i--;
                continue;
            }
            Transform travellerT = traveller.transform;
            // Traveller의 월드 좌표를 현재 포탈의 로컬 좌표로 변환하고
            // Traveller의 현재 포털 로컬 좌표를 연결된 포탈의 월드 좌표로 변환
            var m = linkedPortal.transform.localToWorldMatrix * (transform.worldToLocalMatrix * travellerT.localToWorldMatrix);
    
            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign (Vector3.Dot (offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign (Vector3.Dot (traveller.previousOffsetFromPortal, transform.forward));
            // **변경된 부분**: Y축 방향 판정을 추가
            //int portalSideY = System.Math.Sign(Vector3.Dot (offsetFromPortal, transform.up));
            //int portalSideYOld = System.Math.Sign(Vector3.Dot (traveller.previousOffsetFromPortal, transform.up));
            
            // 포탈 통과 판정 조건문
            if (portalSide != portalSideOld) { // **변경된 부분**
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);

                traveller.graphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;

            } else {
                var Rotation = traveller.graphicsObject.transform.rotation; 
                Vector3 newPosition = m.GetColumn(3); // 변환 행렬에서 계산된 위치 사용
                traveller.graphicsClone.transform.SetPositionAndRotation(newPosition, Rotation); 
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }
    
    public void PrePortalRender () {
        foreach (var traveller in trackedTravellers) {
            UpdateSliceParams (traveller);
        }
    }
    
    public void Render () {
        if (!CameraUtility.VisibleFromCamera (linkedPortal.screen, playerCam)) {
            return;
        }

        CreateViewTexture ();

        
        Vector3[] renderPositions = new Vector3[recursionLimit];
        Quaternion[] renderRotations = new Quaternion[recursionLimit];

        int startIndex = 0;
        portalCam.projectionMatrix = playerCam.projectionMatrix;
        
        Matrix4x4 localToWorldMatrix = playerCam.transform.localToWorldMatrix; 
        for (int i = 0; i < recursionLimit; i++) {
            if (i > 0) {
                // 포탈이 서로를 비추고 있는 상황이 아니라면 반복을 중단
                if (!CameraUtility.BoundsOverlap (screenMeshFilter, linkedPortal.screenMeshFilter, portalCam)) {
                    break;
                    // 포탈 간의 화면이 겹치지 않으면 렌더링을 중단
                }
            }
            // 현재 포탈의 로컬 좌표계를 연결된 포탈의 로컬 좌표계로 변환
            localToWorldMatrix = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            
            int renderOrderIndex = recursionLimit - i - 1;
            //변환 행렬에서 위치(Position) 정보 GetColumn(3)를 가져오고 회전(Rotation) 정보는 localToWorldMatrix.rotation으로 가져옴
            renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn (3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

            portalCam.transform.SetPositionAndRotation (renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        // Hide screen so that camera can see through portal
        screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        linkedPortal.screen.material.SetInt ("displayMask", 0);

        for (int i = startIndex; i < recursionLimit; i++) {
            portalCam.transform.SetPositionAndRotation (renderPositions[i], renderRotations[i]);
            SetNearClipPlane ();
            HandleClipping ();
            portalCam.Render ();

            if (i == startIndex) {
                linkedPortal.screen.material.SetInt ("displayMask", 1);
            }
        }

        // Unhide objects hidden at start of render
        screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    void HandleClipping () {
        const float hideDst = -1000;
        const float showDst = 1000;
        float screenThickness = linkedPortal.ProtectScreenFromClipping (portalCam.transform.position);

        foreach (var traveller in trackedTravellers) {
            if (SameSideOfPortal (traveller.transform.position, portalCamPos)) {
                traveller.SetSliceOffsetDst (hideDst, false);
            } else {
                traveller.SetSliceOffsetDst (showDst, false);
            }

            int cloneSideOfLinkedPortal = -SideOfPortal (traveller.transform.position);
            bool camSameSideAsClone = linkedPortal.SideOfPortal (portalCamPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone) {
                traveller.SetSliceOffsetDst (screenThickness, true);
            } else {
                traveller.SetSliceOffsetDst (-screenThickness, true);
            }
        }

        var offsetFromPortalToCam = portalCamPos - transform.position;
        foreach (var linkedTraveller in linkedPortal.trackedTravellers) {
            if(linkedTraveller == null) continue;
            var travellerPos = linkedTraveller.graphicsObject.transform.position;
            var clonePos = linkedTraveller.graphicsClone.transform.position;

            bool cloneOnSameSideAsCam = linkedPortal.SideOfPortal (travellerPos) != SideOfPortal (portalCamPos);
            if (cloneOnSameSideAsCam) {
                linkedTraveller.SetSliceOffsetDst (hideDst, true);
            } else {
                linkedTraveller.SetSliceOffsetDst (showDst, true);
            }

            bool camSameSideAsTraveller = linkedPortal.SameSideOfPortal (linkedTraveller.transform.position, portalCamPos);
            if (camSameSideAsTraveller) {
                linkedTraveller.SetSliceOffsetDst (screenThickness, false);
            } else {
                linkedTraveller.SetSliceOffsetDst (-screenThickness, false);
            }
        }
    }

    public void PostPortalRender () {
        foreach (var traveller in trackedTravellers) {
            UpdateSliceParams (traveller);
        }
        ProtectScreenFromClipping (playerCam.transform.position);
    }
    void CreateViewTexture () {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height) {
            if (viewTexture != null) {
                viewTexture.Release ();
            }
            viewTexture = new RenderTexture (Screen.width, Screen.height, 0);
            portalCam.targetTexture = viewTexture;
            linkedPortal.screen.material.SetTexture ("_MainTex", viewTexture);
        }
    }
    
    float ProtectScreenFromClipping (Vector3 viewPoint) {
        float halfHeight = playerCam.nearClipPlane * Mathf.Tan (playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, playerCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot (transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3 (screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void UpdateSliceParams (PortalTraveller traveller) {
        if (traveller == null) return;
        int side = SideOfPortal (traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = linkedPortal.transform.forward * side;
        
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = linkedPortal.transform.position;
        
        float screenThickness = screen.transform.localScale.z;
        float sliceOffsetDst = SameSideOfPortal(playerCam.transform.position, traveller.transform.position) ? 0 : -screenThickness;
        float cloneSliceOffsetDst = linkedPortal.SameSideOfPortal(playerCam.transform.position, traveller.transform.position) ? 0 : -screenThickness;

        bool playerSameSideAsTraveller = SameSideOfPortal (playerCam.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller) {
            sliceOffsetDst = -screenThickness;
        }
        bool playerSameSideAsCloneAppearing = side != linkedPortal.SideOfPortal (playerCam.transform.position);
        if (!playerSameSideAsCloneAppearing) {
            cloneSliceOffsetDst = -screenThickness;
        }
        
        for (int i = 0; i < traveller.originalMaterials.Length; i++) {
            traveller.originalMaterials[i].SetVector ("sliceCentre", slicePos);
            traveller.originalMaterials[i].SetVector ("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat ("sliceOffsetDst", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector ("sliceCentre", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector ("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat ("sliceOffsetDst", cloneSliceOffsetDst);

        }
    }
    
    void SetNearClipPlane () {
        Transform clipPlane = transform;
        int dot = System.Math.Sign (Vector3.Dot (clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint (clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector (clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal) + nearClipOffset;
        
        if (Mathf.Abs (camSpaceDst) > nearClipLimit) {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);
            
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } else {
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    void OnTravellerEnterPortal (PortalTraveller traveller)
    {
        int travellerLayer = traveller.gameObject.layer;
        if (!trackedTravellers.Contains (traveller)) {
            traveller.EnterPortalThreshold ();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add (traveller);
            Debug.Log($"{traveller.transform.name}");
            Physics.IgnoreLayerCollision(wallLayer, travellerLayer, true);
        }
        Physics.IgnoreLayerCollision(wallLayer, travellerLayer, true);
    }

    void OnTriggerEnter (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller) {
            OnTravellerEnterPortal (traveller);
        }
    }

    void OnTriggerExit (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        int travellerLayer = traveller.gameObject.layer;
        if (traveller && trackedTravellers.Contains (traveller)) {
            traveller.ExitPortalThreshold ();
            trackedTravellers.Remove(traveller);
            Physics.IgnoreLayerCollision(wallLayer, travellerLayer, false);
        }
        Physics.IgnoreLayerCollision(wallLayer, travellerLayer, false);
    }

    int SideOfPortal (Vector3 pos) {
        return System.Math.Sign (Vector3.Dot (pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal (Vector3 posA, Vector3 posB) {
        return SideOfPortal (posA) == SideOfPortal (posB);
    }

    Vector3 portalCamPos {
        get {
            return portalCam.transform.position;
        }
    }

    void OnValidate () {
        if (linkedPortal != null) {
            linkedPortal.linkedPortal = this;
        }
    }
}