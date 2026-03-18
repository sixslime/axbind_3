namespace SixSlime.AxBind3.Logic;

using Microsoft.Extensions.FileSystemGlobbing;

public class TargetDirManager(string rootPath)
{
    public string RootPath { get; } = rootPath;
    private readonly Dictionary<GlobWrapper, string[]> _globCache = [];

    public string[] GetFiles(params string[][] globs)
    {
        if (globs.Length == 0) return [];
        GlobWrapper globWrapper = new(globs);
        if (_globCache.TryGetValue(globWrapper, out var files)) return files;
        var firstMatcher = new Matcher();
        firstMatcher.AddIncludePatterns(globs[0]);
        HashSet<string> filePaths = [.. firstMatcher.GetResultsInFullPath(RootPath)];
        foreach (var glob in globs[1..])
        {
            var matcher = new Matcher();
            matcher.AddIncludePatterns(glob);
            filePaths.IntersectWith(matcher.GetResultsInFullPath(RootPath));
        }
        var o = filePaths.ToArray();
        _globCache[globWrapper] = o;
        return o;
    }

    private class GlobWrapper(string[][] globs)
    {
        private readonly string[][] _globs = globs;

        public override bool Equals(object? obj)
        {
            if (obj is not GlobWrapper other) return false;
            if (other._globs.Length != _globs.Length) return false;
            for (int i = 0; i < _globs.Length; i++)
            {
                if (!_globs[i].SequenceEqual(other._globs[i])) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return string.Join("|", _globs.Select(x => x)).GetHashCode();
        }
    }
}