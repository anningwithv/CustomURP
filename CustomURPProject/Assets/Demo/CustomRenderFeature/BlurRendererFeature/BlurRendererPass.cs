using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRendererPass : ScriptableRenderPass
{
    private ProfilingSampler m_ProfilingSampler;

    private FilteringSettings m_FilteringSettings;
    private string m_ProfilerTag;
    private BlurRendererFeature.Settings m_Settings;

    private RenderTextureDescriptor m_OpaqueDesc;
    private RenderTargetIdentifier m_CamerColorTexture;

    public BlurRendererPass(BlurRendererFeature.Settings settings, string profilerTag, RenderPassEvent renderPassEvent)
    {
        m_Settings = settings;
        m_ProfilerTag = profilerTag;
        m_ProfilingSampler = new ProfilingSampler(profilerTag);

        this.renderPassEvent = renderPassEvent;
    }

    public void SetUp(RenderTargetIdentifier destination)
    {
        m_CamerColorTexture = destination;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        base.Configure(cmd, cameraTextureDescriptor);

        m_OpaqueDesc = cameraTextureDescriptor;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            //降低分辨率
            m_OpaqueDesc.width /= 4;
            m_OpaqueDesc.height /= 4;

            int blurredRT1ID = Shader.PropertyToID("_BlurRT1");
            int blurredRT2ID = Shader.PropertyToID("_BlurRT2");
            cmd.GetTemporaryRT(blurredRT1ID, m_OpaqueDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(blurredRT2ID, m_OpaqueDesc, FilterMode.Bilinear);
            //颜色RT Blit到临时RT中
            cmd.Blit(m_CamerColorTexture, blurredRT1ID);
            //横向纵向做Blur模糊
            cmd.SetGlobalVector("offsets", new Vector4(m_Settings.blurAmount.x / Screen.width, 0, 0, 0));
            cmd.Blit(blurredRT1ID, blurredRT2ID, m_Settings.blurMaterial);
            cmd.SetGlobalVector("offsets", new Vector4(0, m_Settings.blurAmount.y / Screen.height, 0, 0));
            cmd.Blit(blurredRT2ID, blurredRT1ID, m_Settings.blurMaterial);
            cmd.SetGlobalVector("offsets", new Vector4(m_Settings.blurAmount.x * 2 / Screen.width, 0, 0, 0));
            cmd.Blit(blurredRT1ID, blurredRT2ID, m_Settings.blurMaterial);
            cmd.SetGlobalVector("offsets", new Vector4(0, m_Settings.blurAmount.y * 2 / Screen.height, 0, 0));
            cmd.Blit(blurredRT2ID, blurredRT1ID, m_Settings.blurMaterial);
            //最后在把临时RT Blit回颜色RT
            cmd.Blit(blurredRT1ID, m_CamerColorTexture);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
