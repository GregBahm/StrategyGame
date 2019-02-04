internal class NewBorderGenerator
{
    private readonly MapTextureGen _main;

    public NewBorderGenerator(MapTextureGen main)
    {
        _main = main;
    }

    public void Update()
    {
        _main.BorderMat.SetTexture("_MainTex", _main.BaseTexture);
        _main.BorderMat.SetBuffer("_DistortionData", _main.DistortionOutput);
        _main.BorderMat.SetFloat("_SourceImageWidth", _main.BaseTexture.width);
        _main.BorderMat.SetFloat("_SourceImageHeight", _main.BaseTexture.height);
        _main.BorderMat.SetFloat("_BorderThickness", _main.BorderThickness);
    }
}