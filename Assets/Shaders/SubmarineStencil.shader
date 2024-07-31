Shader "Custom/SubmarineStencil"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-1" "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass {
            Blend Zero One
            ZWrite Off
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }
        }   
    }
}
