Shader "Custom/SubmarineStencil"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-1" "RenderType" = "Opaque" "RenderPipeline" = "HDRenderPipeline" }
        Pass {
            ColorMask 0
            ZWrite Off
            Cull Off
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }
        }   
    }
}
