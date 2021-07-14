Shader "V/PlanarShadow" 
{
	Properties 
	{
		_MainTex("Texture", 2D) = "white" {}

		[Header(Shadow)]
		_GroundHeight("_GroundHeight", Float) = 0
		_ShadowFalloff("_ShadowFalloff", Range(0,1)) = 0.05
		_ShadowColor("ShadowColor", Color) = (0,0,0,1)
	}
	SubShader 
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent"
		}
		LOD 300
		
		// 物体自身着色使用URP自带的ForwardLit pass
		//USEPASS "Universal Render Pipeline/Lit/ForwardLit"

        ZWrite Off
        ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
	    //深度稍微偏移防止阴影与地面穿插
	    //Offset - 1 , 0

		Pass 
		{
			Name "PlanarShadow"
			
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
            CBUFFER_START(UnityPerMaterial)
			float _GroundHeight;
			float4 _ShadowColor;
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
				float4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
			float3 ShadowProjectPos(float4 vertPos)
			{
				float3 shadowPos;

				//得到顶点的世界空间坐标
				float3 worldPos = mul(unity_ObjectToWorld, vertPos).xyz;

				//灯光方向
				Light mainLight = GetMainLight();
				float3 lightDir = normalize(mainLight.direction.xyz);

				//阴影的世界空间坐标（低于地面的部分不做改变）
				shadowPos.y = min(worldPos.y, _GroundHeight);
				shadowPos.xz = worldPos.xz - lightDir.xz * max(0, worldPos.y - _GroundHeight) / lightDir.y;

				return shadowPos;
			}

            Varyings vert(Attributes input) 
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				//得到阴影的世界空间坐标
				float3 shadowPos = ShadowProjectPos(input.vertex);

				//转换到裁切空间
				o.vertex = TransformWorldToHClip(shadowPos);
				//o.vertex = mul(UNITY_MATRIX_VP, shadowPos);

				//得到中心点世界坐标
				//unity_ObjectToWorld这个矩阵每一行的第四个分量分别对应Transform的xyz
				float3 center = float3(unity_ObjectToWorld[0].w, _GroundHeight, unity_ObjectToWorld[2].w);
				//计算阴影衰减
				float falloff = 1 - saturate(distance(shadowPos, center) * _ShadowFalloff);

				//阴影颜色
				o.color = _ShadowColor;
				o.color.a *= falloff;

                return o;
            }
			
			half4 frag(Varyings i) : SV_Target
			{
				return i.color;
			}
            ENDHLSL
		}
	}
}
