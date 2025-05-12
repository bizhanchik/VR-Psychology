Shader "Custom/BlackHole"
{
    Properties
    {
        // Main Settings
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color (Inner color of the black hole)", Color) = (0,0,0,1)
        
        // Distortion Settings
        _Distortion ("Distortion (Strength of space distortion)", Range(0,1)) = 0.5
        _EventHorizonRadius ("Event Horizon Radius (Black hole boundary)", Range(0,1)) = 0.3
        
        // Glow Settings
        _RimColor ("Rim Color (Edge glow color)", Color) = (1,0.5,0,1)
        _RimPower ("Rim Power (Edge glow strength)", Range(0,10)) = 3
        
        // Gravitational Lensing Settings
        _GravitationalLensing ("Gravitational Lensing (Light distortion)", Range(0,2)) = 1
        _TimeWarp ("Time Warp (Time dilation effect)", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Distortion;
            float _EventHorizonRadius;
            float4 _Color;
            float4 _RimColor;
            float _RimPower;
            float _GravitationalLensing;
            float _TimeWarp;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 offset = i.uv - center;
                float dist = length(offset);
                
                // Эффект гравитационного линзирования
                float3 viewDir = normalize(i.viewDir);
                float3 normal = normalize(i.normal);
                float rim = 1.0 - abs(dot(viewDir, normal));
                rim = pow(rim, _RimPower);
                
                // Искажение UV координат для создания эффекта втягивания
                float2 distortedUV = center;
                float distortionFactor = smoothstep(_EventHorizonRadius, 1.0, dist);
                
                // Усиление эффекта гравитационного линзирования
                float lensingEffect = _GravitationalLensing * (1.0 - distortionFactor);
                distortedUV += offset * (1.0 + _Distortion * (1.0 - distortionFactor) + lensingEffect);
                
                // Эффект горизонта событий
                float eventHorizon = smoothstep(_EventHorizonRadius - 0.1, _EventHorizonRadius, dist);
                float blackHoleCore = smoothstep(_EventHorizonRadius - 0.2, _EventHorizonRadius - 0.1, dist);
                
                // Эффект замедления времени
                float timeWarp = smoothstep(0.0, _EventHorizonRadius, dist);
                float timeDistortion = lerp(1.0, 0.5, _TimeWarp * (1.0 - timeWarp));
                
                // Смешивание цветов
                fixed4 col = tex2D(_MainTex, distortedUV);
                col.rgb = lerp(_Color.rgb, col.rgb, distortionFactor);
                
                // Добавление свечения по краям
                float3 rimColor = _RimColor.rgb * rim;
                col.rgb = lerp(col.rgb, rimColor, rim * 0.5);
                
                // Альфа-канал с учетом эффектов
                col.a = lerp(1.0, 0.0, blackHoleCore);
                col.a = max(col.a, rim * 0.5);
                
                // Применение эффекта замедления времени
                col.rgb *= timeDistortion;
                
                return col;
            }
            ENDCG
        }
    }
} 