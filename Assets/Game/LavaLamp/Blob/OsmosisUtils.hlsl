// UNITY_SHADER_NO_UPGRADE
#ifndef UTILS_INCLUDED
#define UTILS_INCLUDED

float4 ballSDF(float2 uv, float2 pos, float radius, float4 color, float4 Out)
{
    float4 c = (radius / length(uv - pos)) * color;
    return c;
}

float4 squareSDF(float2 uv, float2 pos, float sideLength, float4 color)
{
    float halfSide = sideLength * 0.5;
    float2 diff = abs(uv - pos);
    float maxDist = max(diff.x, diff.y);
    float gradientValue = halfSide / maxDist;
    return gradientValue * color;
}

float4 rectangleSDF(float2 uv, float2 pos, float width, float height, float4 color)
{
    float2 halfSize = float2(width * 0.5, height * 0.5);
    float2 diff = abs(uv - pos) - halfSize;
    float dist = max(diff.x, diff.y);
    float gradientValue = clamp(1.0 - dist / max(width, height), 0.0, 1.0);
    return gradientValue * color;
}

void bubbles_float(UnityTexture2D _Bubbles, UnityTexture2D _Bubbles2, UnitySamplerState ss, int _BubbleCount,
                   float2 _UV, float4 _Color,
                   float _Time,
                   float _State,
                   bool _TopVanity,
                   bool _BottomVanity,
                   float _AspectRatio,
                   out float4 Out)
{
    Out = float4(0.0, 0.0, 0.0, 0.0);

    float aspectRatio = _AspectRatio;

    float w = 0.2;
    float h = 0.0002;

    float yPadding = 0.12;
    float y = -yPadding;

    float x = 0;
    x += sin(_Time * 0.8) * 0.01;

    float threshold = 0.75;
    float r = clamp(1.0 - _State, 0, threshold);
    float b = clamp(_State, 0, threshold);
    float4 boundsColor = float4(r, 0.0, b, 1.0);

    if (_BottomVanity)
    {
        Out += rectangleSDF(_UV, float2(x + 1, y), w, h * aspectRatio, boundsColor);
        Out += rectangleSDF(_UV, float2(x + 0.5, y), w, h * aspectRatio, boundsColor);
        Out += rectangleSDF(_UV, float2(x + 0, y), w, h * aspectRatio, boundsColor);
    }

    if (_TopVanity)
    {
        y = 1 + yPadding;

        Out += rectangleSDF(_UV, float2(x + .99, y), w, h * aspectRatio, boundsColor);
        Out += rectangleSDF(_UV, float2(x + 0.43, y), w, h * aspectRatio, boundsColor);
        Out += rectangleSDF(_UV, float2(x + -0.1, y), w, h * aspectRatio, boundsColor);
    }

    for (int i = 0; i < _BubbleCount; i++)
    {
        int row = i / 32;
        int col = i % 32;
        float2 bubbleUV = float2((col + 0.5) / 32.0, (row + 0.5) / 32.0);
        float4 bubble = _Bubbles.Sample(ss, bubbleUV);
        float4 bubble2 = _Bubbles2.Sample(ss, bubbleUV);

        float2 pos = float2(bubble.x, bubble.y);
        float r = clamp(bubble.z, 0, 0.05);

        float2 uv = float2(aspectRatio * _UV.x, _UV.y);
        float2 ballPos = float2(aspectRatio * pos.x, pos.y);

        float4 bubbleColor = (bubble.w == -1)
                                 ? float4(0.0, 2.0, 0.0, 1.1)
                                 : (bubble.w % 2 == 1)
                                 ? float4(1.0, 0.0, 0.0, 1.0)
                                 : float4(0.0, 0.0, 1.0, 1.0);

        if (bubble2.r > 0)
        {
            bubbleColor = boundsColor;
        }

        Out += ballSDF(uv, ballPos, r, bubbleColor, Out);
    }

    if (Out.g > 0.5)
    {
        Out.g = clamp(Out.g * 2.0, 0.0, 1);
        Out.rb = 0;
        Out.a = 1;
    }
}

#endif
