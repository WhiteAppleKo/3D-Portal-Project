using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainCamera : MonoBehaviour {

    Portal[] portals;

    void Awake () {
        portals = FindObjectsOfType<Portal> ();
    }
    
    void OnEnable() {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    void OnDisable() {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    void OnBeginCameraRendering (ScriptableRenderContext context, Camera camera) {
        Debug.Log ("OnPreCull");
        for (int i = 0; i < portals.Length; i++) {
            portals[i].PrePortalRender ();
        }
        for (int i = 0; i < portals.Length; i++) {
            portals[i].Render ();
        }

        for (int i = 0; i < portals.Length; i++) {
            portals[i].PostPortalRender ();
        }

    }

}