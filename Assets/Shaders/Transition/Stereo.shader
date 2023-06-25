Shader "Unlit/Stereo"
{ 
	Properties
	{
	   _MainTex("Main Texture", 2D) = "white" {}
	}  

	SubShader
	{  	
		Tags
        {
		    "RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"          
			"Queue"="Geometry"		
        }
    	Pass
    	{
			Name "Universal Forward"
            Tags {"LightMode" = "UniversalForward"}

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
           		float2 uv 	  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 vertex  	: SV_POSITION;
				float2 uv      	: TEXCOORD0;
				// float4 screenPos: TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
      		};

       		VertexOutput vert(VertexInput v)
        	{
          		VertexOutput o;
       			UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
       			
          		o.vertex = TransformObjectToHClip(v.vertex.xyz);
       			o.uv = v.uv;
          		return o;
        	}	
       		
        	half4 frag(VertexOutput i) : SV_Target
        	{
        		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        		
				float4 color = _MainTex.Sample(sampler_MainTex, i.uv);
          		return color;
        	}
			ENDHLSL  
    	}
    }
}