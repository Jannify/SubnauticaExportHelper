using System;
using System.IO;
using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace SubnauticaExportHelper.PostExporter;

public class FixSubnauticaAssetsPostExporter : IPostExporter
{
    public void DoPostExport(Ripper ripper)
    {
        string exportPath = ripper.Settings.ExportPath;
        string assetsPath = Path.Combine(exportPath, "Assets");

        Info("Overriding manifest.json");
        Directory.CreateDirectory(Path.Combine(exportPath, "Packages"));
        File.WriteAllText(Path.Combine(exportPath, "Packages", "manifest.json"), ScriptFiles.manifest);

        Info("Overriding ProjectSettings.asset");
        File.WriteAllText(Path.Combine(exportPath, "ProjectSettings", "ProjectSettings.asset"), ScriptFiles.ProjectSettings);

        Info("Overriding ProjectSettings.asset");
        File.WriteAllText(Path.Combine(exportPath, "ProjectSettings", "ProjectSettings.asset"), ScriptFiles.ProjectSettings);

        FixReplaceCode(Path.Combine(assetsPath, "Resources", "uGUI.prefab"), ScriptFiles.CameraScannerRoomPattern, ScriptFiles.CameraScannerRoomReplacement);

        Info("Adding AssetBundle tag");
        File.WriteAllText(Path.Combine(assetsPath, "Asset_Bundles", "basegeneratorpieces.meta"), ScriptFiles.AssetBundleMeta.Replace("{1}", RandomGuid()).Replace("{2}", "basegeneratorpieces"));
        File.WriteAllText(Path.Combine(assetsPath, "Asset_Bundles", "logos.meta"), ScriptFiles.AssetBundleMeta.Replace("{1}", RandomGuid()).Replace("{2}", "logos"));
        File.WriteAllText(Path.Combine(assetsPath, "Asset_Bundles", "waterdisplacement.meta"), ScriptFiles.AssetBundleMeta.Replace("{1}", RandomGuid()).Replace("{2}", "waterdisplacement"));
    }

    private void FixReplaceCode(string path, string pattern, string replacement)
    {
        Info($"Fixing {Path.GetFileName(path)}");
        string text = File.ReadAllText(path);
        File.WriteAllText(path, text.Replace(pattern, replacement));
    }

    private string RandomGuid() => Guid.NewGuid().ToString().Replace("-", string.Empty);
    private void Info(string message) => Logger.Info(LogCategory.Plugin, $"[SubnauticaExportHelper] {message}");
}