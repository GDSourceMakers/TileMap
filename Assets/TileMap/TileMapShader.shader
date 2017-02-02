Shader "TileMap/Shader"
{
	Properties
	{
		_MainTex("Texture", 2DArray) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
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
			#pragma target 3.5
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex	: POSITION;
				float2 uv		: TEXCOORD0;
				float4 atlasMap : TEXCOORD1;
				float4 color    : COLOR;
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;
				fixed4 color    : COLOR;
				float4 vertex : SV_POSITION;
			};

			UNITY_DECLARE_TEX2DARRAY(_MainTex);
			sampler2D _MissingTex;
			float4 _MainTex_ST;

			fixed4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex = UnityPixelSnap(o.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv.x = v.uv.x;
				o.uv.y = v.uv.y;
				o.uv.z = v.atlasMap.x;

				o.color = v.color * _Color;

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
					
					//color.r = 255;
					//color.b = 255;
					
					//color.a = 255;
					color = UNITY_SAMPLE_TEX2DARRAY(_MainTex, input.uv) * input.color;
					color.rgb *= color.a;
					//color.g = 255;

					//color.a = 255;
				}

				return color;
			}
			ENDCG
		}
	}
}