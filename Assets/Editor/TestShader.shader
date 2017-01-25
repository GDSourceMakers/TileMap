Shader "Test/Test"
{
	Properties
	{
		_MainTex("Texture", 2DArray) = "white" {}
		_MissingTex("Missing Tex", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		LOD 100

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 atlasMap : TEXCOORD1;
				};

				struct v2f
				{
					float3 uv : TEXCOORD0;
					float3 uv1 : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

					UNITY_DECLARE_TEX2DARRAY(_MainTex);
					sampler2D _MissingTex;
					float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.vertex = UnityPixelSnap(o.vertex);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv.x = v.uv.x;
					o.uv.y = v.uv.y;

					return o;
				}

				fixed4 frag(v2f input) : SV_Target
				{		
					float4 color;

					if (input.uv.z == 255)
					{
						color = tex2D(_MissingTex, input.uv);
						//color = float4(0, 0, 0, 0);
					}
					else
					{
						color = UNITY_SAMPLE_TEX2DARRAY(_MainTex, input.uv);
					}

					return color;
				}
			ENDCG
		}
	}
}
