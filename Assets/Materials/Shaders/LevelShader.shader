Shader "Custom/NeonGrid_Advanced"
{
    Properties
    {
        [Header(Colors)]
        [HDR]_Color("Neon Color", Color) = (1, 0, 1, 1)
        [HDR]_FaceColor("Face Color", Color) = (0.05, 0.05, 0.05, 1)

        [Header(Grid Controls)]
        _GridScale("Grid Scale", Float) = 5
        _LineWidth("Line Width", Range(0.0, 0.5)) = 0.05
        _Softness("Edge Softness", Range(0.0, 0.2)) = 0.02
        [Header(vertex settings)]

        _VertexSize("Vertex Size", Range(0.0, 0.5)) = 0.05
        _VertexIntensity("Vertex Glow Intensity", Float) = 1.0

        [Header(Glow Controls)]
        _GlowIntensity("Line Glow Intensity", Float) = 3.0
        _LineGlowBoost("Extra Glow on Lines", Float) = 2.0

        [Header(Face Fill)]
        _FaceBrightness("Face Brightness", Range(0, 1)) = 0.0 
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "NeonGridPass"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _FaceColor;
            float _GridScale;
            float _LineWidth;
            float _Softness;
            float _VertexSize;
            float _VertexIntensity;
            float _GlowIntensity;
            float _LineGlowBoost;
            float _FaceBrightness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv * _GridScale;
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float2 f = frac(i.uv);

                // take the minimum of the distance between x and y 
                float dx = min(f.x, 1.0 - f.x);
                float dy = min(f.y, 1.0 - f.y);
                float d  = min(dx, dy);

                //  mask base on linewidth
                float edgeMask = 1.0 - smoothstep(_LineWidth, _LineWidth + _Softness, d);
                float vx = 1.0 - smoothstep(_VertexSize, _VertexSize + _Softness, dx);
                float vy = 1.0 - smoothstep(_VertexSize, _VertexSize + _Softness, dy);
                float vertexGlow = vx * vy * _VertexIntensity;

                // Glow calcs
                float baseGlow = edgeMask + vertexGlow;
                float glowBoost = pow(edgeMask, 1.0) * _LineGlowBoost;
                float glowFactor = baseGlow + glowBoost;
                float3 lineColor = _Color.rgb * glowFactor * _GlowIntensity;

                //face and final color
                float3 faceColor = _FaceColor.rgb * _FaceBrightness;
                float3 finalColor = lerp(faceColor, lineColor, saturate(baseGlow));

                // alpha is of highest max values between baseglow and face color
                float alpha = saturate(max(baseGlow, _FaceBrightness * _FaceColor.a));
                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
