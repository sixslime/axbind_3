namespace SixSlime.AxBind3.Logic;

using System.Text;

internal class TransformBuffer
{
    public static TransformBuffer Create(string text, string captureStart, string captureEnd, string? captureEscape)
    {
        List<string> inactive = [];
        List<string> active = [];
        StringBuilder inactiveTextBuffer = new();
        StringBuilder activeTextBuffer = new();
        while (text.Length > 0)
        {
            var startSplit = text.Split(captureStart, 2);
            if (startSplit.Length != 2) break;
            if (captureEscape is not null && startSplit[0].EndsWith(captureEscape))
            {
                inactiveTextBuffer.Append(startSplit[0][..-(captureEscape.Length - 1)]);
                inactiveTextBuffer.Append(captureStart);
                text = startSplit[1];
                continue;
            }

            var captureAttempt = startSplit[1];
            var endSplit = captureAttempt.Split(captureEnd, 2);
            if (captureEscape is not null && endSplit[0].EndsWith(captureEscape))
            {
                activeTextBuffer.Append(endSplit[0][..-(captureEscape.Length - 1)]);
                activeTextBuffer.Append(captureEnd);
                text = endSplit[1];
                continue;
            }

            if (endSplit.Length != 2) break;
            inactiveTextBuffer.Append(startSplit[0]);
            activeTextBuffer.Append(endSplit[0]);
            inactive.Add(inactiveTextBuffer.ToString());
            active.Add(activeTextBuffer.ToString());
            inactiveTextBuffer.Clear();
            activeTextBuffer.Clear();
            text = endSplit[1];
        }

        inactiveTextBuffer.Append(text);
        inactive.Add(inactiveTextBuffer.ToString());
        return new(inactive.ToArray(), active.ToArray());
    }

    private readonly string[] _activeText;
    private readonly string[] _inactiveText;

    private TransformBuffer(string[] inactive, string[] active)
    {
        _inactiveText = inactive;
        _activeText = active;
    }

    public async Task ApplyFunction(TransformFunction function)
    {
        var processes = new Task<string>[_activeText.Length];
        for (var i = 0; i < _activeText.Length; i++) processes[i] = function.Run(_activeText[i]);
        var results = await Task.WhenAll(processes);
        for (var i = 0; i < _activeText.Length; i++) _activeText[i] = results[i];
    }

    public void ApplyMap(IReadOnlyDictionary<string, string> map)
    {
        for (var i = 0; i < _activeText.Length; i++)
            if (map.TryGetValue(_activeText[i], out var replacement))
                _activeText[i] = replacement;
    }

    public string GetResult()
    {
        StringBuilder text = new();
        for (var i = 0; i < _inactiveText.Length; i++)
        {
            text.Append(_inactiveText[i]);
            if (i < _activeText.Length) text.Append(_activeText[i]);
        }

        return text.ToString();
    }
}