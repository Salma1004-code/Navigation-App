using Foundation;
using System;
using System.Collections.Generic;

public static class Keys
{
    // MARK: Internal UserDefaults Keys
    public static readonly string appUseCount = "GDAAppUseCount";
    public static readonly string newFeaturesLastDisplayedVersion = "GDANewFeaturesLastDisplayedVersion";
    public static readonly string clientIdentifier = "GDAUserDefaultClientIdentifier";
    public static readonly string metricUnits = "GDASettingsMetric";
    public static readonly string locale = "GDASettingsLocaleIdentifier";
    public static readonly string voiceID = "GDAAppleSynthVoice";
    public static readonly string speakingRate = "GDASettingsSpeakingRate";
    public static readonly string beaconVolume = "GDABeaconVolume";
    public static readonly string ttsVolume = "GDATTSVolume";
    public static readonly string otherVolume = "GDAOtherVolume";
    public static readonly string ttsGain = "GDATTSAudioGain";
    public static readonly string beaconGain = "GDABeaconAudioGain";
    public static readonly string afxGain = "GDAAFXAudioGain";
    public static readonly string telemetryOptout = "GDASettingsTelemetryOptout";
    public static readonly string selectedBeaconName = "GDASelectedBeaconName";
    public static readonly string useOldBeacon = "GDASettingsUseOldBeacon";
    public static readonly string playBeaconStartEndMelody = "GDAPlayBeaconStartEndMelody";
    public static readonly string automaticCalloutsEnabled = "GDASettingsAutomaticCalloutsEnabled";
    public static readonly string sensePlace = "GDASettingsPlaceSenseEnabled";
    public static readonly string senseLandmark = "GDASettingsLandmarkSenseEnabled";
    public static readonly string senseMobility = "GDASettingsMobilitySenseEnabled";
    public static readonly string senseInformation = "GDASettingsInformationSenseEnabled";
    public static readonly string senseSafety = "GDASettingsSafetySenseEnabled";
    public static readonly string senseIntersection = "GDASettingsIntersectionsSenseEnabled";
    public static readonly string senseDestination = "GDASettingsDestinationSenseEnabled";
    public static readonly string apnsDeviceToken = "GDASettingsAPNsDeviceToken";
    public static readonly string pushNotificationTags = "GDASettingsPushNotificationTags";
    public static readonly string previewIntersectionsIncludeUnnamedRoads = "GDASettingsPreviewIntersectionsIncludeUnnamedRoads";
    public static readonly string audioSessionMixesWithOthers = "GDAAudioSessionMixesWithOthers";
    public static readonly string markerSortStyle = "GDAMarkerSortStyle";
    // MARK: Notification Keys
    public static readonly string enabled = "GDAEnabled";
}

public class SettingsContext
{
    // MARK: Shared Instance
    public static SettingsContext Shared { get; } = new SettingsContext();

    // MARK: User Defaults
    private NSUserDefaults userDefaults => NSUserDefaults.StandardUserDefaults;

    // MARK: Initialization
    public SettingsContext()
    {
        // register default values
        userDefaults.RegisterDefaults(new NSDictionary<NSString, NSObject>(
            new NSString(Keys.appUseCount), NSNumber.FromInt32(0),
            new NSString(Keys.newFeaturesLastDisplayedVersion), new NSString("0.0.0"),
            new NSString(Keys.metricUnits), NSNumber.FromBoolean(NSLocale.CurrentLocale.UsesMetricSystem),
            new NSString(Keys.speakingRate), NSNumber.FromFloat(0.55f),
            new NSString(Keys.beaconVolume), NSNumber.FromFloat(0.75f),
            new NSString(Keys.ttsVolume), NSNumber.FromFloat(0.75f),
            new NSString(Keys.otherVolume), NSNumber.FromFloat(0.75f),
            new NSString(Keys.ttsGain), NSNumber.FromFloat(5f),
            new NSString(Keys.beaconGain), NSNumber.FromFloat(5f),
            new NSString(Keys.afxGain), NSNumber.FromFloat(5f),
            new NSString(Keys.telemetryOptout), NSNumber.FromBoolean(BuildSettings.Configuration != BuildConfiguration.Release),
            new NSString(Keys.selectedBeaconName), new NSString(V2Beacon.Description),
            new NSString(Keys.useOldBeacon), NSNumber.FromBoolean(false),
            new NSString(Keys.playBeaconStartEndMelody), NSNumber.FromBoolean(false),
            new NSString(Keys.automaticCalloutsEnabled), NSNumber.FromBoolean(true),
            new NSString(Keys.sensePlace), NSNumber.FromBoolean(true),
            new NSString(Keys.senseLandmark), NSNumber.FromBoolean(true),
            new NSString(Keys.senseMobility), NSNumber.FromBoolean(true),
            new NSString(Keys.senseInformation), NSNumber.FromBoolean(true),
            new NSString(Keys.senseSafety), NSNumber.FromBoolean(true),
            new NSString(Keys.senseIntersection), NSNumber.FromBoolean(true),
            new NSString(Keys.senseDestination), NSNumber.FromBoolean(true),
            new NSString(Keys.previewIntersectionsIncludeUnnamedRoads), NSNumber.FromBoolean(false),
            new NSString(Keys.audioSessionMixesWithOthers), NSNumber.FromBoolean(true),
            new NSString(Keys.markerSortStyle), new NSString(SortStyle.Distance.ToString())
        ));

        ResetLocaleIfNeeded();
    }

