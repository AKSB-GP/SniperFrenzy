Shader "Custom/CelShading"
{
    Properties{
        [Header(Color)]
        _Color("Base color", Color) = (1, 0, 1, 1)
        _MainTex ("Main tex", 2D) = "white" {}
        [HDR]_AmbientColor("Ambient color", Color) = (0.1, 0, .01, 1)
        [HDR]_SpecularColor("Specular Color", Color) = (0.9, 0.9, 0.9, 1)
        [HDR]_RimColor("Rim color", Color) = (1, 0, 1, 1)
        _Glossiness("Glossiness", Range(0.0, 1)) = 0.5
        [Header(Rim settings)]
        _RimAmount("Rim amount", Range(0.0, 1.0)) = 0.5
        [Header(Band cutoffs)]
        _bandCutOff("Band cutoff", Range(0.0, 1.0)) = 0.5
        _bandAmount("Band amount", int) = 4
        [HDR]_Emission("Emission", Color) = (0, 0, 0, 1)

    }
    SubShader {

        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        pass {
            Name "Celshading"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //to make the shader srp batcher compatible, a thing for URP, not needed in BRP
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            half4 _AmbientColor;
            half _Glossiness;
            half4 _SpecularColor;
            half _LightIntensity;
            half _RimAmount;
            half4 _RimColor;
            half _RimStrength;
            half _bandCutOff;
            half _bandAmount;
            half4 _Emission;
            sampler2D _MainTex;
            CBUFFER_END

            struct Attributes {
                float4 positionLS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalsLS : NORMAL;

            };

            struct Varyings{
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalsWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };

            //Vertex shader
            Varyings Vert(Attributes o){
                Varyings output;
                //object space relative to mesh to the 2d screen coordinate
                output.positionCS = TransformObjectToHClip(o.positionLS.xyz);
                output.uv = o.uv;
                output.normalsWS = TransformObjectToWorldNormal(o.normalsLS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(o.positionLS.xyz));
                output.positionWS = TransformObjectToWorld(o.positionLS.xyz);
                return output;
            }
            //Fragment shader
            half4 Frag(Varyings v) : SV_Target{
                //Mainlight info
                Light lightdata = GetMainLight();
                float3 lightDir = normalize(lightdata.direction);
                float3 lightColor = lightdata.color;
                //Albedo texture
                float3 albedoTex = tex2D(_MainTex, v.uv).rgb;
                //base calculations
                float3 normal = normalize(v.normalsWS);
                float n_dot_L = saturate(dot(lightDir, normal));
                //bands
                float diffuse = round(saturate(n_dot_L / _bandCutOff) * _bandAmount) / _bandAmount;
                //shadow
                float shadowAtten = MainLightRealtimeShadow(TransformWorldToShadowCoord(v.positionWS));
                shadowAtten = step(0.5, shadowAtten);
                float shadow = diffuse * shadowAtten;
                //specular :
                float3 viewDirWS = normalize(v.viewDirWS);
                float3 halfVector = reflect(- lightDir, normal);
                float h_dot_v = dot(viewDirWS, halfVector);
                float specular = _SpecularColor.rgb * step(1 - _Glossiness, h_dot_v);

                //rim light / outline
                float rimDot = 1 - saturate(dot(viewDirWS, normal));
                float rimAmount = step(1 - _RimAmount, rimDot);
                float3 rimLight = rimAmount * _RimColor.rgb*n_dot_L;
                
                //finalcolor, it is more common correct to multiply the albedo texture, however addition here is done for visual reasons
                float3 finalColor = (_Color.rgb + albedoTex + specular) * lightColor * _AmbientColor * _Emission.rgb * shadow + rimLight;
                return half4(finalColor, 1);

            }
            ENDHLSL
        }
        pass {
            Name "Shadowcast"
            Tags { "LightMode" = "ShadowCaster" }
            //define actualy hlsl code for render logic
            //We tell that we change from Shaderlab to HLSL
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //to make the shader srp batcher compatible, a thing for URP, not needed in BRP

            struct Attributes {
                float4 positionLS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalsLS : NORMAL;

            };

            struct Varyings{
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalsWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };

            //Vertex shader
            Varyings Vert(Attributes o){
                Varyings output;
                output.positionCS = TransformObjectToHClip(o.positionLS.xyz);
                output.uv = o.uv;
                output.normalsWS = TransformObjectToWorldNormal(o.normalsLS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(o.positionLS.xyz));
                output.positionWS = TransformObjectToWorld(o.positionLS.xyz);
                VertexPositionInputs positions = GetVertexPositionInputs(o.positionLS.xyz);
                return output;
            }
            //Fragment shader
            half4 Frag(Varyings v) : SV_Target{
                //shadow calcs
                float shadowAtten = MainLightRealtimeShadow(TransformWorldToShadowCoord(v.positionWS));
                float shadow = step(0.5, shadowAtten);
                return 0;

            }
            ENDHLSL
        }
    }
}
