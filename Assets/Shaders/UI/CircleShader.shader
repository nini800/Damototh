// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CircleDrawer"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Circle Color", Color) = (1,1,1,1)
		_ColorIntensity("Color Intensity", Float) = 1
		_CenterOffset("Center Offset", Vector) = (0, 0, 0, 0)
		_Radius("Radius", Range(0.001, 0.5)) = 0.25
		_Width("Width", Range(0.001, 0.5)) = 0.05
		_Fill("Fill", Range(0, 1)) = 1
		_StartAngle("Start Angle", Range(0, 360)) = 0
		[MaterialToggle] _Clockwise("Clockwise", Float) = 0

		_AADist("Anti-Aliasing Distance", Range(0, 0.25)) = 0.000371
		_AALin("Anti-Aliasing Linearity", Range(0.01, 5)) = 1

		_Divider("Divider for Flat Smooth", Range(0.001, 5)) = 1


	}
	CGINCLUDE

		float Abs(float value)
		{
			if (value < 0)
				return -value;
			else
				return value;
		}
		float Limit(float value, float max)
		{
			if (value > max)
				return max;
			else
				return value;
		}
	ENDCG

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Transparent+1"
		}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON

			sampler2D _MainTex;
			float4 _Color;
			float _ColorIntensity;
			float2 _CenterOffset;
			float _Radius;
			float _Width;
			float _Fill;
			float _StartAngle;
			float _Clockwise;
			float _AADist;
			float _AALin;
			float _Divider;

			struct Vertex
			{
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct Fragment
			{
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			Fragment vert(Vertex v)
			{
				Fragment o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = v.uv_MainTex;
				o.uv2 = v.uv2;

				return o;
			}

			float4 frag(Fragment IN) : COLOR
			{
				float4 o = _Color;

				float radToDeg = 57.2957795130823;
				half4 c = tex2D(_MainTex, IN.uv_MainTex);
				
				float2 trig = normalize(IN.uv_MainTex - float2(0.5, 0.5) + _CenterOffset);
				float dist = sqrt((IN.uv_MainTex.x - 0.5f + _CenterOffset.x) * (IN.uv_MainTex.x - 0.5f + _CenterOffset.x) + (IN.uv_MainTex.y - 0.5f + _CenterOffset.y) * (IN.uv_MainTex.y - 0.5f + _CenterOffset.y));
				float angle = trig.y >= 0 ? acos(trig.x) * radToDeg : -acos(trig.x) * radToDeg + 360;
				
				angle -= _StartAngle;

				if (angle < 0)
					angle = 360 + angle;

				if (_Clockwise == 1)
					angle = 360 - angle;

				if (angle / 360 <= _Fill)
				{
					float minima = _Radius - _Width / 2;
					float maxima = _Radius + _Width / 2;


					if (dist < maxima && dist > minima)//If we are in the circle
					{
						o.a *= 1;
					}
					else if (dist < maxima + _AADist && dist > minima - _AADist)//If we are in the circle borders
					{
						if (dist >= maxima)
							o.a *= pow(((maxima + _AADist) - (dist)) / _AADist, _AALin);
						else
							o.a *= pow(((dist)-(minima - _AADist)) / _AADist, _AALin);
					}
					else
						o.a = 0;
				}
				else if (angle / 360 <= _Fill + _AADist / _Divider && _Fill > 0 || (angle - 360) / 360 >= -_AADist / _Divider && _Fill > 0)
				{

					float minima = _Radius - _Width / 2;
					float maxima = _Radius + _Width / 2;


					if (dist < maxima + _AADist && dist > minima - _AADist)//If we are in the circle borders
					{
						float endCaseLin = ((_Fill + _AADist / _Divider - angle / 360) / (_AADist / _Divider));
						float beginCaseLin = ((_AADist / _Divider - Abs((angle - 360) / 360)) / (_AADist / _Divider));

						float endCaseFlat = pow(endCaseLin, _AALin);
						float beginCaseFlat = pow(beginCaseLin, _AALin);

						float endCaseUpper = pow(((maxima + _AADist) - (dist)) / _AADist * endCaseLin, _AALin);
						float beginCaseUpper = pow(((maxima + _AADist) - (dist)) / _AADist * beginCaseLin, _AALin);
						float endCaseLower = pow(((dist)-(minima - _AADist)) / _AADist * endCaseLin, _AALin);
						float beginCaseLower = pow(((dist)-(minima - _AADist)) / _AADist * beginCaseLin, _AALin);


						if (angle / 360 <= _Fill + (_AADist / _Divider) && (angle - 360) / 360 >= -(_AADist / _Divider) && _Fill > 0)
						{
							if (dist >= maxima)
								o.a *= Limit(endCaseUpper + beginCaseUpper, pow(((maxima + _AADist) - (dist)) / (_AADist), _AALin));
							else if (dist <= minima)
								o.a *= Limit(endCaseLower + beginCaseLower, pow(((dist)-(minima - _AADist)) / (_AADist), _AALin));
							else
								o.a *= endCaseFlat + beginCaseFlat;
						}
						else if (angle / 360 <= _Fill + (_AADist / _Divider) && _Fill)
						{
							if (dist >= maxima)
								o.a *= endCaseUpper;
							else if (dist <= minima)
								o.a *= endCaseLower;
							else
								o.a *= endCaseFlat;
						}
						else if ((angle - 360) / 360 >= -(_AADist / _Divider) && _Fill > 0)
						{
							if (dist >= maxima)
								o.a *= beginCaseUpper;
							else if (dist <= minima)
								o.a *= beginCaseLower;
							else
								o.a *= beginCaseFlat;
						}
						else
							o.a = 0;
					}
					else
						o.a = 0;



				}
				else
					o.a = 0;

				

				return o * _ColorIntensity;
			}



			ENDCG
		}
	}
}