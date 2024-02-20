using System;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using CoreLocation;
using Foundation;

public class BeaconDemoHelper
{
    private enum BeaconType
    {
        Location,
        Entity,
        Ref
    }

    private struct AudioSystemState
    {
        public BeaconType? OriginalBeacon;
        public bool WasBeaconEnabled;
        public bool WereBeaconMelodiesDisabled;
        public bool OriginalBeaconMelodiesState;
        public bool WereCalloutsEnabled;
    }

    private bool shouldRestoreState = false;
    private AudioSystemState? systemState;
    private IDisposable beaconTimer;

    private CLLocation UserLocation => AppContext.Shared.GeolocationManager.Location;
    
    private CLLocation DefaultBeaconLocation
    {
        get
        {
            var heading = AppContext.Shared.GeolocationManager.PresentationHeading.Value;
            if (UserLocation != null && heading != null)
            {
                var distance = 100;
                var bearing = heading.DoubleValue;
                return UserLocation.Coordinate.Destination(distance, bearing);
            }
            return null;
        }
    }

    public void Prepare(bool disableMelodies = true)
    {
        var calloutsEnabled = SettingsContext.Shared.AutomaticCalloutsEnabled;
        var beaconMelodiesEnabled = SettingsContext.Shared.PlayBeaconStartAndEndMelodies;
        var beaconManager = AppContext.Shared.SpatialDataContext.DestinationManager;

        BeaconType? originalBeacon = null;
        var refDestination = beaconManager.Destination;
        if (refDestination != null)
        {
            if (!refDestination.IsTemp)
            {
                originalBeacon = BeaconType.Ref;
            }
            else
            {
                var poi = refDestination.GetPOI();
                if (poi is GenericLocation loc)
                {
                    originalBeacon = BeaconType.Location;
                }
                else
                {
                    originalBeacon = BeaconType.Entity;
                }
            }
        }

        systemState = new AudioSystemState
        {
            OriginalBeacon = originalBeacon,
            WasBeaconEnabled = beaconManager.IsAudioEnabled,
            WereBeaconMelodiesDisabled = disableMelodies,
            OriginalBeaconMelodiesState = beaconMelodiesEnabled,
            WereCalloutsEnabled = calloutsEnabled
        };

        AppContext.Shared.EventProcessor.Hush(playSound: false);
        SettingsContext.Shared.AutomaticCalloutsEnabled = false;

        if (disableMelodies)
        {
            SettingsContext.Shared.PlayBeaconStartAndEndMelodies = false;
        }
    }

    public void Play(bool styleChanged = false, bool shouldTimeOut = true, CLLocation newBeaconLocation = null, string logContext = "volume_controls.demo")
    {
        var beaconManager = AppContext.Shared.SpatialDataContext.DestinationManager;
        var currentBeaconLocation = newBeaconLocation ?? DefaultBeaconLocation;

        if (beaconTimer != null)
        {
            beaconTimer.Dispose();
            if (styleChanged && currentBeaconLocation != null)
            {
                AppContext.Shared.EventProcessor.Hush(playSound: false);
                beaconManager.SetDestination(currentBeaconLocation, null, true, UserLocation, logContext);
            }
            if (shouldTimeOut)
            {
                beaconTimer = Observable.Timer(TimeSpan.FromSeconds(8))
                    .Subscribe(_ =>
                    {
                        AppContext.Shared.EventProcessor.Hush(playSound: false);
                        beaconTimer.Dispose();
                    });
            }
            return;
        }

        AppContext.Shared.EventProcessor.Hush(playSound: false);
        beaconTimer?.Dispose();

        if (currentBeaconLocation == null)
        {
            return;
        }

        shouldRestoreState = true;

        beaconManager.SetDestination(currentBeaconLocation, null, true, UserLocation, logContext);

        if (shouldTimeOut)
        {
            beaconTimer = Observable.Timer(TimeSpan.FromSeconds(8))
                .Subscribe(_ =>
                {
                    AppContext.Shared.EventProcessor.Hush(playSound: false);
                    beaconTimer.Dispose();
                });
        }
    }

    public void UpdateBeaconLocation(CLLocation newBeaconLocation)
    {
        if (UserLocation != null)
        {
            AppContext.Shared.SpatialDataContext.DestinationManager.UpdateDestinationLocation(newBeaconLocation, UserLocation);
        }
    }

    public void RestoreState(string logContext = "volume_controls.demo")
    {
        beaconTimer?.Dispose();
        beaconTimer = null;

        if (systemState == null)
        {
            return;
        }

        SettingsContext.Shared.AutomaticCalloutsEnabled = systemState.Value.WereCalloutsEnabled;

        if (systemState.Value.WereBeaconMelodiesDisabled)
        {
            SettingsContext.Shared.PlayBeaconStartAndEndMelodies = systemState.Value.OriginalBeaconMelodiesState;
        }

        if (!shouldRestoreState && !systemState.Value.WasBeaconEnabled)
        {
            return;
        }

        AppContext.Shared.EventProcessor.Hush(playSound: false);
        var beaconManager = AppContext.Shared.SpatialDataContext.DestinationManager;
        beaconManager.ClearDestination(logContext);

        if (systemState.Value.OriginalBeacon == null)
        {
            return;
        }

        switch (systemState.Value.OriginalBeacon)
        {
            case BeaconType.Location:
                beaconManager.SetDestination(systemState.Value.OriginalBeacon, null, systemState.Value.WasBeaconEnabled, UserLocation, logContext + ".restoring_original");
                break;
            case BeaconType.Entity:
                beaconManager.SetDestination(systemState.Value.OriginalBeacon, null, systemState.Value.WasBeaconEnabled, UserLocation, systemState.Value.OriginalBeaconMelodiesState, logContext + ".restoring_original");
                break;
            case BeaconType.Ref:
                beaconManager.SetDestination(systemState.Value.OriginalBeacon, null, systemState.Value.WasBeaconEnabled, UserLocation, logContext + ".restoring_original");
                break;
        }
    }
}
