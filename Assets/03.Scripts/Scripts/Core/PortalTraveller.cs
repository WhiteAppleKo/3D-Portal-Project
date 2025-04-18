using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour {

    public GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }
    private string layerNmae = "Clone";
    public Vector3 previousOffsetFromPortal { get; set; }

    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    // 첫 포탈 진입 복제 생성 + 준비
    public virtual void EnterPortalThreshold () {
        if (graphicsClone == null) {
            // 그래픽클론이 없다면
            graphicsClone = Instantiate (graphicsObject);
            //graphicsClone.layer = LayerMask.NameToLayer (layerNmae);
            void SetLayerRecursively(GameObject obj, int newLayer) {
                obj.layer = newLayer;
                foreach (Transform child in obj.transform) {
                    SetLayerRecursively(child.gameObject, newLayer);
                }
            }
            SetLayerRecursively(graphicsClone, LayerMask.NameToLayer(layerNmae));
            // 클론 생성
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            // 클론오브젝트의 부모를 원본의 부모와 동일하게 설정
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            // 스케일을 동일하게 설정
            originalMaterials = GetMaterials (graphicsObject);
            cloneMaterials = GetMaterials (graphicsClone);
            // 클론과 원본 각각의 머테리얼을 가져옴
        } else {
            graphicsClone.SetActive (true);
            // 이미 그래픽클론이 존재한다면  새로 만들지 않고 활성화만 시켜줌
        }
    }

    // 텔레포트를 포함에 포탈에 더이상 닿고있지 않을 때 실행
    // 효과 종료
    public virtual void ExitPortalThreshold () {
        graphicsClone.SetActive (false);
        // SliceNormal 속성에 Vector3.zero를 설정,
        // 오브젝트를 자른 듯한 효과를 표현하는 sliceNoral벡터에 zero를 넣으면
        // 슬라이싱 방향이 없도록 해서 자르지 않는 효과
        for (int i = 0; i < originalMaterials.Length; i++) {
            originalMaterials[i].SetVector ("sliceNormal", Vector3.zero);
        }
        // 이 사람이 만든 custom/slice라는 셰이더에 있음
    }

    // dst = 슬라이싱 오프셋  거리
    // clone = 복제된 오브젝트에 적용할지 원본에 적용할지 true = 복제, false = 원본
    // 슬라이싱 효과 진행 조절
    public void SetSliceOffsetDst (float dst, bool clone) {
        for (int i = 0; i < originalMaterials.Length; i++) {
            if (clone) {
                cloneMaterials[i].SetFloat ("sliceOffsetDst", dst);
            } else {
                originalMaterials[i].SetFloat ("sliceOffsetDst", dst);
            }
            // sliceOffsetDst값을 머테리얼에 설정
        }
    }

    Material[] GetMaterials(GameObject g) {
        var matList = new List<Material>();

        // MeshRenderer에서 머테리얼 가져오기
        var meshRenderers = g.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in meshRenderers) {
            foreach (var mat in renderer.materials) {
                matList.Add(mat);
            }
        }

        // SkinnedMeshRenderer에서 머테리얼 가져오기
        var skinnedMeshRenderers = g.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in skinnedMeshRenderers) {
            foreach (var mat in renderer.materials) {
                matList.Add(mat);
            }
        }

        return matList.ToArray();
    }
    // 전달 받은 게임오브젝트를 포함한 하위 오브젝트들의 MeshRenderer를 배열로 반환하는 코드
    // Material[] GetMaterials (GameObject g) {
    //     var renderers = g.GetComponentsInChildren<MeshRenderer> ();
    //     // g와 그 자식 오브젝트들에서 MeshRenderer 컴포넌트를 찾아 배열 저장
    //     var matList = new List<Material> ();
    //     foreach (var renderer in renderers) {
    //         foreach (var mat in renderer.materials) {
    //             matList.Add (mat);
    //         }
    //     }
    //     return matList.ToArray ();
    // }
}