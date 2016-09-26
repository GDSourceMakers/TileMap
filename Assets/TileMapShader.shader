Shader "TileMap/Shader"
{
	Properties
	{
		_MainTex("Texture", 2DArray) = "white" {}
		_MissingTex("Missing Tex", 2D) = "white" {}
		//_AtlasMap ("AtlasMap", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
		 Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 atlasMap : COLOR;
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 atlasMap : COLOR;
			};

			UNITY_DECLARE_TEX2DARRAY(_MainTex);
			sampler2D _MissingTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex = UnityPixelSnap(o.vertex);
				o.uv.x = v.uv.x;
				o.uv.y = v.uv.y;
				o.uv.z = 0;
				o.atlasMap = v.atlasMap;
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f input) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

				float3 uvCord;
				uvCord = input.uv;

				float4 color;

				if(input.atlasMap.x < 1)
				{
					uvCord.z = input.atlasMap.x;

					color = UNITY_SAMPLE_TEX2DARRAY_LOD(_MainTex, uvCord, 0);
				}
				else
				{
					color = tex2D(_MissingTex, input.uv);
				}

				//color = color * input.atlasMap.x;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return color;
			}
			ENDCG
		}
	}
}
