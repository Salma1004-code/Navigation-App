using System;
using System.IO;

public interface IDirectoryObserverDelegate
{
    void EventDispatched(DirectoryObserver directoryObserver);
}

public class DirectoryObserver : IDisposable
{
    private readonly string path;
    private readonly IDirectoryObserverDelegate delegate;
    private FileSystemWatcher watcher;

    public DirectoryObserver(string path, IDirectoryObserverDelegate delegate)
    {
        this.path = path;
        this.delegate = delegate;
    }

    public void StartObserving()
    {
        watcher = new FileSystemWatcher(path)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;

        watcher.EnableRaisingEvents = true;
    }

    public void StopObserving()
    {
        watcher?.Dispose();
        watcher = null;
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        delegate?.EventDispatched(this);
    }

    private void OnRenamed(object source, RenamedEventArgs e)
    {
        delegate?.EventDispatched(this);
    }

    public void Dispose()
    {
        StopObserving();
    }
}
