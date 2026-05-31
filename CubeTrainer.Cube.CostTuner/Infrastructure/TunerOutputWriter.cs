using System.Text.Json;

namespace CubeTrainer.Cube.CostTuner.Infrastructure;

internal static class TunerOutputWriter
{
    public static void WriteJsonReport<T>(string path, T report)
    {
        EnsureParentDirectory(path);
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        File.WriteAllText(path, json);
    }

    private static void EnsureParentDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
