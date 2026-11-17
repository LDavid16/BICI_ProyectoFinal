Shader "Custom/PintarSoloCuadro"
{
    Properties
    {
        _Color ("Color Elegido", Color) = (1,1,1,1)
        _MainTex ("Textura Original (Albedo)", 2D) = "white" {}
        _ColorBaseBici ("Color Original del Cuadro", Color) = (0.78, 0.86, 0.90, 1)
        _Tolerancia ("Tolerancia de Deteccion", Range(0.05, 0.8)) = 0.28
        _Glossiness ("Suavidad / Brillo", Range(0,1)) = 0.5
        _Metallic ("Metalico", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _ColorBaseBici;
        half _Tolerancia;
        half _Glossiness;
        half _Metallic;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            
            // Calcula qué tan parecido es el píxel al color original del cuadro
            float dist = distance(tex.rgb, _ColorBaseBici.rgb);
            float mascara = 1.0 - smoothstep(_Tolerancia * 0.6, _Tolerancia, dist);

            // Calcula brillo de la textura para conservar sombras y reflejos
            float luminancia = dot(tex.rgb, float3(0.299, 0.587, 0.114));
            fixed3 colorPintado = _Color.rgb * saturate(luminancia * 1.15);

            // Mezcla: solo pinta donde coincide con el cuadro; el resto queda intacto
            o.Albedo = lerp(tex.rgb, colorPintado, mascara);
            o.Metallic = _Metallic * mascara;
            o.Smoothness = _Glossiness;
            o.Alpha = tex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}