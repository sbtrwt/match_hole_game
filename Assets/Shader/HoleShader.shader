Shader "Custom/HoleShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HoleMask("Hole Mask", 2D) = "white" {}
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        sampler2D _HoleMask;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_HoleMask;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 mask = tex2D(_HoleMask, IN.uv_HoleMask);
            o.Albedo = c.rgb;
            o.Alpha = mask.r;
        }
        ENDCG
    }
        FallBack "Diffuse"
}