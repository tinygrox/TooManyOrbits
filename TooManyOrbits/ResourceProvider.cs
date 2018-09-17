using UnityEngine;
using ToolbarControl_NS;

namespace TooManyOrbits
{
	internal class ResourceProvider
	{
        private readonly string m_resourcePath;
        private readonly string m_resourcePathNoGameData;


		public Texture ToolbarIcon => LoadTextureResource("ToolbarIcon-38");
        public Texture GreenToolbarIcon => LoadTextureResource("ToolbarIcon-Green");
		public Texture PencilIcon => LoadTextureResource("Pencil");
		public Texture ExpandIcon => LoadTextureResource("Expand");
		public Texture RetractIcon => LoadTextureResource("Retract");
		public Texture MoveIcon => LoadTextureResource("Move");


		public ResourceProvider(string resourcePath)
		{
			m_resourcePath = resourcePath;
			if (!m_resourcePath.EndsWith("/"))
			{
				m_resourcePath += '/';
			}
            m_resourcePathNoGameData = m_resourcePath;
            m_resourcePath = "GameData/" + m_resourcePath;
            

        }

		private Texture LoadTextureResource(string resourceName)
		{
			string path = BuildPath(resourceName);
			Log.Debug("Loading texture: " + path);

            Texture2D texture = new Texture2D(2,2);
                
            if (! ToolbarControl.LoadImageFromFile(ref texture, path))
			{
				Log.Error("Failed to load texture " + resourceName);
			}
			return texture;
		}

		internal string BuildPath(string resourceName, bool gamedata = true)
		{
            if (gamedata)
			    return m_resourcePath + "PluginData/Images/" + resourceName;
            else
                return m_resourcePathNoGameData + "PluginData/Images/" + resourceName;
        }
	}
}
