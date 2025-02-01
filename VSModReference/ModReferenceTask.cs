using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace VSModReference;

internal static class Extensions
{
    public static bool TryGetMetadata(this ITaskItem taskItem, string metadataName, out string? metadata)
    {
        metadata = default;
        if (((ICollection<string>)taskItem.MetadataNames).Contains(metadataName))
            return (metadata = taskItem.GetMetadata(metadataName)) != null;

        return false;
    }
}
public class ModReferenceTask : Task {
    [Required]
    public string IntermediateOutputPath { get; set; } = null!;

    [Required]
    public ITaskItem[] VSModReference { get; set; } = null!;
    
    [Output]
    public ITaskItem[] NewReferences { get; private set; } = null!;


    public override bool Execute()
    {
        var outputDirectory = Path.Combine(IntermediateOutputPath, "merges");
        Directory.CreateDirectory(outputDirectory);

        foreach (var taskItem in VSModReference)
        {
            if (!taskItem.TryGetMetadata("Include", out var include)) continue;
            var version = "latest";
            taskItem.TryGetMetadata("Version", out version);
            var assemblies = "*";
            taskItem.TryGetMetadata("Assembly", out assemblies);
            taskItem.TryGetMetadata("Assemblies", out assemblies);
            
            Log.LogMessage(MessageImportance.High, $"{include} - {version} - {assemblies}");
        }
        return true;
    }
}