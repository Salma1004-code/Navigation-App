using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

public enum OperationState
{
    Normal,
    Sleep,
    Snooze
}

public static class NotificationNameExtensions
{
    public static string AutomaticCalloutsEnabledChanged = "GDAAutomaticCalloutsChanged";
    public static string AutoCalloutCategorySenseChanged = "GDAAutomaticCalloutSenseChanged";
    public static string BeaconVolumeChanged = "GDABeaconVolumeChanged";
    public static string TTSVolumeChanged = "GDATTSVolumeChanged";
    public static string OtherVolumeChanged = "GDAOtherVolumeChanged";
    public static string BeaconGainChanged = "GDABeaconGainChanged";
    public static string PreviewIntersectionsIncludeUnnamedRoadsDidChange = "PreviewIntersectionsIncludeUnnamedRoadsDidChange";
    public static string AppOperationStateDidChange = "GDAAppOperationStateDidChange";
}

public class SettingsContext
{
    private static SettingsContext _instance;
    public static SettingsContext Instance => _instance ??= new SettingsContext();

    private Dictionary<string, object> _userDefaults = new Dictionary<string, object>();

    public int AppUseCount
    {
        get => GetValue<int>("GDAAppUseCount");
        set => SetValue("GDAAppUseCount", value);
    }

    public string NewFeaturesLastDisplayedVersion
    {
        get => GetValue<string>("GDANewFeaturesLastDisplayedVersion") ?? "0.0.0";
        set => SetValue("GDANewFeaturesLastDisplayedVersion", value);
    }

    public string ClientId
    {
        get
        {
            if (_userDefaults.ContainsKey("GDAUserDefaultClientIdentifier"))
            {
                return _userDefaults["GDAUserDefaultClientIdentifier"].ToString();
            }
            else
            {
                string clientId = Guid.NewGuid().ToString();
                _userDefaults["GDAUserDefaultClientIdentifier"] = clientId;
                return clientId;
            }
        }
        set => _userDefaults["GDAUserDefaultClientIdentifier"] = value;
    }

    public bool MetricUnits
    {
        get => GetValue<bool>("GDASettingsMetric");
        set => SetValue("GDASettingsMetric", value);
    }

    public string Locale
    {
        get => GetValue<string>("GDASettingsLocaleIdentifier");
        set => SetValue("GDASettingsLocaleIdentifier", value);
    }

    public float SpeakingRate
    {
        get => GetValue<float>("GDASettingsSpeakingRate");
        set => SetValue("GDASettingsSpeakingRate", value);
    }

