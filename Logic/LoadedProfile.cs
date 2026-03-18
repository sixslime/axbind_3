namespace SixSlime.AxBind3.Logic;

public record LoadedProfile(string Name) : ILoaded
{
    public required IReadOnlyList<Pass> Passes { get; init; }
    public record Pass
    {
        public required IReadOnlyList<Layer> Layers { get; init; }
        public required string CaptureEnd { get; init; }
        public required string? CaptureEscapeSequence { get; init; }
        public required string CaptureStart { get; init; }
        public required IReadOnlyList<string> Files { get; init; }
    }

    public record Layer
    {
        public required ELayerTransform Transform { get; init; }
        public required IReadOnlyList<string>? Files { get; init; }
    }

    public abstract record ELayerTransform
    {
        public sealed record Map : ELayerTransform
        {
            public required string Name { get; init; }
        }

        public sealed record Function : ELayerTransform
        {
            public required string Name { get; init; }
        }
    }

    // egregious.
    public async Task<TargetFile[]> Evaluate(string targetDirectory, ResourceLoader resources)
    {
        if (Passes.Count == 0) return [];
        Dictionary<string, string> fileWrites = [];
        var targetDir = new TargetDirManager(targetDirectory);
        int passIndex = -1;
        foreach (var pass in Passes)
        {
            passIndex++;
            Logger.Info($"[pass {passIndex + 1}]");
            List<KeyValuePair<string, Task<string>>> fileReads = [];
            var passFiles = pass.Files!.ToArray();
            foreach (var targetPath in targetDir.GetFiles(passFiles))
                if (!fileWrites.ContainsKey(targetPath))
                    fileReads.Add(KeyValuePair.Create(targetPath, File.ReadAllTextAsync(targetPath)));
            var readResults = await Task.WhenAll(fileReads.Select(x => x.Value));
            for (var i = 0; i < readResults.Length; i++) fileWrites[fileReads[i].Key] = readResults[i];

            Dictionary<string, TransformBuffer> fileBuffers = new(fileWrites.Count);
            int layerIndex = -1;
            foreach (var layer in pass.Layers)
            {
                layerIndex++;
                Logger.Info($" [layer {layerIndex + 1}]");
                var layerFiles = layer.Files?.ToArray();
                foreach (var targetPath in targetDir.GetFiles(layerFiles is null ? [passFiles] : [passFiles, layerFiles]))
                {
                    Logger.VerboseInfo($"  [file '{targetPath}']");
                    var bufferExisted = fileBuffers.TryGetValue(targetPath, out var buf);
                    var targetBuffer = bufferExisted ? buf! : TransformBuffer.Create(fileWrites[targetPath], pass.CaptureStart, pass.CaptureEnd, pass.CaptureEscapeSequence);
                    switch (layer.Transform)
                    {
                        case ELayerTransform.Function v:
                            await targetBuffer.ApplyFunction(resources.LoadFunction(v.Name));
                            break;
                        case ELayerTransform.Map v:
                            targetBuffer.ApplyMap(resources.LoadMap(v.Name));
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