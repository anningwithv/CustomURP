using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class BlurRendererFeature : ScriptableRendererFeature
{
    public Settings settings = new Settings();

    private BlurRendererPass m_BlurRenderPass = null;
    private Vector2 m_CurBlurAmount = Vector2.zero;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_BlurRenderPass.SetUp(renderer.cameraColorTarget);

        renderer.EnqueuePass(m_BlurRenderPass);
    }

    public override void Create()
    {
        m_BlurRenderPass = new BlurRendererPass(settings, "BlurRenderPass", settings.Event);
    }

    //void Update()
    //{
    //    if (m_BlurRenderPass != null)
    //    {
    //        if (m_CurBlurAmount != settings.blurAmount)
    //        {
    //            m_CurBlurAmount = settings.blurAmount;
    //            m_BlurRenderPass.UpdateBlurAmount(m_CurBlurAmount);
    //        }
    //    }
    //}

    [System.Serializable]
    public class Settings
    {
        public Vector2 blurAmount;
        public Material blurMaterial;

        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
    }
}