    public float BeaconVolume
    {
        get => GetValue<float>("GDABeaconVolume");
        set
        {
            SetValue("GDABeaconVolume", value);
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNameExtensions.BeaconVolumeChanged, this);
        }
    }

    public float TTSVolume
    {
        get => GetValue<float>("GDATTSVolume");
        set
        {
            SetValue("GDATTSVolume", value);
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNameExtensions.TTSVolumeChanged, this);
        }
    }

    public float OtherVolume
    {
        get => GetValue<float>("GDAOtherVolume");
        set
        {
            SetValue("GDAOtherVolume", value);
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNameExtensions.OtherVolumeChanged, this);
        }
    }

    public float TTSGain
    {
        get => GetValue<float>("GDATTSAudioGain");
        set => SetValue("GDATTSAudioGain", value);
    }

    public float BeaconGain
    {
        get => GetValue<float>("GDABeaconAudioGain");
        set
        {
            SetValue("GDABeaconAudioGain", value);
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNameExtensions.BeaconGainChanged, this);
        }
    }

    public float AfxGain
    {
        get => GetValue<float>("GDAAFXAudioGain");
        set => SetValue("GDAAFXAudioGain", value);
    }

    public bool TelemetryOptout
    {
        get => GetValue<bool>("GDASettingsTelemetryOptout");
        set => SetValue("GDASettingsTelemetryOptout", value);
    }

    public bool PreviewIntersectionsIncludeUnnamedRoads
    {
        get => GetValue<bool>("GDASettingsPreviewIntersectionsIncludeUnnamedRoads");
        set
        {
            SetValue("GDASettingsPreviewIntersectionsIncludeUnnamedRoads", value);
            NotificationCenter.DefaultCenter.PostNotificationName(NotificationNameExtensions.PreviewIntersectionsIncludeUnnamedRoadsDidChange, this);
        }
    }

    public bool AudioSessionMixesWithOthers
    {
        get => GetValue<bool>("GDAAudioSessionMixesWithOthers");
        set => SetValue("GDAAudioSessionMixesWithOthers", value);
    }

    public string SelectedBeacon
    {
        get
        {
            if (_userDefaults.ContainsKey("GDASelectedBeaconName"))
            {
                return _userDefaults["GDASelectedBeaconName"].ToString();
            }
            else
            {
                if (_userDefaults.ContainsKey("GDASettingsUseOldBeacon") && (bool)_userDefaults["GDASettingsUseOldBeacon"])
                {
                    return ClassicBeacon;
                }
                else
                {
                    return V2Beacon;
                }
            }
        }
        set => _userDefaults["GDASelectedBeaconName"] = value;
    }

    public bool PlayBeaconStartAndEndMelodies
    {
        get => GetValue<bool>("GDAPlayBeaconStartEndMelody");
        set => SetValue("GDAPlayBeaconStartEndMelody", value);
    }

    private SettingsContext() { }

    private T GetValue<T>(string key)
    {
        if (_userDefaults.ContainsKey(key))
        {
            return (T)_userDefaults[key];
        }
        else
        {
            // Provide default values for certain types here if needed
            return default;
        }
    }

    private void SetValue<T>(string key, T value)
    {
        _userDefaults[key] = value;
    }

    public static string ClassicBeacon = "Classic";
    public static string V2Beacon = "V2";

    public static SettingsContext Shared => Instance;
}

public class AppContext
{
    private static AppContext _instance;
    public static AppContext Instance => _instance ??= new AppContext();

    private OperationState _state = OperationState.Normal;
    public OperationState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                NotificationCenter.DefaultCenter.PostNotificationName(NotificationNameExtensions.AppOperationStateDidChange, this, new NSDictionary(NotificationNameExtensions.AppOperationStateDidChange, new NSString(_state.ToString())));
}
}
}
  public bool IsInTutorialMode { get; set; }

private bool _hasAttemptedToStart;
private bool _hasStarted;

private AppContext() { }

public void Start(bool fromFirstLaunch = false)
{
    _hasAttemptedToStart = true;
    Instance.IsFirstLaunch = fromFirstLaunch;

    if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Inactive)
    {
        StartBLE();

        if (IsActive)
        {
            AudioEngine.Shared.Start();
            EventProcessor.Shared.Start();
        }

        GeolocationManager.Shared.Start();

        DeviceMotionManager.Shared.StartDeviceMotionUpdates();
        CloudKeyValueStore.Shared.Start();
        SpatialDataContext.Shared.Start();

        if (!(EventProcessor.Shared.ActiveBehavior is OnboardingBehavior))
        {
            EventProcessor.Shared.Process(new GlyphEvent(GlyphEventType.AppLaunch));
        }

        AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
        appDelegate.PushNotificationManager.Start();

        _hasStarted = true;
    }
}

private void StartBLE()
{
    if (DeviceManager.Shared.HasStoredDevices)
    {
        // Wait for the central BLE manager to enter the powered ON state before trying to connect to any devices
        NotificationCenter.DefaultCenter.AddObserver(BLEManager.NotificationName.BluetoothDidUpdateState, (NSNotification notification) =>
        {
            var state = (CBManagerState)(NSNumber)notification.UserInfo[BLEManager.NotificationKeys.State];
            if (state == CBManagerState.PoweredOn)
            {
                DeviceManager.Shared.LoadAndConnectDevices();
            }
        });

        // Show error alert if BLE is unauthorized
        NotificationCenter.DefaultCenter.AddObserver(BLEManager.NotificationName.BluetoothDidUpdateState, (NSNotification notification) =>
        {
            var state = (CBManagerState)(NSNumber)notification.UserInfo[BLEManager.NotificationKeys.State];
            if (state == CBManagerState.Unauthorized)
            {
                var rootViewController = AppContextExtensions.RootViewController();
                UIAlertController alert = ErrorAlerts.BuildBLEAlert();
                rootViewController.PresentViewController(alert, true, null);
            }
        });

        // Initialize the lazy property
        var bleManager = BLEManager.Shared;
    }
}

