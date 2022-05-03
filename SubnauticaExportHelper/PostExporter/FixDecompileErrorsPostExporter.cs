using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace SubnauticaExportHelper.PostExporter;

public class FixDecompileErrorsPostExporter : IPostExporter
{
    private readonly string[] whitelistAssemblies =
    {
        "iTween", "LibNoise", "LitJson", "LumenWorks", "Newtonsoft.Json", "PlatformIODefault", "PlatformUtilsDefault", "Poly2Tri",
        "Unity.MemoryProfiler", "Unity.Timeline", "XGamingRuntime"
    };

    public void DoPostExport(Ripper ripper)
    {
        string assetsPath = ripper.Settings.AssetsPath;
        string auxiliaryFilesPath = ripper.Settings.AuxiliaryFilesPath;
        string subnauticaPath = ripper.GameStructure.PlatformStructure.RootPath;

        Info("Extract valid Scripts");
        Directory.Move(Path.Combine(assetsPath, "Scripts", "Assembly-CSharp-firstpass"), Path.Combine(assetsPath, "Plugins"));
        Directory.Move(Path.Combine(assetsPath, "Scripts", "Assembly-CSharp"), Path.Combine(assetsPath, "ScriptsTMP"));
        Directory.Delete(Path.Combine(assetsPath, "Scripts"), true);
        Directory.Move(Path.Combine(assetsPath, "ScriptsTMP"), Path.Combine(assetsPath, "Scripts"));

        Info("Copying compiled assemblies");
        Directory.CreateDirectory(Path.Combine(assetsPath, "Libraries"));
        foreach (string assemblyName in whitelistAssemblies)
        {
            File.Move(Path.Combine(auxiliaryFilesPath, "GameAssemblies", $"{assemblyName}.dll"), Path.Combine(assetsPath, "Libraries", $"{assemblyName}.dll"));
        }

        Info("Copying native assemblies");
        Directory.CreateDirectory(Path.Combine(assetsPath, "Libraries", "Native"));
        CopyDirectory(Path.Combine(subnauticaPath, "Subnautica_Data", "Plugins"), Path.Combine(assetsPath, "Libraries", "Native"));

        Info("Add UnityEngine.PostProcessing scripts");
        string scriptPath = Path.Combine(assetsPath, "Scripts");
        string pluginsPath = Path.Combine(assetsPath, "Plugins");
        Directory.CreateDirectory(Path.Combine(scriptPath, "UnityEngine", "PostProcessing"));
        File.WriteAllText(Path.Combine(scriptPath, "UnityEngine", "PostProcessing", "PostProcessingComponent.cs"), ScriptFiles.PostProcessingComponent);
        File.WriteAllText(Path.Combine(scriptPath, "UnityEngine", "PostProcessing", "PostProcessingComponentBase.cs"), ScriptFiles.PostProcessingComponentBase);
        File.WriteAllText(Path.Combine(scriptPath, "UnityEngine", "PostProcessing", "PostProcessingComponentCommandBuffer.cs"), ScriptFiles.PostProcessingComponentCommandBuffer);
        File.WriteAllText(Path.Combine(scriptPath, "UnityEngine", "PostProcessing", "PostProcessingComponentRenderTexture.cs"), ScriptFiles.PostProcessingComponentRenderTexture);
        File.WriteAllText(Path.Combine(pluginsPath, "EditorModifications.cs"), ScriptFiles.EditorModifications);

        Info("Overriding complex classes");
        Directory.CreateDirectory(Path.Combine(pluginsPath, "SubnauticaFixes"));
        File.WriteAllText(Path.Combine(pluginsPath, "SubnauticaFixes", "MeshExtension.cs"), ScriptFiles.MeshExtension);

        FixSpecialNameMethod(Path.Combine(pluginsPath, "VoxelandChunk.cs"), ScriptFiles.VoxelandChunkPattern);
        FixSpecialNameMethod(Path.Combine(scriptPath, "WorldStreaming", "ClipmapChunk.cs"), ScriptFiles.ClipmapChunkPattern);
        FixSpecialNameMethod(Path.Combine(scriptPath, "uGUI_PopupMessage.cs"), ScriptFiles.uGUI_PopupMessagePattern);
        FixSpecialNameMethod(Path.Combine(scriptPath, "uGUI.cs"), ScriptFiles.uGUI_PopupMessagePattern);
        FixSpecialNameMethod(Path.Combine(scriptPath, "uGUI_Tooltip.cs"), ScriptFiles.uGUI_PopupMessagePattern);

        FixReplaceCode(Path.Combine(scriptPath, "uGUI_Pings.cs"), ScriptFiles.uGUI_PingsPattern, ScriptFiles.uGUI_PingsReplacement);
        FixReplaceCode(Path.Combine(pluginsPath, "SentrySdk.cs"), ScriptFiles.SentrySDKPattern, ScriptFiles.SentrySDKReplacement);
    }

    private void FixSpecialNameMethod(string path, string pattern)
    {
        Info($"Fixing {Path.GetFileName(path)}");
        string text = File.ReadAllText(path);
        File.WriteAllText(path, text.Replace(pattern, string.Empty));
    }

    private void FixReplaceCode(string path, string pattern, string replacement)
    {
        Info($"Fixing {Path.GetFileName(path)}");
        string text = File.ReadAllText(path);
        File.WriteAllText(path, text.Replace(pattern, replacement));
    }

    private static void Info(string message) => Logger.Info(LogCategory.Plugin, $"[SubnauticaExportHelper] {message}");

    private static void CopyDirectory(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}
