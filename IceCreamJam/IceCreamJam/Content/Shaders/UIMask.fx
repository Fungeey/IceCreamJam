sampler s0;
float Progress;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
    if (coords.y < 75 * (1-coords.x-(Progress) + 0.01))
        return tex2D(s0, coords);
    else
        return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}