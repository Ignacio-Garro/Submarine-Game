Shader "CustomRenderTexture/OutsideWaterShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("InputTex", 2D) = "white" {}
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveScale("Wave Scale", Float) = 0.1
     }

     SubShader
     {
        Tags { "Queue" = "Geometry" }
        Stencil {
            Ref 1
            Comp NotEqual
        }

        Pass
        {
            Name "OutsideWaterShader"
            CGPROGRAM
            #pragma vertex vsMain
            #pragma fragment psMain

            # include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;
            float _WaveSpeed;
            float _WaveScale;


            struct VsIn
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct VsOut
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            VsOut vsMain(appdata_full v)
            {
                VsOut o;

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                v.vertex.y += sin(_Time.z * _WaveSpeed + worldPos.x*10) * _WaveScale;
                o.pos = UnityObjectToClipPos(v.vertex);
    o.pos.y += sin(_Time.z * _WaveSpeed + worldPos.x) * _WaveScale;
    o.color = float4(v.normal, 1.0);
                return o;
            }
            float4 psMain(VsOut i) : COLOR {

                return _Color;
            }

            ENDCG

        }
    }
}
