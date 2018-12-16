Shader "Custom/Standard"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_BumpMap ("Bump Map", 2D) = "bump" {}
		_DivX ("Divide X", Float) = 1
		_DivY ("Divide Y", Float) = 1
		_Specular ("Specular Color", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(1, 256)) = 8 
		_SpecularRange("If Specular", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags{"LightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fwdbase
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				SHADOW_COORDS(1)
				float4 T2W0 : TEXCOORD2;
				float4 T2W1 : TEXCOORD3;
				float4 T2W2 : TEXCOORD4;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			fixed4 _Color;
			fixed _DivX;
			fixed _DivY;
			fixed4 _Specular;
			float _Gloss;
			float _SpecularRange;

			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
				o.uv.xy *= fixed2(_DivX, _DivY);
				o.uv.zw *= fixed2(_DivX, _DivY);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.T2W0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.T2W1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.T2W2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				TRANSFER_SHADOW(o);

				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldPos = fixed3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 halfDir = normalize(worldLightDir + worldViewDir);
				fixed3 albedo = _Color.rgb * tex2D(_MainTex, i.uv.xy);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				

				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				bump = fixed3(dot(bump, i.T2W0.xyz), dot(bump, i.T2W1.xyz), dot(bump, i.T2W2.xyz));
				fixed3 diffuse = albedo * _LightColor0.rgb * saturate(dot(worldLightDir, bump));

				fixed3 specular = _Specular.rgb * _LightColor0.rgb * pow(saturate(dot(bump, halfDir)), _Gloss) * _SpecularRange;

				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);
				//fixed3 atten = SHADOW_ATTENUATION(i);
				return fixed4(ambient + (specular + diffuse) * atten, 1.0);
			}
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ForwardAdd" }
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
						// make fog work
			#pragma multi_compile_fwdadd
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#include "UnityCG.cginc"

			struct a2v
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				SHADOW_COORDS(1)
				float4 T2W0 : TEXCOORD2;
				float4 T2W1 : TEXCOORD3;
				float4 T2W2 : TEXCOORD4;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			fixed4 _Color;
			fixed _DivX;
			fixed _DivY;


			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
				o.uv.xy *= fixed2(_DivX, _DivY);
				o.uv.zw *= fixed2(_DivX, _DivY);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.T2W0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.T2W1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.T2W2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				TRANSFER_SHADOW(o);


				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 worldPos = fixed3(i.T2W0.w, i.T2W1.w, i.T2W2.w);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 albedo = _Color.rgb * tex2D(_MainTex, i.uv.xy);
				//fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				bump = fixed3(dot(bump, i.T2W0.xyz), dot(bump, i.T2W1.xyz), dot(bump, i.T2W2.xyz));
				fixed3 diffuse = albedo * _LightColor0.rgb * saturate(dot(worldLightDir, bump));
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);
				//fixed3 atten = SHADOW_ATTENUATION(i);
				return fixed4(diffuse * atten, 1.0);
			}
			ENDCG
		}
	}
			Fallback "VertexLit"
}
