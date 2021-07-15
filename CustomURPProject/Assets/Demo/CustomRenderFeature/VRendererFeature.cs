using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VRendererFeature : ScriptableRendererFeature
{
    public Settings settings = new Settings();

    private VRendererPass m_VRenderPass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_VRenderPass);
    }

    public override void Create()
    {
        m_VRenderPass = new VRendererPass(settings, "VRenderPass", settings.Event);
    }

    [System.Serializable]
    public class Settings
    {
        public Mesh mesh;
        public Material material;
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
    }
}
