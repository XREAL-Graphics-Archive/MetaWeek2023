Shader "Unlit/Transparent Rim"
{ 
	Properties
	{
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
	}

	SubShader
	{  	
		Tags
        {
		    "RenderPipeline"="UniversalPipeline"
			"RenderType"="Transparent"          
			"Queue"="Transparent"		
        }
		Pass{
			Name "Outline"
			Tags {"LightMode" = "SRPDefaultUnlit"}
			
			Blend [_SrcBlend] [_DstBlend]
			ZWrite Off
			Cull Front
			
			HLSLPROGRAM
        	#pragma prefer_hlslcc gles
        	#pragma exclude_renderers d3d11_9x
        	#pragma vertex vert
        	#pragma fragment frag		

       		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			
			float4 _MainTex_ST;
			Texture2D _MainTex;
			SamplerState sampler_MainTex;	

			struct VertexInput
			{
           		float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 vertex  	: SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir	: TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
      		};

       		VertexOutput vert(VertexInput v)
        	{
          		VertexOutput o;
       			UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
       			
          		o.vertex = TransformObjectToHClip(v.vertex.xyz);
       			o.normal = TransformObjectToWorldNormal(v.normal);
       			o.viewDir = normalize(_WorldSpaceCameraPos - TransformObjectToWorld(v.vertex.xyz).xyz);
          		return o;
        	}	
       		
        	half4 frag(VertexOutput i) : SV_Target
        	{
        		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        		float rim = 1 - abs(dot(i.viewDir, i.normal));
        		rim = pow(rim, 2);
        		half4 color = half4(1,1,1,1);
        		color *= rim;
          		return color;
        	}
			ENDHLSL  
		}
    }
}