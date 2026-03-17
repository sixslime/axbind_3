namespace SixSlime.AxBind3.Logic;
using Microsoft.Extensions.FileSystemGlobbing;
internal class TargetDirManager(string rootPath)
{
    public string RootPath { get; } = rootPath;
    private Dictionary<GlobWrapper, TargetFile[]> _globCache = [];

    public TargetFile[] GetFiles(params string[] globs)
    {
        if (globs.Length == 0) return [];
        GlobWrapper globWrapper = new(globs);
        if (_globCache.TryGetValue(globWrapper, out var files)) return files;
        HashSet<string> filePaths = [..new Matcher().AddInclude(globs[0]).GetResultsInFullPath(RootPath)];
        foreach (var glob in globs[1..])
        {
            filePaths.IntersectWith(new Matcher().AddInclude(glob).GetResultsInFullPath(RootPath));
        }

        var targetFiles = filePaths.Select(path => new TargetFile()
        {
            OriginalText = File.ReadAllText(path),
            Path = path,
        })
        .ToArray();
        _globCache[globWrapper] = targetFiles;
        return targetFiles;
    }

    private class GlobWrapper(string[] globs)
    {
        private readonly string[] _globs = globs;
        public override bool Equals(object? obj)
        {
            return obj is GlobWrapper other && _globs.SequenceEqual(other._globs);
        }

        public override int GetHashCode()
        {
            return string.Join("|", _globs).GetHashCode();
        }
    }
}