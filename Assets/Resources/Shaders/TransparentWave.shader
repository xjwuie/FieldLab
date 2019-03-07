Shader "Custom/TransparentWave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		//_FlowDir("Flow Direction", Vector) = (1, 0, 0, 0)
		//_FlowSpeed("Flow Speed", Range(0, 2)) = 1
		_Frequency("Wave Frequency", Range(0, 10)) = 5


		_AlphaScale("Alpha Scale", Range(0, 1)) = 0.5
		_MinAlpha("Min Alpha", Range(0, 0.8)) = 0.2
		_AttenFactor("Attenuation Factor", Range(1, 10)) = 2

	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent"
		"IgnoreProjector" = "True"
		"Queue" = "Transparent" }
		LOD 100

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float3 worldNormal : TEXCOORD1;
		float3 worldPos : TEXCOORD2;
	};

	sampler2D _MainTex;
	fixed4 _Color;
	float4 _MainTex_ST;
	//float4 _FlowDir;
	float _AlphaScale;
	//float _FlowSpeed;
	float _Frequency;
	float _MinAlpha;
	float _AttenFactor;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		//o.uv += frac(_Time.y * _FlowSpeed * float2(_FlowDir.x, _FlowDir.y));
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float tmpX, tmpY;
		tmpX = i.uv.x * 2 - 1;
		tmpY = i.uv.y * 2 - 1;
		float degOffset = atan(tmpY / tmpX) +radians(180) * step(tmpX, 0);

		float dis = distance(i.uv, float2(0.5, 0.5));
		float alphaMul = sin(_Frequency * _Time.y + dis * 50 + degOffset);
		
		tmpY = 2 / (1 - _MinAlpha);		
		tmpX = tmpY - 1;
		alphaMul = (alphaMul + tmpX) / tmpY;

		float disFactor = 1 - dis / 2 * _AttenFactor;
		disFactor *= disFactor;
		//disFactor *= disFactor;

		fixed3 worldNormal = normalize(i.worldNormal);
		fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

		fixed4 texCol = tex2D(_MainTex, i.uv);
		fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * texCol.xyz;
		fixed3 diffuse = _LightColor0.rgb * texCol.xyz * max(0, dot(worldNormal, worldLightDir));
		fixed4 col = fixed4(ambient + diffuse, texCol.a) * _Color;
		col.a *= _AlphaScale * alphaMul * disFactor;
		return col;
	}
		ENDCG
	}
	}

		Fallback "Transparent/VertexLit"
}