public void GoToSleep()
{
    EventProcessor.Shared.Sleep();
    GeolocationManager.Shared.Stop();
    DeviceMotionManager.Shared.StopDeviceMotionUpdates();
    SpatialDataContext.Shared.Stop();
    State = OperationState.Sleep;
}

public void Snooze()
{
    SpatialDataContext.Shared.Stop();
    GeolocationManager.Shared.SnoozeDelegate = new SnoozeDelegate();
    GeolocationManager.Shared.Snooze();
    DeviceMotionManager.Shared.StopDeviceMotionUpdates();
    State = OperationState.Snooze;
}

public void WakeUp()
{
    if (State == OperationState.Sleep || State == OperationState.Snooze)
    {
        GeolocationManager.Shared.Start();
        DeviceMotionManager.Shared.StartDeviceMotionUpdates();
        SpatialDataContext.Shared.Start();
        State = OperationState.Normal;
        EventProcessor.Shared.Wake();
    }
}

public void ValidateActive()
{
    if (!_hasStarted && _hasAttemptedToStart)
    {
        if (!_hasStarted)
        {
            Console.WriteLine("AppContext has not yet started...");
        }
        else
        {
            Console.WriteLine("Validated: AppContext has been started");
        }
        return;
    }

    Console.WriteLine("AppContext failed to start previously. Calling start() again...");
    Start(Instance.IsFirstLaunch);
}

public static bool IsActive => !(UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background && AppState == UIApplicationState.Background);

public static OperationState AppState => _appState;
private static OperationState _appState = UIApplication.SharedApplication.ApplicationState;

public static UIViewController RootViewController()
{
    if (UIApplication.SharedApplication.Delegate is AppDelegate appDelegate)
    {
        return appDelegate.Window.RootViewController;
    }
    return null;
}

public bool IsStreetPreviewing => EventProcessor.Shared.IsActive(typeof(StreetPreviewBehavior));
public bool IsRouteGuidanceActive => EventProcessor.Shared.IsActive(typeof(RouteGuidance));

public bool IsFirstLaunch { get; set; }

public static void Process(Event evt)
{
    EventProcessor.Shared.Process(evt);
}
public class SnoozeDelegate : GeolocationManagerSnoozeDelegate
{
public void SnoozeDidFail()
{
AppContext.Instance.WakeUp();
}
  public void SnoozeDidTrigger()
{
    AppContext.Instance.WakeUp();
}

_state.ToString())));
}
}
}

scss
Copy code
public bool IsInTutorialMode { get; set; }

private bool _hasAttemptedToStart;
private bool _hasStarted;

private AppContext() { }

public void Start(bool fromFirstLaunch = false)
{
    _hasAttemptedToStart = true;
    Instance.IsFirstLaunch = fromFirstLaunch;

    if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Inactive)
    {
        StartBLE();

        if (IsActive)
        {
            AudioEngine.Shared.Start();
            EventProcessor.Shared.Start();
        }

        GeolocationManager.Shared.Start();

        DeviceMotionManager.Shared.StartDeviceMotionUpdates();
        CloudKeyValueStore.Shared.Start();
        SpatialDataContext.Shared.Start();

        if (!(EventProcessor.Shared.ActiveBehavior is OnboardingBehavior))
        {
            EventProcessor.Shared.Process(new GlyphEvent(GlyphEventType.AppLaunch));
        }

        AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
        appDelegate.PushNotificationManager.Start();

        _hasStarted = true;
    }
}

