Shader "V/PlanarShadow" 
{
	Properties 
	{
		_MainTex("Texture", 2D) = "white" {}
		_ShadowInvLen("ShadowInvLen", float) = 1.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
        ZWrite Off
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha
		Cull Back

		Pass 
		{
			Name "PlanarShadow"
			
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
            CBUFFER_START(UnityPerMaterial)
			float4 _ShadowPlane;
			float4 _ShadowProjDir;
			float4 _WorldPos;
			float _ShadowInvLen;
			float4 _ShadowFadeParams;
			float _ShadowFalloff;
            CBUFFER_END
			
            struct Attributes 
            {
                float4 vertex : POSITION;
                //float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
        
            struct Varyings 
            {
                float4 vertex : SV_POSITION;
				float3 xlv_TEXCOORD0 : TEXCOORD0;
				float3 xlv_TEXCOORD1 : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings vert(Attributes input) 
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	
				float4 lightdir = normalize(_ShadowProjDir);
				float3 worldpos = mul(unity_ObjectToWorld, input.vertex).xyz;
				// _ShadowPlane.w = p0 * n  // 平面的w分量就是p0 * n
				float distance = (_ShadowPlane.w - dot(_ShadowPlane.xyz, worldpos)) / dot(_ShadowPlane.xyz, lightdir.xyz);
				worldpos = worldpos + distance * lightdir.xyz;
				o.vertex = mul(unity_MatrixVP, float4(worldpos, 1.0));
				o.xlv_TEXCOORD0 = _WorldPos.xyz;
				o.xlv_TEXCOORD1 = worldpos;

                return o;
            }
			
			half4 frag(Varyings i) : SV_Target
			{
				float3 posToPlane_2 = (i.xlv_TEXCOORD0 - i.xlv_TEXCOORD1);
				float4 color;
				color.xyz = float3(0.0, 0.0, 0.0);

				// 下面两种阴影衰减公式都可以使用(当然也可以自己写衰减公式)
				// 1. 王者荣耀的衰减公式
				color.w = (pow((1.0 - clamp(((sqrt(dot(posToPlane_2, posToPlane_2)) * _ShadowInvLen) - _ShadowFadeParams.x), 0.0, 1.0)), _ShadowFadeParams.y) * _ShadowFadeParams.z);

				// 2. https://zhuanlan.zhihu.com/p/31504088 这篇文章介绍的另外的阴影衰减公式
				//color.w = 1.0 - saturate(distance(i.xlv_TEXCOORD0, i.xlv_TEXCOORD1) * _ShadowFalloff);

				//return color;
				return half4(_ShadowFalloff, _ShadowFalloff, _ShadowFalloff, 1);
			}
            ENDHLSL
		}
	}
}
