using AssetRipper.Library;
using SubnauticaExportHelper.PostExporter;

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
