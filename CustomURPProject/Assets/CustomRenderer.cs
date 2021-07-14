using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderer : ScriptableRenderer
{
    public CustomRenderer(ScriptableRendererData data) : base(data)
    {
        
    }

    public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
    {
    }
}
