Shader "FullScreen/completePost"
{
    Properties
    {
        _PixelizationIntensity("Pixelization Intensity", Range(0, 1)) = 0
        _PixelizationResolution("Pixelization Resolution", Range(0, 1)) = 0
        _SmoothQuantizationIntensity("Smooth Quantization Intensity", Range(0, 1)) = 0
        _SmoothQuantizationSteps("Smooth Quantization Steps", Range(1, 100)) = 10
        _RoughQuantizationIntensity("Rough Quantization Intensity", Range(0, 1)) = 0
        _RoughQuantizationSteps("Rough Quantization Steps", Range(1, 100)) = 10
    }
    HLSLINCLUDE
    
float _PixelizationIntensity;
float _PixelizationResolution;
float _SmoothQuantizationSteps;
float _SmoothQuantizationIntensity;
float _RoughQuantizationIntensity;
float _RoughQuantizationSteps;

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    // The PositionInputs struct allow you to retrieve a lot of useful information for your fullScreenShader:
    // struct PositionInputs
    // {
    //     float3 positionWS;  // World space position (could be camera-relative)
    //     float2 positionNDC; // Normalized screen coordinates within the viewport    : [0, 1) (with the half-pixel offset)
    //     uint2  positionSS;  // Screen space pixel coordinates                       : [0, NumPixels)
    //     uint2  tileCoord;   // Screen tile coordinates                              : [0, NumTiles)
    //     float  deviceDepth; // Depth from the depth buffer                          : [0, 1] (typically reversed)
    //     float  linearDepth; // View space Z coordinate                              : [Near, Far]
    // };

    // To sample custom buffers, you have access to these functions:
    // But be careful, on most platforms you can't sample to the bound color buffer. It means that you
    // can't use the SampleCustomColor when the pass color buffer is set to custom (and same for camera the buffer).
    // float4 CustomPassSampleCustomColor(float2 uv);
    // float4 CustomPassLoadCustomColor(uint2 pixelCoords);
    // float LoadCustomDepth(uint2 pixelCoords);
    // float SampleCustomDepth(float2 uv);

    // There are also a lot of utility function you can use inside Common.hlsl and Color.hlsl,
    // you can check them out in the source code of the core SRP package.

float3 rgb2hsv(float3 rgb)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(rgb.bg, K.wz), float4(rgb.gb, K.xy), step(rgb.b, rgb.g));
    float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 hsv)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(hsv.rrr + K.xyz) * 6 - K.www);
    return hsv.z * lerp(K.rrr, clamp(p - K.rrr, 0.0, 1.0), hsv.y);
}
float posterize(float val, float steps)
{
    return round(val * steps) / steps;
}

float4 PixelizationEffect(Varyings varyings)
{
    float2 uv = float2(posterize(varyings.positionCS.x, _PixelizationResolution), posterize(varyings.positionCS.y, _PixelizationResolution));
    return float4(CustomPassLoadCameraColor(uv, 0), 1);
    
}

float4 SmoothQuantEffect(Varyings varyings, float4 color)
{
    float3 hsv = rgb2hsv(color.xyz);
    hsv.z = posterize(hsv.z, _SmoothQuantizationSteps);
    float3 rgb = hsv2rgb(hsv);
    return float4(rgb, 1);
}

float4 RoughPosterizationEffect(Varyings varyings, float4 color)
{
    return float4(posterize(color.r, _RoughQuantizationSteps), posterize(color.g, _RoughQuantizationSteps), posterize(color.b, _RoughQuantizationSteps), 1);
}

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
    float4 color = _PixelizationIntensity >= 1 ? PixelizationEffect(varyings) : float4(CustomPassLoadCameraColor(varyings.positionCS.xy, 0), 1);
    if (_SmoothQuantizationIntensity >= 1)
    {
        color = SmoothQuantEffect(varyings, color);
    }
    if (_RoughQuantizationIntensity >= 1)
    {
        color = RoughPosterizationEffect(varyings, color);
    }
    return color;
        
    }

    

    


    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Custom Pass 0"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off

}
