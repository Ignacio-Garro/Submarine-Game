Shader "CustomRenderTexture/posterization"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("InputTex", 2D) = "white" {}
        _PosterizationLevels ("Posterization Levels", Range(2, 10)) = 4
    }

    SubShader
    {
        Blend One Zero

        Pass
        {
            Name "posterization"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4      _Color;
            sampler2D   _MainTex;
            float       _PosterizationLevels;

            // Posterize function: Quantizes color values to create posterization
            float Posterize(float value, float levels) {
                return floor(value * levels) / levels;
            }

            // PostProcess function: Applies posterization to RGB channels
            float4 PostProcess(float4 color, float levels) {
                color.r = Posterize(color.r, levels);
                color.g = Posterize(color.g, levels);
                color.b = Posterize(color.b, levels);
                return color;
            }

            // Fragment shader
            float4 frag(v2f_customrendertexture IN) : SV_Target
            {
                float2 uv = IN.localTexcoord.xy;
                float4 color = tex2D(_MainTex, uv) * _Color;

                // Apply posterization effect
                color = PostProcess(color, _PosterizationLevels);

                return color;
            }
            ENDCG
        }
    }
}
