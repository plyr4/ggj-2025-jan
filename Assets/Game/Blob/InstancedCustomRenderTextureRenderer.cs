using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

public class InstancedCustomRenderTextureRenderer : MonoBehaviour
{
    public CustomRenderTexture _renderTexture;
    [ReadOnlyInspector]
    [SerializeField]
    public CustomRenderTexture _renderTextureInstance;
    [Space]
    public Material _coreMaterial;
    [ReadOnlyInspector]
    [SerializeField]
    public Material _coreMaterialInstance;
    [Space]
    public Material _quadMaterial;
    [ReadOnlyInspector]
    [SerializeField]
    public Material _quadMaterialInstance;

    public class Result
    {
        public CustomRenderTexture _renderTexture;
        public Material _coreMaterial;
        public Material _quadMaterial;
    }

    public Result GenerateCustomRenderTextureMaterial()
    {
        RenderTextureFormat format = FindBestRandomWriteSupportedFormat();
        _renderTextureInstance = new CustomRenderTexture(_renderTexture.width, _renderTexture.height,
            GraphicsFormatUtility.GetGraphicsFormat(format, RenderTextureReadWrite.sRGB));
        _renderTextureInstance.name = $"{_renderTexture.name}-instanced";

        _coreMaterialInstance = new Material(_coreMaterial);
        _coreMaterialInstance.name = $"{_coreMaterial.name}-instanced";
        _renderTextureInstance.material = _coreMaterialInstance;

        _renderTextureInstance.initializationMode = CustomRenderTextureUpdateMode.OnLoad;
        _renderTextureInstance.updateMode = CustomRenderTextureUpdateMode.Realtime;
        _renderTextureInstance.enableRandomWrite = false;
        _renderTextureInstance.doubleBuffered = false;
        _renderTextureInstance.wrapMode = TextureWrapMode.Clamp;
        _renderTextureInstance.filterMode = FilterMode.Point;

        _renderTexture.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
        _renderTexture.updateMode = CustomRenderTextureUpdateMode.OnDemand;

        _quadMaterialInstance = new Material(_quadMaterial);
        _quadMaterialInstance.name = $"{_quadMaterial.name}-instanced";
        _quadMaterialInstance.mainTexture = _renderTextureInstance;

        Result result = new Result();
        result._renderTexture = _renderTextureInstance;
        result._coreMaterial = _coreMaterialInstance;
        result._quadMaterial = _quadMaterialInstance;

        return result;
    }

    public static RenderTextureFormat FindBestRandomWriteSupportedFormat()
    {
        RenderTextureFormat[] preferredFormats =
        {
            RenderTextureFormat.ARGB32,
            RenderTextureFormat.Depth,
            RenderTextureFormat.ARGBHalf,
            RenderTextureFormat.Shadowmap,
            RenderTextureFormat.RGB565,
            RenderTextureFormat.ARGB4444,
            RenderTextureFormat.ARGB1555,
            RenderTextureFormat.Default,
            RenderTextureFormat.ARGB2101010,
            RenderTextureFormat.DefaultHDR,
            RenderTextureFormat.ARGB64,
            RenderTextureFormat.ARGBFloat,
            RenderTextureFormat.RGFloat,
            RenderTextureFormat.RGHalf,
            RenderTextureFormat.RFloat,
            RenderTextureFormat.RHalf,
            RenderTextureFormat.R8,
            RenderTextureFormat.ARGBInt,
            RenderTextureFormat.RGInt,
            RenderTextureFormat.RInt,
            RenderTextureFormat.BGRA32,
            RenderTextureFormat.RGB111110Float,
            RenderTextureFormat.RG32,
            RenderTextureFormat.RGBAUShort,
            RenderTextureFormat.RG16,
            RenderTextureFormat.BGRA10101010_XR,
            RenderTextureFormat.BGR101010_XR,
            RenderTextureFormat.R16,
        };

        foreach (RenderTextureFormat format in preferredFormats)
        {
            if (SystemInfo.SupportsRandomWriteOnRenderTextureFormat(format))
            {
                return format;
            }
        }

        Debug.LogWarning("unable to find a suitable texture format for random write, using default");
        return RenderTextureFormat.Default;
    }
}