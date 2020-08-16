sampler mainTex;
float2 strongPoint;
float frequency;
float amplitude;
float totalTimeMs;
float timerMs;


float4 frag(float2 pos : TEXCOORD0) : COLOR0
{
    float dist = distance(pos, strongPoint);
    float strength = (1 - dist) * ((totalTimeMs - timerMs) / totalTimeMs);
    float distortion = sin(pos.y * frequency * timerMs) * (amplitude * strength);
    return tex2D(mainTex, float2(pos.x + distortion, pos.y));
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 frag();
    }
}