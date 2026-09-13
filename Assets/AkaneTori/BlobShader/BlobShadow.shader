Shader "AkaneShader/BlobShadow"
{
    Properties
    {
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.6)
        _Radius ("Radius", Float) = 1.0
        _Softness ("Softness", Range(0.01, 1.0)) = 0.3
        _Falloff ("Falloff", Range(0.01, 5.0)) = 2.0
        _ShadowSpread ("Shadow Spread", Range(1.0, 2.0)) = 1.0
        _MaxHeight ("Max Height for Fade", Float) = 3.0
        _CharacterPosition ("Character World Position", Vector) = (0, 0, 0, 0)
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
            Name "BlobShadow"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Offset -1, -1

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            #define MAX_CHARACTERS 16 //最大キャラクター数

            CBUFFER_START(UnityPerMaterial)
            half4 _ShadowColor;
            float _Radius;
            float _Softness;
            float _Falloff;
            float _MaxHeight;
            float _ShadowSpread;
            float4 _CharacterPositions[MAX_CHARACTERS];
            int _CharacterCount;
            CBUFFER_END

            float CalcShadow(float3 posWS, float4 charPos){
                //現在のキャラの高さを割合にする
                float heightRatio = saturate(charPos.y / _MaxHeight);

                //キャラの高さの割合に応じて影の広がり具合を計算
                float radius = lerp(_Radius, _Radius * _ShadowSpread, heightRatio);

                //キャラの位置からの距離を計算
                float2 center = posWS.xz - charPos.xz;
                float dist = length(center);

                //影のぼかし
                float shadow = 1.0 - smoothstep(radius - _Softness, radius, dist);//端
                //中心が1、端が0
                shadow *= pow(saturate(1.0 - dist / radius), _Falloff);//全体

                //キャラの高さに応じて影の濃さを決める
                shadow *= 1.0 - heightRatio;

                return shadow;
            }

            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionWS  = TransformObjectToWorld(i.positionOS.xyz);
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }

            //丸影を落とす処理
            half4 frag(Varyings i) : SV_Target
            {
                float shadow = 0;

                for (int idx = 0; idx < _CharacterCount; ++idx)
                {
                    float s = CalcShadow(i.positionWS, _CharacterPositions[idx]);
                    shadow = max(shadow, s); //影が重なった部分は濃い方を採用
                }

                //影の色と透明度を設定する
                half4 color = _ShadowColor;
                color.a *= shadow;

                return color;
            }
            ENDHLSL
        }
    }
}