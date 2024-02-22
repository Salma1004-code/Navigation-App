using AVFoundation;
using CoreLocation;

public interface IDynamicSound<TAssetType> where TAssetType : IDynamicAudioEngineAsset
{
    AVAudioFormat CommonFormat { get; }
    
    BeaconAccents? IntroAsset { get; }
    
    BeaconAccents? OutroAsset { get; }

    (TAssetType Asset, TAssetType.Volume Volume)? GetAsset(CLLocationDirection? userHeading, CLLocation userLocation);

    (TAssetType Asset, TAssetType.Volume Volume)? GetAsset(CLLocation userLocation);

    AVAudioPCMBuffer GetBuffer(TAssetType asset);

    AVAudioPCMBuffer GetBuffer(BeaconAccents melody);
}