    private void ResetLocaleIfNeeded()
    {
        // If the user has selected a locale in the first launch experience, but did not finish the first
        // launch experience and terminated the app half way, reset the chosen locale when re-opening the app.
        if (!FirstUseExperience.DidComplete(FirstUseExperienceType.OOBE) && Locale != null)
        {
            Locale = null;
        }
    }

    // MARK: Properties
    public int AppUseCount
    {
        get => (int)userDefaults.IntForKey(Keys.appUseCount);
        set => userDefaults.SetInt(value, Keys.appUseCount);
    }

    public string NewFeaturesLastDisplayedVersion
    {
        get
        {
            var versionString = userDefaults.StringForKey(Keys.newFeaturesLastDisplayedVersion);
            if (versionString == null)
            {
                userDefaults.SetString("0.0.0", Keys.newFeaturesLastDisplayedVersion);
                versionString = "0.0.0";
            }
            return versionString;
        }
        set => userDefaults.SetString(value, Keys.newFeaturesLastDisplayedVersion);
    }

    public string ClientId
    {
        get
        {
            var clientId = userDefaults.StringForKey(Keys.clientIdentifier);
            if (clientId == null)
            {
                clientId = Guid.NewGuid().ToString();
                userDefaults.SetString(clientId, Keys.clientIdentifier);
            }
            return clientId;
        }
        set => userDefaults.SetString(value, Keys.clientIdentifier);
    }

    public bool MetricUnits
    {
        get => userDefaults.BoolForKey(Keys.metricUnits);
        set => userDefaults.SetBool(value, Keys.metricUnits);
    }

    public NSLocale Locale
    {
        get
        {
            var identifier = userDefaults.StringForKey(Keys.locale);
            if (identifier == null) return null;

            if (_locale != null && _locale.Identifier == identifier) return _locale;

            _locale = new NSLocale(identifier);
            return _locale;
        }
        set
        {
            if (value != null) userDefaults.SetString(value.Identifier, Keys.locale);
            else userDefaults.RemoveObject(Keys.locale);
        }
    }

    public float SpeakingRate
    {
        get => userDefaults.FloatForKey(Keys.speakingRate);
        set => userDefaults.SetFloat(value, Keys.speakingRate);
    }

