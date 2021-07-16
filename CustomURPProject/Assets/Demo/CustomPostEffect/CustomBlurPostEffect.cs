using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("Post-processing/CustomBlur")]
public class CustomBlurPostEffect : VolumeComponent, IPostProcessComponent
{
    public MinFloatParameter blurAmountX = new MinFloatParameter(0, 0);
    public MinFloatParameter blurAmountY = new MinFloatParameter(0, 0);

    public bool IsActive()
    {
        return blurAmountX.value > 0 && blurAmountY.value > 0;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}
