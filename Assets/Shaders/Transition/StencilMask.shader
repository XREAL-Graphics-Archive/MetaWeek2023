Shader "Unlit/StencilMask"
{ 
	Properties
	{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 1
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
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
	        Blend Zero One
    		ZWrite Off
	        Cull [_Cull]
    		
			Stencil{
    			Ref [_StencilRef]
				Comp Always
                Pass Replace
				Fail Keep
			}
        }
	}
}