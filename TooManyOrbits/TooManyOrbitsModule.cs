using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using TooManyOrbits.UI;
using UnityEngine;

namespace TooManyOrbits
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class TooManyOrbitsModule : MonoBehaviour
	{
        static internal TooManyOrbitsModule Instance;

        public const string ModName = "TooManyOrbits";
		public const string ResourcePath = ModName + "/";

		private string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
		private string ConfigurationFile => $"GameData/{ModName}/PluginData/{ModName}.cfg";

		private KeyCode m_toggleButton;
		private ToolbarButton m_toolbarButton;
		internal ConfigurationWindow m_window;
		private Configuration m_configuration;
		private IVisibilityController m_visibilityController;
		private bool m_lastVisibilityState = true;
		private bool m_skipUpdate = false;

		private void Start()
		{
			Log.Info($"Starting {ModName} v{Version}...");
            Instance = this;

			var resourceProvider = new ResourceProvider(ModName);

			Log.Debug("Loading configuration");
			m_configuration = ConfigurationParser.LoadFromFile(ConfigurationFile);
			m_configuration.PropertyChanged += OnConfigurationChanged;
			m_toggleButton = m_configuration.ToggleKey;

			Log.Debug("Setting up OrbitVisibilityController");
			m_visibilityController = new OrbitVisibilityController(m_configuration);
			m_visibilityController.OnVisibilityChanged += OnOrbitVisibilityChanged;

			// setup window
			Log.Debug("Creating window");
			m_window = new ConfigurationWindow(ModName, m_configuration, m_visibilityController, resourceProvider);

			// setup toolbar button
			Log.Debug("Creating toolbar button");
            m_toolbarButton = new ToolbarButton(this.gameObject, resourceProvider);


			// get notifcations when player changes to map view
			MapView.OnEnterMapView += OnEnterMapView;
			MapView.OnExitMapView += OnExitMapView;

			// disable script until woken up by entering map view
			enabled = false;
		}

		[SuppressMessage("ReSharper", "DelegateSubtraction")]
		private void OnDestroy()
		{
			Log.Info("Shutting down");
			MapView.OnEnterMapView -= OnEnterMapView;
			MapView.OnExitMapView -= OnExitMapView;

			Log.Debug("Disposing ToolbarButton");
			m_toolbarButton.Dispose();

			Log.Debug("Disposing window");
			m_window.Dispose();

			Log.Debug("Disposing OrbitVisibilityController");
			m_visibilityController.OnVisibilityChanged -= OnOrbitVisibilityChanged;
			m_visibilityController.Dispose();

			Log.Debug("Writing configuration file");
			string configurationDirectory = Path.GetDirectoryName(ConfigurationFile);
			if (configurationDirectory != null && !Directory.Exists(configurationDirectory))
			{
				Directory.CreateDirectory(configurationDirectory);
			}
			ConfigurationParser.SaveToFile(ConfigurationFile, m_configuration);
		}

		private void Update()
		{
			if (m_skipUpdate)
			{
				m_skipUpdate = false;
				return;
			}

			if (Input.GetKeyDown(m_toggleButton))
			{
				m_visibilityController.Toggle();
			}
		}

		private void OnEnterMapView()
		{
			Log.Debug("Enabling...");
			enabled = true;

			if (!m_lastVisibilityState)
			{
				m_visibilityController.Hide();
			}
			m_toolbarButton.Show();
		}

		private void OnExitMapView()
		{
			Log.Debug("Disabling...");
			enabled = false;

			m_lastVisibilityState = m_visibilityController.IsVisible;
			m_visibilityController.Show();
			m_toolbarButton.Hide();
		}

		private void OnOrbitVisibilityChanged(bool orbitsVisible)
		{
			const float duration = 1.5f;
			if (enabled)
			{
				var message = orbitsVisible ? "Orbits shown" : "Orbits hidden";
				ScreenMessages.PostScreenMessage(message, duration);
			}
		}

		private void OnGUI()
		{
			var originalSkin = GUI.skin;
			GUI.skin = HighLogic.Skin;

			m_window.Draw();

			GUI.skin = originalSkin;
		}

		private void OnConfigurationChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args?.PropertyName == nameof(Configuration.ToggleKey))
			{
				m_toggleButton = m_configuration.ToggleKey;
				m_skipUpdate = true;
				Log.Info($"Changed toggle key to '{m_configuration.ToggleKey}'");
			}
		}
	}
}
