public class MissionService
{

    public List<string> GlobalLog { get; private set; } = new();

    public void AddLog(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm");
        GlobalLog.Insert(0, $"[{timestamp}] {message}");
        if (GlobalLog.Count > 10) GlobalLog.RemoveAt(10);

        // Här kan vi senare trigga en händelse så UI:t uppdateras
        NotifyStateChanged();
    }

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();
}