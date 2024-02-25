using System;
using System.Collections.Generic;
using UIKit;

public struct FeatureInfo
{
    // Properties
    private readonly string titleKey;
    private readonly string descriptionKey;
    private readonly string accessibilityDescriptionKey;
    private readonly string imageFilename;
    private readonly string buttonLabelKey;
    private readonly string buttonAccessibilityHintKey;

    public readonly int? hyperlinkStart;
    public readonly int? hyperlinkLength;
    public readonly string hyperlinkURL;
    public readonly VersionString version;
    public readonly int order;

    // Computed Properties
    public string LocalizedTitle => GDLocalizedString(titleKey);
    public string LocalizedDescription => GDLocalizedString(descriptionKey);
    public string LocalizedAccessibilityDescription => GDLocalizedString(accessibilityDescriptionKey);
    public UIImage LocalizedImage
    {
        get
        {
            if (string.IsNullOrEmpty(imageFilename))
                return null;

            var localizedImage = UIImage.FromBundle(imageFilename + "_" + LocalizationContext.CurrentAppLocale.IdentifierHyphened);
            if (localizedImage != null)
                return localizedImage;

            return UIImage.FromBundle(imageFilename + "_" + LocalizationContext.DevelopmentLocale.IdentifierHyphened);
        }
    }
    public string ButtonLabel => buttonLabelKey != null ? GDLocalizedString(buttonLabelKey) : null;
    public string ButtonAccessibilityHint => buttonAccessibilityHintKey != null ? GDLocalizedString(buttonAccessibilityHintKey) : null;

    // Initialization
    public FeatureInfo(VersionString version, Dictionary<string, string> properties)
    {
        titleKey = properties.ContainsKey("Title") ? properties["Title"] : "";
        descriptionKey = properties.ContainsKey("Description") ? properties["Description"] : "";
        accessibilityDescriptionKey = properties.ContainsKey("AccessibilityDescription") ? properties["AccessibilityDescription"] : "";
        hyperlinkStart = properties.ContainsKey("HyperlinkStart") ? int.Parse(properties["HyperlinkStart"]) : null as int?;
        hyperlinkLength = properties.ContainsKey("HyperlinkLength") ? int.Parse(properties["HyperlinkLength"]) : null as int?;
        hyperlinkURL = properties.ContainsKey("HyperlinkURL") ? properties["HyperlinkURL"] : null;
        imageFilename = properties.ContainsKey("Image") ? properties["Image"] : null;
        buttonLabelKey = properties.ContainsKey("ButtonLabel") ? properties["ButtonLabel"] : null;
        buttonAccessibilityHintKey = properties.ContainsKey("ButtonAccessibilityHint") ? properties["ButtonAccessibilityHint"] : null;
        this.version = version;
        order = properties.ContainsKey("Order") ? int.Parse(properties["Order"]) : -1;
    }
}

public class NewFeatures
{
    public static VersionString CurrentVersion => new VersionString(NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString());
    public static VersionString LastDisplayedVersion => new VersionString(SettingsContext.Shared.NewFeaturesLastDisplayedVersion);
    public Dictionary<VersionString, List<FeatureInfo>> Features { get; private set; } = new Dictionary<VersionString, List<FeatureInfo>>();

    public NewFeatures()
    {
        var allFeaturesHistory = allFeaturesHistory();
        var lastDisplayedVersion = LastDisplayedVersion;

        foreach (var kvp in allFeaturesHistory)
        {
            if (kvp.Key > lastDisplayedVersion)
                Features[kvp.Key] = kvp.Value;
        }
    }

    public static Dictionary<VersionString, List<FeatureInfo>> allFeaturesHistory()
    {
        var path = NSBundle.MainBundle.PathForResource("NewFeatures", "plist");
        if (path == null)
            return null;

        var dict = NSDictionary.FromFile(path) as NSDictionary;
        if (dict == null)
            return null;

        var allFeaturesHistory = new Dictionary<VersionString, List<FeatureInfo>>();
        foreach (var key in dict.Keys)
        {
            var versionKey = new VersionString(key.ToString());
            var featuresValue = dict.ValueForKey(key) as NSArray;

            var allVersionFeatures = new List<FeatureInfo>();
            for (nuint i = 0; i < featuresValue.Count; i++)
            {
                var feature = featuresValue.GetItem<NSDictionary>(i);
                var properties = new Dictionary<string, string>();
                foreach (var propertyKey in feature.Keys)
                {
                    properties[propertyKey.ToString()] = feature.ValueForKey(propertyKey).ToString();
                }
                allVersionFeatures.Add(new FeatureInfo(versionKey, properties));
            }

            allFeaturesHistory[versionKey] = allVersionFeatures;
        }

        return allFeaturesHistory;
    }

    public bool ShouldShowNewFeatures()
    {
        if (LastDisplayedVersion >= CurrentVersion)
            return false;

        return Features.Any(pair => pair.Key > LastDisplayedVersion);
    }

    public void NewFeaturesDidShow()
    {
        SettingsContext.Shared.NewFeaturesLastDisplayedVersion = CurrentVersion.String;
    }
}
