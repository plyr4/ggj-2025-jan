// UNITY_SHADER_NO_UPGRADE
#ifndef UTILS_INCLUDED
#define UTILS_INCLUDED

float4 ballSDF(float2 uv, float2 pos, float radius, float4 color)
{
    return (radius / length(uv - pos)) * color;
}

float4 squareSDF(float2 uv, float2 pos, float sideLength, float4 color)
{
    // Calculate the half-side length of the square
    float halfSide = sideLength * 0.5;
    
    // Compute the distances to the edges of the square
    float2 diff = abs(uv - pos);
    float maxDist = max(diff.x, diff.y);

    // Normalize the distance based on the square's half-side length
    float gradientValue = halfSide / maxDist;

    // Return the color scaled by the gradient value
    return gradientValue * color;
}


void bubbles_float(UnityTexture2D _Bubbles, UnitySamplerState ss, int _BubbleCount, float2 _UV, float4 _Color,
                   out float4 Out)
{
    Out = float4(0.0, 0.0, 0.0, 0.0);
    float aspectRatio = 16.0 / 9.0;

    for (int i = 0; i < _BubbleCount; i++)
    {
        int row = i / 32;
        int col = i % 32;
        float2 bubbleUV = float2((col + 0.5) / 32.0, (row + 0.5) / 32.0);
        float4 bubble = _Bubbles.Sample(ss, bubbleUV);

        float x = bubble.x;
        float y = bubble.y;

        float2 pos = float2(x, y);
        float r = clamp(bubble.z, 0, 0.05);

        float2 uv = float2(aspectRatio * _UV.x, _UV.y);
        float2 ballPos = float2(aspectRatio * pos.x, pos.y);

        // Out += squareSDF(uv, ballPos, r, _Color);
        Out += ballSDF(uv, ballPos, r, _Color);
    }
}
#endif
