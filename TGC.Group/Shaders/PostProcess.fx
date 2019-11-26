//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture texCasco;
sampler2D cascoMap = sampler_state
{
	Texture = (texCasco);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float screen_dx;					// tamaño de la pantalla en pixels
float screen_dy;
float time;

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float3 Normal :   NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

texture g_RenderTarget;
sampler RenderTarget =
sampler_state
{
	Texture = <g_RenderTarget>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
    float3 Normal :          TEXCOORD1; // Normales
};

void vs_main(float4 vPos : POSITION, float2 vTex : TEXCOORD0, out float4 oPos : POSITION, out float2 oScreenPos : TEXCOORD0)
{
	oPos = vPos;
	oScreenPos = vTex;
	oPos.w = 1;
}

float Frequency = 10;
float Phase = 0;
float Amplitude = 0.1;
bool withoutHelmet = true;
bool inWater = false;
float4 ps_main(in float2 Tex : TEXCOORD0, in float2 vpos : VPOS) : COLOR0
{	
	//Move to another affect
	float2 cord = Tex;
	if (withoutHelmet)
	{
		if (inWater)
		{
			cord.x += sin(cord.y * Frequency + Phase + time) * Amplitude;
		}

		return tex2D(RenderTarget, cord);
	}
	else
	{
		float4 texelCasco = tex2D(cascoMap, cord);
		if (texelCasco.a < 1)
		{
			//All pixels with transparency. Middle of helmet
			return tex2D(RenderTarget, cord);
		}
		else
		{
			//Helmet texture
			return texelCasco;
		}
	}
}

technique PostProcess
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_main();
	}
}