private void StartBLE()
{
    if (DeviceManager.Shared.HasStoredDevices)
    {
        // Wait for the central BLE manager to enter the powered ON state before trying to connect to any devices
        NotificationCenter.DefaultCenter.AddObserver(BLEManager.NotificationName.BluetoothDidUpdateState, (NSNotification notification) =>
        {
            var state = (CBManagerState)(NSNumber)notification.UserInfo[BLEManager.NotificationKeys.State];
            if (state == CBManagerState.PoweredOn)
            {
                DeviceManager.Shared.LoadAndConnectDevices();
            }
        });

        // Show error alert if BLE is unauthorized
        NotificationCenter.DefaultCenter.AddObserver(BLEManager.NotificationName.BluetoothDidUpdateState, (NSNotification notification) =>
        {
            var state = (CBManagerState)(NSNumber)notification.UserInfo[BLEManager.NotificationKeys.State];
            if (state == CBManagerState.Unauthorized)
            {
                var rootViewController = AppContextExtensions.RootViewController();
                UIAlertController alert = ErrorAlerts.BuildBLEAlert();
                rootViewController.PresentViewController(alert, true, null);
            }
        });

        // Initialize the lazy property
        var bleManager = BLEManager.Shared;
    }
}

public void GoToSleep()
{
    EventProcessor.Shared.Sleep();
    GeolocationManager.Shared.Stop();
    DeviceMotionManager.Shared.StopDeviceMotionUpdates();
    SpatialDataContext.Shared.Stop();
    State = OperationState.Sleep;
}

public void Snooze()
{
    SpatialDataContext.Shared.Stop();
    GeolocationManager.Shared.SnoozeDelegate = new SnoozeDelegate();
    GeolocationManager.Shared.Snooze();
    DeviceMotionManager.Shared.StopDeviceMotionUpdates();
    State = OperationState.Snooze;
}

public void WakeUp()
{
    if (State == OperationState.Sleep || State == OperationState.Snooze)
    {
        GeolocationManager.Shared.Start();
        DeviceMotionManager.Shared.StartDeviceMotionUpdates();
        SpatialDataContext.Shared.Start();
        State = OperationState.Normal;
        EventProcessor.Shared.Wake();
    }
}

public void ValidateActive()
{
    if (!_hasStarted && _hasAttemptedToStart)
    {
        if (!_hasStarted)
        {
            Console.WriteLine("AppContext has not yet started...");
        }
        else
        {
            Console.WriteLine("Validated: AppContext has been started");
        }
        return;
    }

    Console.WriteLine("AppContext failed to start previously. Calling start() again...");
    Start(Instance.IsFirstLaunch);
}

public static bool IsActive => !(UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background && AppState == UIApplicationState.Background);

public static OperationState AppState => _appState;
private static OperationState _appState = UIApplication.SharedApplication.ApplicationState;

public static UIViewController RootViewController()
{
    if (UIApplication.SharedApplication.Delegate is AppDelegate appDelegate)
    {
        return appDelegate.Window.RootViewController;
    }
    return null;
}

public bool IsStreetPreviewing => EventProcessor.Shared.IsActive(typeof(StreetPreviewBehavior));
public bool IsRouteGuidanceActive => EventProcessor.Shared.IsActive(typeof(RouteGuidance));

public bool IsFirstLaunch { get; set; }

public static void Process(Event evt)
{
    EventProcessor.Shared.Process(evt);
}
}

public class SnoozeDelegate : GeolocationManagerSnoozeDelegate
{
public void SnoozeDidFail()
{
AppContext.Instance.WakeUp();
}

csharp
Copy code
public void SnoozeDidTrigger()
{
    AppContext.Instance.WakeUp();
}
}

public static class AppContextExtensions
{
public static UIViewController RootViewController(this AppContext appContext)
{
if (UIApplication.SharedApplication.Delegate is AppDelegate appDelegate)
{
return appDelegate.Window.RootViewController;
}
return null;
}
}
