using System;
using System.Collections.Generic;
using System.Threading;
using Foundation;

public class Sounds
{
    private static readonly Sounds EmptyInstance = new Sounds();

    public static Sounds Empty => EmptyInstance;

    private readonly object lockObject = new object();
    private readonly List<Sound> soundList;
    private AsyncSoundsNotificationHandler onNotificationHandler;
    private Notification.Name notificationName;
    private object notificationObject;

    public bool IsEmpty => soundList.Count == 0;

    public Sounds(List<Sound> sounds = null)
    {
        soundList = sounds ?? new List<Sound>();
    }

    public Sounds(Sound sound) : this(new List<Sound> { sound })
    {
    }

    public Sounds(List<Sound> soundList, AsyncSoundsNotificationHandler onNotificationHandler, Notification.Name notificationName, object notificationObject)
        : this(soundList)
    {
        this.onNotificationHandler = onNotificationHandler;
        this.notificationName = notificationName;
        this.notificationObject = notificationObject;

        if (notificationName != null)
        {
            NotificationCenter.DefaultCenter.AddObserver(this, new ObjCRuntime.Selector("OnAsyncSoundsCompleted"), notificationName, notificationObject);
        }
    }

    public Sound Next()
    {
        lock (lockObject)
        {
            if (soundList.Count > 0)
            {
                var nextSound = soundList[0];
                soundList.RemoveAt(0);
                return nextSound;
            }

            return null;
        }
    }

    [Export("OnAsyncSoundsCompleted")]
    private void OnAsyncSoundsCompleted(NSNotification notification)
    {
        lock (lockObject)
        {
            var sounds = onNotificationHandler?.Invoke(notification);
            if (sounds != null)
            {
                soundList.AddRange(sounds);
            }
        }
    }
}

public delegate List<Sound> AsyncSoundsNotificationHandler(NSNotification notification);
