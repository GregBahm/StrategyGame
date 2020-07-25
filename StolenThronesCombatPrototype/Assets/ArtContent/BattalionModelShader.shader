
Shader "Unlit/BattalionModelShader"
{
    Properties
    {
        _HighColor("High Color", Color) = (1,1,1,1)
        _SpecColor("Spec Color", Color) = (1,1,1,1)
        _SubsurfaceColor("Subsurface Color", Color) = (1,1,1,1)
        _LowColor("Low Color", Color) = (1,1,1,1)
        _Ramp("Ramp", Float) = 1
    }
    SubShader
    {
        Tags { "LightMode" = "ForwardBase"}
        LOD 100

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "AutoLight.cginc"

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
                float3 displayVolumeSpace : TEXCOORD1;
                float3 normal : NORMAL;
                float3 viewDir : VIEWDIR;
                float4 _ShadowCoord : TEXCOORD2;
            };

            float3 _HighColor;
            float3 _LowColor;
            float3 _SpecColor;
            float3 _SubsurfaceColor;
            float _Ramp;

            float4x4 _DisplayVolume;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 worldSpace = mul(unity_ObjectToWorld, v.vertex);
                o.displayVolumeSpace = mul(_DisplayVolume, float4(worldSpace, 1));
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);

                o._ShadowCoord = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 norm = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float3 halfAngle = normalize(viewDir + float3(0, 1, 0));
                float specBase = dot(norm, halfAngle);

                float param = pow(saturate(i.displayVolumeSpace.y * .5), _Ramp);
                float3 baseCol = lerp(_HighColor, _LowColor, param);
                float normLight = (norm.y + 1) * .5;
                normLight = pow(normLight, .5);
                float3 col = baseCol + normLight * .2;


                float specA = pow(saturate(specBase), 5);
                float specB = pow(saturate(specBase), 200);
                col = lerp(col, pow(col, 2), specA * .3);
                col = lerp(col, _SpecColor, saturate(specB * 5) * .2);

                float shadowness = SHADOW_ATTENUATION(i);
                
                float3 shadowColor = _LowColor;
                col = lerp(baseCol, col, shadowness);

                col = lerp(_HighColor, col, param * 2);

                return float4(col, 1);
            }
            ENDCG
        }
    }
}