    public float BeaconVolume
    {
        get => userDefaults.FloatForKey(Keys.beaconVolume);
        set
        {
            userDefaults.SetFloat(value, Keys.beaconVolume);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.beaconVolumeChanged, null);
        }
    }

    public float TtsVolume
    {
        get => userDefaults.FloatForKey(Keys.ttsVolume);
        set
        {
            userDefaults.SetFloat(value, Keys.ttsVolume);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.ttsVolumeChanged, null);
        }
    }

    public float OtherVolume
    {
        get => userDefaults.FloatForKey(Keys.otherVolume);
        set
        {
            userDefaults.SetFloat(value, Keys.otherVolume);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.otherVolumeChanged, null);
        }
    }

    public float TtsGain
    {
        get => userDefaults.FloatForKey(Keys.ttsGain);
        set => userDefaults.SetFloat(value, Keys.ttsGain);
    }

    public float BeaconGain
    {
        get => userDefaults.FloatForKey(Keys.beaconGain);
        set
        {
            userDefaults.SetFloat(value, Keys.beaconGain);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.beaconGainChanged, null);
        }
    }

    public float AfxGain
    {
        get => userDefaults.FloatForKey(Keys.afxGain);
        set => userDefaults.SetFloat(value, Keys.afxGain);
    }

    public bool TelemetryOptout
    {
        get => userDefaults.BoolForKey(Keys.telemetryOptout);
        set => userDefaults.SetBool(value, Keys.telemetryOptout);
    }

    public bool PreviewIntersectionsIncludeUnnamedRoads
    {
        get => userDefaults.BoolForKey(Keys.previewIntersectionsIncludeUnnamedRoads);
        set
        {
            userDefaults.SetBool(value, Keys.previewIntersectionsIncludeUnnamedRoads);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.previewIntersectionsIncludeUnnamedRoadsDidChange, null,
                new NSDictionary<NSString, NSObject>(Keys.enabled, NSNumber.FromBoolean(value)));
        }
    }

    public bool AudioSessionMixesWithOthers
    {
        get => userDefaults.BoolForKey(Keys.audioSessionMixesWithOthers);
        set => userDefaults.SetBool(value, Keys.audioSessionMixesWithOthers);
    }

    // MARK: Audio Beacon

    public string SelectedBeacon
    {
        get
        {
            var selected = userDefaults.StringForKey(Keys.selectedBeaconName);
            if (selected == null)
            {
                if (userDefaults.BoolForKey(Keys.useOldBeacon)) return ClassicBeacon.Description;
                return V2Beacon.Description;
            }
            return selected;
        }
        set => userDefaults.SetString(value, Keys.selectedBeaconName);
    }

    public bool PlayBeaconStartAndEndMelodies
    {
        get => userDefaults.BoolForKey(Keys.playBeaconStartEndMelody);
        set => userDefaults.SetBool(value, Keys.playBeaconStartEndMelody);
    }

    // MARK: Push Notifications

    public NSData ApnsDeviceToken
    {
        get => userDefaults.DataForKey(Keys.apnsDeviceToken);
        set => userDefaults.SetData(value, Keys.apnsDeviceToken);
    }

    public NSSet<string> PushNotificationTags
    {
        get
        {
            var array = userDefaults.StringArrayForKey(Keys.pushNotificationTags);
            return array != null ? new NSSet<string>(array) : null;
        }
        set
        {
            if (value != null) userDefaults.SetStringArray(value.ToArray(), Keys.pushNotificationTags);
            else userDefaults.RemoveObject(Keys.pushNotificationTags);
        }
    }

    // MARK: Apple TTS

    public string VoiceId
    {
        get => userDefaults.StringForKey(Keys.voiceID);
        set => userDefaults.SetString(value, Keys.voiceID);
    }

    // MARK: Markers and Routes List

    public SortStyle DefaultMarkerSortStyle
    {
        get
        {
            var sortString = userDefaults.StringForKey(Keys.markerSortStyle);
            return sortString != null && Enum.TryParse(sortString, out SortStyle sort) ? sort : SortStyle.Distance;
        }
        set => userDefaults.SetString(value.ToString(), Keys.markerSortStyle);
    }

    public bool AutomaticCalloutsEnabled
    {
        get => userDefaults.BoolForKey(Keys.automaticCalloutsEnabled);
        set
        {
            userDefaults.SetBool(value, Keys.automaticCalloutsEnabled);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.automaticCalloutsEnabledChanged, null,
                new NSDictionary<NSString, NSObject>(Keys.enabled, NSNumber.FromBoolean(value)));
        }
    }

    public bool PlaceSenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.sensePlace);
        set
        {
            userDefaults.SetBool(value, Keys.sensePlace);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.autoCalloutCategorySenseChanged, null);
        }
    }

    public bool LandmarkSenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.senseLandmark);
        set
        {
            userDefaults.SetBool(value, Keys.senseLandmark);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.autoCalloutCategorySenseChanged, null);
        }
    }

    public bool MobilitySenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.senseMobility);
        set
        {
            userDefaults.SetBool(value, Keys.senseMobility);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.autoCalloutCategorySenseChanged, null);
        }
    }

    public bool InformationSenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.senseInformation);
        set
        {
            userDefaults.SetBool(value, Keys.senseInformation);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.autoCalloutCategorySenseChanged, null);
        }
    }

    public bool SafetySenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.senseSafety);
        set
        {
            userDefaults.SetBool(value, Keys.senseSafety);
            NSNotificationCenter.DefaultCenter.PostNotificationName(Keys.autoCalloutCategorySenseChanged, null);
        }
    }

    public bool IntersectionSenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.senseIntersection);
        set => userDefaults.SetBool(value, Keys.senseIntersection);
    }

    public bool DestinationSenseEnabled
    {
        get => userDefaults.BoolForKey(Keys.senseDestination);
        set => userDefaults.SetBool(value, Keys.senseDestination);
    }
}

