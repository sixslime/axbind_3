namespace SixSlime.AxBind3.Logic;
using Microsoft.Extensions.FileSystemGlobbing;
internal class TargetDirManager(string rootPath)
{
    public string RootPath { get; } = rootPath;
    private Dictionary<GlobWrapper, string[]> _globCache = [];

    public string[] GetFiles(params string[] globs)
    {
        if (globs.Length == 0) return [];
        GlobWrapper globWrapper = new(globs);
        if (_globCache.TryGetValue(globWrapper, out var files)) return files;
        HashSet<string> filePaths = [..new Matcher().AddInclude(globs[0]).GetResultsInFullPath(RootPath)];
        foreach (var glob in globs[1..])
        {
            filePaths.IntersectWith(new Matcher().AddInclude(glob).GetResultsInFullPath(RootPath));
        }
        var o = filePaths.ToArray();
        _globCache[globWrapper] = o;
        return o;
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