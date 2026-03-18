namespace SixSlime.AxBind3.TomlModel;

using Tomlyn.Serialization;
using Tomlyn;
using Logic;

public class ProfileFile : TomlValidatable
{
    public static ProfileFile FromToml(string toml)
    {
        return TomlSerializer.Deserialize(toml, AppTomlContext.Default.ProfileFile)!;
    }

    public List<Pass> Passes { get; set; } = [];
    protected override (object?, string)[] RequiredKeys => [];

    // egregious.
    public async Task<TargetFile[]> Evaluate(string targetDirectory, ResourceLoader loader)
    {
        if (Passes.Count == 0) return [];
        Dictionary<string, string> fileWrites = [];
        var targetDir = new TargetDirManager(targetDirectory);
        for (var passIndex = 0; passIndex < Passes.Count; passIndex++)
        {
            var pass = Passes[passIndex];
            pass.ValidateRequiredKeys($"pass {passIndex + 1}");
            List<KeyValuePair<string, Task<string>>> fileReads = [];
            foreach (var targetPath in targetDir.GetFiles(pass.Files!))
                if (!fileWrites.ContainsKey(targetPath))
                    fileReads.Add(KeyValuePair.Create(targetPath, File.ReadAllTextAsync(targetPath)));
            var readResults = await Task.WhenAll(fileReads.Select(x => x.Value));
            for (var i = 0; i < readResults.Length; i++) fileWrites[fileReads[i].Key] = readResults[i];

            Dictionary<string, TransformBuffer> fileBuffers = new(fileWrites.Count);
            for (var layerIndex = 0; layerIndex < pass.Layers.Count; layerIndex++)
            {
                var layer = pass.Layers[layerIndex];
                layer.ValidateRequiredKeys($"pass {passIndex + 1}, layer {layerIndex + 1}");
                foreach (var targetPath in targetDir.GetFiles(layer.Files is null ? [pass.Files!] : [pass.Files!, layer.Files]))
                {
                    var bufferExisted = fileBuffers.TryGetValue(targetPath, out var buf);
                    var targetBuffer = bufferExisted ? buf! : TransformBuffer.Create(fileWrites[targetPath], pass.CaptureStart!, pass.CaptureEnd!, pass.CaptureEscapeSequence);
                    switch (layer.Transform)
                    {
                        case var v when v!.Map is not null:
                            targetBuffer.ApplyMap(loader.LoadMap(v.Map).GetMappings(loader));
                            break;
                        case var v when v!.Function is not null:
                            await targetBuffer.ApplyFunction(loader.LoadFunction(v.Function));
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    if (!bufferExisted) fileBuffers[targetPath] = targetBuffer;
                }
            }

            foreach (var (path, buffer) in fileBuffers)
                fileWrites[path] = buffer.GetResult();
        }

        return fileWrites.Select(x => new TargetFile
        {
            Path = x.Key,
            Contents = x.Value
        }).ToArray();
    }
}