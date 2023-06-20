Shader "Unlit/PortalBall"
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
			};

			struct VertexOutput
			{
				float4 vertex  	: SV_POSITION;
				float2 uv      	: TEXCOORD0;
				float4 screenPos: TEXCOORD1;
      		};

       		VertexOutput vert(VertexInput v)
        	{
          		VertexOutput o;				
          		o.vertex = TransformObjectToHClip(v.vertex.xyz);
          		// o.uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
       			o.screenPos = ComputeScreenPos(o.vertex);
          		return o;
        	}	
       		
        	half4 frag(VertexOutput i) : SV_Target
        	{
        		i.screenPos /= i.screenPos.w;
				float4 color = _MainTex.Sample(sampler_MainTex, float2(i.screenPos.xy));
          		return color;
        	}
			ENDHLSL  
    	}
    }
}