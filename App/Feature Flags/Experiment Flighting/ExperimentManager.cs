using System;
using System.IO;
using System.Threading.Tasks;

namespace Soundscape
{
    public class ExperimentManager
    {
        private readonly ExperimentServiceModel service = new ExperimentServiceModel();
        private readonly object lockObject = new object();
        private Flight currentFlight;

        public event EventHandler ExperimentManagerReady;

        public ExperimentManager()
        {
            currentFlight = LoadConfiguration();
        }

        public void Initialize()
        {
            var shouldDownloadDescriptions = FeatureFlag.IsEnabled(Feature.ExperimentConfiguration);

            var configurationDidComplete = false;
            var descriptionsDidComplete = shouldDownloadDescriptions ? false : true;

            service.GetExperimentConfiguration(currentFlight?.Etag, (result) =>
            {
                lock (lockObject)
                {
                    if (configurationDidComplete)
                        return;

                    configurationDidComplete = true;

                    switch (result)
                    {
                        case Result<Flight>.Success var flight:
                            currentFlight = flight.Data;
                            SaveConfiguration(currentFlight);
                            break;
                        case Result<Flight>.Failure var error:
                            if (error.Error is ExperimentServiceError err && err == ExperimentServiceError.NotModified)
                                GDLogAppInfo("Flight controls have not changed since last downloaded...");
                            else
                                GDLogAppError($"Error downloading flight controls: {error.Error.Message}");
                            break;
                    }

                    if (descriptionsDidComplete)
                        ExperimentManagerReady?.Invoke(this, EventArgs.Empty);
                }
            });

            if (!shouldDownloadDescriptions)
                return;

            service.GetExperimentDescriptions((result) =>
            {
                lock (lockObject)
                {
                    if (descriptionsDidComplete)
                        return;

                    descriptionsDidComplete = true;

                    switch (result)
                    {
                        case Result<ExperimentDescription[]>.Success var descriptions:
                            currentFlight.ExperimentDescriptions = descriptions.Data;
                            break;
                        case Result<ExperimentDescription[]>.Failure var error:
                            GDLogAppError($"Error downloading flight controls: {error.Error.Message}");
                            break;
                    }

                    if (configurationDidComplete)
                        ExperimentManagerReady?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        public bool IsEnabled(KnownExperiment experiment, Locale locale = null)
        {
            return currentFlight?.IsActive(experiment.Uuid, locale ?? LocalizationContext.CurrentAppLocale) ?? false;
        }

        public void SetIsEnabled(Guid uuid, bool isEnabled)
        {
            if (!FeatureFlag.IsEnabled(Feature.ExperimentConfiguration))
                return;

            currentFlight?.SetIsActive(uuid, isEnabled);
        }

        private Flight LoadConfiguration()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Experiments", "controls.json");

            if (!File.Exists(filePath))
                return null;

            try
            {
                var data = File.ReadAllText(filePath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Flight>(data);
            }
            catch (Exception ex)
            {
                GDLogAppError($"Unable to save current flight state: {ex.Message}");
                return null;
            }
        }

        private void SaveConfiguration(Flight configuration)
        {
            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Experiments");
            var filePath = Path.Combine(directoryPath, "controls.json");

            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var data = Newtonsoft.Json.JsonConvert.SerializeObject(configuration);
                File.WriteAllText(filePath, data);
            }
            catch (Exception ex)
            {
                GDLogAppError($"Unable to save current flight state: {ex.Message}");
            }
        }
    }
}
