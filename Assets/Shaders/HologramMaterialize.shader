Shader "Effects/Hologram Materialize" {
   Properties {
      _MainTex ("RGBA Texture Image", 2D) = "white" {}
      _HologramTexture ("Hologram Texture", 2D) = "white" {}
      _TimeScale("Grid Movement Timescale", Float) = 0.0
      _AnimTime("Animation Time", Float) = 20.0
   }
   SubShader {
      Tags {"Queue" = "Transparent"} 

      Pass {	
       	ZWrite on
        Blend SrcAlpha OneMinusSrcAlpha 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         uniform sampler2D _MainTex;  
         uniform sampler2D _HologramTexture;    
         uniform float _TimeScale;
         uniform float _AnimTime;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.tex = input.texcoord;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	_AnimTime += _Time * _TimeScale;
         
         	float3 originalPosition = input.tex.xyz;
         
          	float4 textureColor = tex2D(_MainTex, input.tex.xy);
          	
          	input.tex.x += _AnimTime * _TimeScale;
          	input.tex.y += _AnimTime * _TimeScale;
          	
          	float4 HologramTexture1 = tex2D(_HologramTexture, input.tex.xy);
          	
          	input.tex.x -= _AnimTime * 2.0 * _TimeScale;
          	input.tex.y -= _AnimTime * 2.0 * _TimeScale;
          	
          	float4 HologramTexture2 = tex2D(_HologramTexture, input.tex.xy);
          	
          	if(originalPosition.x >= _AnimTime)
          	{
          		textureColor.a = 0.0;
          	}
          	else
          	{
          		HologramTexture1.rgba = 0.0;
          		HologramTexture2.rgba = 0.0;
          	}
          	
          	if(_AnimTime > 1.0f)
          	{
          		textureColor.a = 2.0 - (_AnimTime);
          	}
          	else if(_AnimTime < 0.0f)
          	{
          		HologramTexture1.a *= 1.0 + (_AnimTime);
          		HologramTexture2.a *= 1.0 + (_AnimTime);
          	}
 
            return textureColor + HologramTexture1 + HologramTexture2;  
         }
 
         ENDCG
      }
   }
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent"
}