using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VRendererPass : ScriptableRenderPass
{
    private ProfilingSampler m_ProfilingSampler;

    private FilteringSettings m_FilteringSettings;
    private string m_ProfilerTag;
    private VRendererFeature.Settings m_Settings;

    public VRendererPass(VRendererFeature.Settings settings, string profilerTag, RenderPassEvent renderPassEvent)
    {
        m_Settings = settings;
        m_ProfilerTag = profilerTag;
        m_ProfilingSampler = new ProfilingSampler(profilerTag);

        this.renderPassEvent = renderPassEvent;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            cmd.DrawMesh(m_Settings.mesh, Matrix4x4.identity, m_Settings.material);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }
}
