Shader "FullScreen/borderPost"
{
    Properties
    {
        _Thickness("Thickness", Range(0.1, 10)) = 1
        _NormalThreshold("Normals", Range(0, 1)) = 0.5
        _DepthThreshold("Depth", Range(0, 10)) = 0.5
        _BorderColor("BorderColor", Color) =  (0, 0, 0, 1)
        
    }
    HLSLINCLUDE
    
float _Thickness;
    float _Steps;
    float _NormalThreshold;
    float _DepthThreshold;
    float4 _BorderColor;

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


bool compareNormals(float3 normal1, float3 normal2)
{
    return dot(normal1, normal2) < _NormalThreshold;
}

bool compareDepth(float depth1, float deph2)
{
    return abs(depth1 - deph2) > _DepthThreshold;
}

float getLinearDepth(Varyings varyings, float2 offset)
{
    float2 uv = varyings.positionCS.xy + offset;
    float depth = LoadCameraDepth(uv);
    PositionInputs posInput = GetPositionInput(uv, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
    return posInput.linearDepth;

}

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        
    
        
        
        //float3 up = LoadNormalWS(varyings.positionCS.xy - float2(1, 0));
        //float3 down = LoadNormalWS(varyings.positionCS.xy + float2(1, 0));
        //float3 rigth = LoadNormalWS(varyings.positionCS.xy - float2(0, 1));
        //float3 left = LoadNormalWS(varyings.positionCS.xy + float2(0, 1));
        //bool border = compareNormals(normalWS, down) || compareNormals(normalWS, rigth) || compareNormals(normalWS, left) || compareNormals(normalWS, up);
    float depth = getLinearDepth(varyings, float2(0, 0));
    
    bool border = compareDepth(depth, getLinearDepth(varyings, float2(_Thickness, 0))) || compareDepth(depth, getLinearDepth(varyings, float2(-_Thickness, 0))) || compareDepth(depth, getLinearDepth(varyings, float2(0, _Thickness))) || compareDepth(depth, getLinearDepth(varyings, float2(0, -_Thickness)));;
    float3 color = CustomPassLoadCustomColor(varyings.positionCS.xy);
        return border ? _BorderColor : float4(color, 1);
}

    

    

    


    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Custom Pass 1"

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
