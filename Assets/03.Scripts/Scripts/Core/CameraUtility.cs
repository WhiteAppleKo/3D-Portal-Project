using UnityEngine;

public static class CameraUtility {
    static readonly Vector3[] cubeCornerOffsets = {
        new Vector3 (1, 1, 1),
        new Vector3 (-1, 1, 1),
        new Vector3 (-1, -1, 1),
        new Vector3 (-1, -1, -1),
        new Vector3 (-1, 1, -1),
        new Vector3 (1, -1, -1),
        new Vector3 (1, 1, -1),
        new Vector3 (1, -1, 1),
    };

    // http://wiki.unity3d.com/index.php/IsVisibleFrom
    // 카메라의 시야는 6개의 평면 앞뒤위아래좌우 로 구성된 시야 피라미드(frustum)임
    // 이평면들을 배열로 가져옴.
    // 충돌테스트해서 충돌하면 범위안에 있으니 보이는거 true 반환
    public static bool VisibleFromCamera (Renderer renderer, Camera camera) {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (frustumPlanes, renderer.bounds);
    }

    public static bool BoundsOverlap (MeshFilter nearObject, MeshFilter farObject, Camera camera) {

        // 가까운 물체 먼 물체 두개가 화면상에서 겹치는지 확인
        var near = GetScreenRectFromBounds (nearObject, camera);
        var far = GetScreenRectFromBounds (farObject, camera);

        // X축과 Y축이 각각 겹치지않으면 false
        // 둘 다 겹치면 시각적으로 겹친다고 판단
        if (far.zMax > near.zMin) {
            // X축 겹침 확인
            if (far.xMax < near.xMin || far.xMin > near.xMax) {
                return false;
            }
            // Y축 겹침 확인
            if (far.yMax < near.yMin || far.yMin > near.yMax) {
                return false;
            }
            // 겹침
            return true;
        }
        return false;
    }

    // With thanks to http://www.turiyaware.com/a-solution-to-unitys-camera-worldtoscreenpoint-causing-ui-elements-to-display-when-object-is-behind-the-camera/
    public static MinMax3D GetScreenRectFromBounds (MeshFilter renderer, Camera mainCamera) {
        MinMax3D minMax = new MinMax3D (float.MaxValue, float.MinValue);

        // Mesh의 로컬 바운딩 박스 정보
        Vector3[] screenBoundsExtents = new Vector3[8];
        // 8개 박스 꼭짓점 방향 벡터 sharedMesh.bounds는 SkinnedMeshRenderer를 통해 접근 가능한 메쉬의 바운딩 박스 정보를 가져옴
        var localBounds = renderer.sharedMesh.bounds;
        // 카메라에 하나라도 있는지
        bool anyPointIsInFrontOfCamera = false;

        for (int i = 0; i < 8; i++) {
            Vector3 localSpaceCorner = localBounds.center + Vector3.Scale (localBounds.extents, cubeCornerOffsets[i]);
            Vector3 worldSpaceCorner = renderer.transform.TransformPoint (localSpaceCorner);
            Vector3 viewportSpaceCorner = mainCamera.WorldToViewportPoint (worldSpaceCorner);

            if (viewportSpaceCorner.z > 0) {
                anyPointIsInFrontOfCamera = true;
            } else {
                // Z값이 0보다 작아 카메라 뒤에 있는걸로 판정이되면
                // 화면 반대편으로 튀는 것을 방지하기위해 x,y값을 클램핑함
                viewportSpaceCorner.x = (viewportSpaceCorner.x <= 0.5f) ? 1 : 0;
                viewportSpaceCorner.y = (viewportSpaceCorner.y <= 0.5f) ? 1 : 0;
            }

            // Update bounds with new corner point
            minMax.AddPoint (viewportSpaceCorner);
        }

        // All points are behind camera so just return empty bounds
        if (!anyPointIsInFrontOfCamera) {
            return new MinMax3D ();
        }

        return minMax;
    }

    public struct MinMax3D {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
        public float zMin;
        public float zMax;

        public MinMax3D (float min, float max) {
            this.xMin = min;
            this.xMax = max;
            this.yMin = min;
            this.yMax = max;
            this.zMin = min;
            this.zMax = max;
        }

        public void AddPoint (Vector3 point) {
            xMin = Mathf.Min (xMin, point.x);
            xMax = Mathf.Max (xMax, point.x);
            yMin = Mathf.Min (yMin, point.y);
            yMax = Mathf.Max (yMax, point.y);
            zMin = Mathf.Min (zMin, point.z);
            zMax = Mathf.Max (zMax, point.z);
        }
    }

}