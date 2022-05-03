using System.Security.Cryptography;
using System.Text.Json;
using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace SubnauticaExportHelper.PostExporter;

public class MoveFilesPostExporter : IPostExporter
{
    public void DoPostExport(Ripper ripper)
    {
        string exportPath = ripper.Settings.ProjectRootPath;
        string assetsPath = ripper.Settings.AssetsPath;
        string subnauticaPath = ripper.GameStructure.PlatformStructure.RootPath;

        Info("Backing up Shaders");
        MD5 md5 = MD5.Create();
        Dictionary<string, byte[]> shaderToHash = new();
        Directory.CreateDirectory(Path.Combine(exportPath, "ShaderBackup", "Shader"));
        foreach (string shaderPath in Directory.GetFiles(Path.Combine(assetsPath, "Shader"), "*.asset", SearchOption.AllDirectories))
        {
            CopyShader(shaderPath);
        }

        foreach (string shaderPath in Directory.GetFiles(Path.Combine(assetsPath, "Resources"), "*.asset", SearchOption.AllDirectories))
        {
            if (File.ReadLines(shaderPath).ElementAt(3) == "Shader:")
            {
                CopyShader(shaderPath);
            }
        }

        void CopyShader(string shaderPath)
        {
            string destinationPath = shaderPath.Replace("Assets", "ShaderBackup");
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

            using FileStream fileStream = File.OpenRead(shaderPath);
            shaderToHash.Add(Path.GetFileName(shaderPath),  md5.ComputeHash(fileStream));

            fileStream.Position = 0;
            using FileStream destinationStream = File.OpenWrite(destinationPath);
            fileStream.CopyTo(destinationStream);
        }

        File.WriteAllText(Path.Combine(exportPath, "ShaderBackup", "shaderHashes.json"), JsonSerializer.Serialize(shaderToHash));

        Info("Copying StreamingAssets, this may take a while");
        CopyDirectory(Path.Combine(subnauticaPath, "Subnautica_Data", "StreamingAssets"), Path.Combine(assetsPath, "StreamingAssets"));

        Info("Moving Prefabs/PDA assets");
        string prefabsPDA = Path.Combine(assetsPath, "Prefabs", "PDA");
        Directory.CreateDirectory(prefabsPDA);
        File.Move(Path.Combine(assetsPath, "ScriptableObject", "PDAData.asset"), Path.Combine(prefabsPDA, "PDAData.asset"));
        File.Move(Path.Combine(assetsPath, "ScriptableObject", "PDAData.asset.meta"), Path.Combine(prefabsPDA, "PDAData.asset.meta"));

        Info("Moving scenes");
        string sceneDir = Path.Combine(assetsPath, "Scene");
        Directory.Move(Path.Combine(sceneDir, "Scenes"), Path.Combine(assetsPath, "Scenes"));
        Directory.Move(Path.Combine(sceneDir, "SubmarineScenes"), Path.Combine(assetsPath, "SubmarineScenes"));
        Directory.Delete(sceneDir);

        Info("Moving Atlases assets");
        string uGuiResourceDir = Path.Combine(assetsPath, "uGUI", "Resources");
        Directory.CreateDirectory(uGuiResourceDir);
        Directory.Move(Path.Combine(assetsPath, "Resources", "atlases"), Path.Combine(uGuiResourceDir, "Atlases"));
    }

    private void Info(string message) => Logger.Info(LogCategory.Plugin, $"[SubnauticaExportHelper] {message}");

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
