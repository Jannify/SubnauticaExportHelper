using AssetRipper.Library;
using AssetRipper.Library.Attributes;
using SubnauticaExportHelper;
using SubnauticaExportHelper.PostExporter;

[assembly: RegisterPlugin(typeof(SubnauticaExportHelperPlugin))]

namespace SubnauticaExportHelper;

public class SubnauticaExportHelperPlugin : PluginBase
{
    public override string Name => "SubnauticaExportHelper";

    public override void Initialize()
    {
        CurrentRipper.OnFinishExporting += OnFinishExporting;
    }

    private void OnFinishExporting()
    {
        CurrentRipper.AddPostExporter(new MoveFilesPostExporter());
        CurrentRipper.AddPostExporter(new FixDecompileErrorsPostExporter());
        CurrentRipper.AddPostExporter(new FixSubnauticaAssetsPostExporter());
    }
}