float4x4 World;
float4x4 View;
float4x4 Projection;
float3 DiffuseColor;
float Alpha;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : SV_POSITION;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET0
{
    // TODO: add your pixel shader code here.
	float4 color = float4(0.f, 0.f, 0.f, 0.f);
	color.rgb = DiffuseColor;
	color.a = Alpha;

    return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

		#if SM4
			VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
			PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
		#elif SM3
			VertexShader = compile vs_3_0 VertexShaderFunction();
			PixelShader = compile ps_3_0 PixelShaderFunction();
		#else
			VertexShader = compile vs_2_0 VertexShaderFunction();
			PixelShader = compile ps_2_0 PixelShaderFunction();
		#endif
    }
}
