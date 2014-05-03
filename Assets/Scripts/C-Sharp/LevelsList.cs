using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scenes.Scripts
{
	/// <summary>
	/// A context that is always available in the game. It provides global game features and information.
	/// </summary>
	public static class Game
	{
		#region Properties
		/// <summary>
		/// The folder the levels file is located when the game is hosted in the editor.
		/// </summary>
		private const string EDITOR_LEVELS_FILE_DIRECTORY = "Assets/Scenes/";
		/// <summary>
		/// The folder the levels file is located when the game has been deployed.
		/// </summary>
		private const string BUILD_LEVELS_FILE_DIRECTORY = "";
		/// <summary>
		/// The filename and extension of the file that contains the available levels.
		/// </summary>
		private const string LEVEL_FILE_NAME = "Levels.ini";
		
		/// <summary>
		/// The name of the world object. The world object and all of it's children are notified of game events.
		/// </summary>
		private const string WORLD_OBJECT = "World";
		/// <summary>
		/// The name of the game paused event.
		/// </summary>
		private const string ON_GAME_PAUSED_EVENT = "OnGamePaused";
		/// <summary>
		/// The name of the game resumed event.
		/// </summary>
		private const string ON_GAME_RESUMED_EVENT = "OnGameResumed";
		
		/// <summary>
		/// Indicates if the levels file has been updated this run. If so, it is not updated again. This only applies to the game when it is hosted
		/// in the editor.
		/// </summary>
		private static bool _hasUpdatedLevelsFile;
		/// <summary>
		/// The names of all levels in the game.
		/// </summary>
		private static string[] _levels;
		
		/// <summary>
		/// The previous Time.timeScale. This value is used to resume the game at the same pace it was paused at.
		/// </summary>
		private static float _previousTimeScale;
		/// <summary>
		/// Indicates if the game is currently paused.
		/// </summary>
		private static bool _isGamePaused;
		
		/// <summary>
		/// Gets the names of all levels in the game.
		/// </summary>
		public static string[] Levels
		{
			get
			{
				if (_levels == null)
				{
					string directory;
					
					// The directory depends on the environment. In the editor, relative paths can be used.
					if (Application.isEditor)
					{
						directory = EDITOR_LEVELS_FILE_DIRECTORY;
					}
					else
					{
						string dataPath = Application.dataPath;
						
						Debug.Log( string.Format("Data path detected at '{0}'.", dataPath) );
						
						directory = Path.Combine(dataPath ?? string.Empty, BUILD_LEVELS_FILE_DIRECTORY);
					}
					
					_levels = ReadLevelsFile(directory);
					
					Debug.Log( string.Format( "Discovered level names: {0}.", string.Join( ", ", _levels ) ) );
				}
				
				return _levels;
			}
		}
		
		/// <summary>
		/// Gets a value that indicates if the game is currently paused.
		/// </summary>
		public static bool IsGamePaused
		{
			get { return _isGamePaused; }
		}
		#endregion
		
		#region Constructors & Destructors
		/// <summary>
		/// Initializes the Game.
		/// </summary>
		static Game()
		{
			// Get an initial previous time scale so that we won't accidentally resume with a scale of 0.
			_previousTimeScale = Time.timeScale;
		}
		#endregion
		
		#region Game Management
		/// <summary>
		/// Pauses the game. The game paused event is fired for all game objects in the world object.
		/// </summary>
		public static void Pause()
		{
			Debug.Log("Game pausing.");
			
			// Pause the game and indicate that the game is actually paused.
			_previousTimeScale = Time.timeScale;
			Time.timeScale = 0.0f;
			_isGamePaused = true;
			
			// Give every game object in the world the chance to react to the game pausing.
			foreach (GameObject @object in GameObject.FindGameObjectsWithTag(WORLD_OBJECT))
			{
				@object.BroadcastMessage(ON_GAME_PAUSED_EVENT, SendMessageOptions.DontRequireReceiver);
			}
			
			Debug.Log(String.Format("Game paused on Time.TimeScale = {0}.", Time.timeScale));
		}
		
		/// <summary>
		/// Resumes the game. The game resumed event is fired for all game objects in the world object.
		/// </summary>
		public static void Resume()
		{
			Debug.Log( "Game resuming." );
			
			// Indicate that the game is unpaused. Should anyone check this value, it would be in the game resume event, which is called just before the game is
			// actually resumed.
			_isGamePaused = false;
			
			// Give every game object in the world the chance to react to the game pausing.
			foreach (GameObject @object in GameObject.FindGameObjectsWithTag(WORLD_OBJECT))
			{
				@object.BroadcastMessage(ON_GAME_RESUMED_EVENT, SendMessageOptions.DontRequireReceiver);
			}
			
			// Unpause the game.
			Time.timeScale = _previousTimeScale;
			
			Debug.Log(String.Format("Game resumed on Time.TimeScale = {0}.", Time.timeScale));
		}
		#endregion
		
		#region Level Management
		#if UNITY_EDITOR
		/// <summary>
		/// This method is called when post-processing a build, which occurs after a build has been made. This method updates the levels file in the build directory.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="pathToBuiltProject"></param>
		[UnityEditor.Callbacks.PostProcessBuild]
		public static void PostProcessBuild(UnityEditor.BuildTarget target, string pathToBuiltProject)
		{
			const string DATA_FOLDER = "{0}_Data";
			
			Debug.Log(string.Format("Post-processing build '{0}' at '{1}'.", target, pathToBuiltProject));
			
			// The file name is integrated in some folder/file names of the built game. It may be needed to create references to these dynamic folders/files.
			string fileName = Path.GetFileNameWithoutExtension( pathToBuiltProject );
			// The build directory is the build path, without file name and extension and appended with the custom path.
			string buildDirectory = Path.Combine( Path.Combine( Path.GetDirectoryName( pathToBuiltProject ) ?? string.Empty,
			                                                   string.Format( DATA_FOLDER, fileName ) ),
			                                     BUILD_LEVELS_FILE_DIRECTORY );
			
			Debug.Log(string.Format("Detected levels file directory '{0}'.", buildDirectory));
			
			WriteLevelsFile(buildDirectory);
			
			Debug.Log("Post-processed build.");
		}
		
		/// <summary>
		/// This method is called when post-processing a scene, which occurs in either the editor when running a scene or at build time when building a scene. This
		/// method updates the levels file, if applicable.
		/// </summary>
		[UnityEditor.Callbacks.PostProcessScene]
		public static void PostProcessScene()
		{
			Debug.Log( "Post-processing scene." );
			
			if ( !_hasUpdatedLevelsFile )
			{
				// Only write a levels file if we're in the editor. If not, the PostProcessBuild method will do this, because the PostProcessScene is called for all scenes.
				if ( Application.isEditor )
				{
					Debug.Log( string.Format( "Detected editor, writing levels file to '{0}'.", EDITOR_LEVELS_FILE_DIRECTORY ) );
					
					WriteLevelsFile( EDITOR_LEVELS_FILE_DIRECTORY );
					_hasUpdatedLevelsFile = true;
				}
			}
			else
			{
				Debug.Log( "Already updated levels file." );
			}
			
			Debug.Log( "Post-processed scene." );
		}
		
		/// <summary>
		/// Writes or creates the levels file by collecting all levels configured in the build and (re-)writing the levels file at the provided directory.
		/// </summary>
		/// <param name="directory">The directory to write the levels file to.</param>
		private static void WriteLevelsFile(string directory)
		{
			List<string> levelNames = new List<string>();
			
			// Collect the names of all levels in the build settings.
			foreach (UnityEditor.EditorBuildSettingsScene buildSettingsScene in UnityEditor.EditorBuildSettings.scenes)
			{
				if (buildSettingsScene.enabled)
				{
					string name = buildSettingsScene.path.Substring(buildSettingsScene.path.LastIndexOf(Path.AltDirectorySeparatorChar) + 1);
					name = name.Substring(0, name.Length - 6);
					
					levelNames.Add( name );
					
					Debug.Log(string.Format("Detected level at '{0}' with name '{1}'.", buildSettingsScene.path, name));
				}
			}
			
			string path = Path.Combine(directory, LEVEL_FILE_NAME);
			
			Debug.Log( string.Format("Writing levels file to '{0}'.", path) );
			
			// Write the names of all levels to a file, so that it can be retrieved when running.
			using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(stream))
				{
					foreach (string levelName in levelNames)
					{
						writer.WriteLine( levelName );
					}
				}
			}
		}
		#endif
		
		/// <summary>
		/// Reads the levels file from the provided directory.
		/// </summary>
		/// <param name="directory">The directory that contains the levels file.</param>
		/// <returns>The discovered levels.</returns>
		private static string[] ReadLevelsFile(string directory)
		{
			string path = Path.Combine(directory, LEVEL_FILE_NAME);
			
			Debug.Log(string.Format("Reading levels file from '{0}'.", path));
			
			List<string> levelNames = new List<string>();
			
			if (File.Exists(path))
			{
				// Read the names of all levels from the levels file.
				using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						// Possibly use ReadToEnd and string.Split(fileContent, Environment.NewLine).
						while (!reader.EndOfStream)
						{
							levelNames.Add(reader.ReadLine());
						}
					}
				}
			}
			else
			{
				Debug.LogWarning("Levels file does not exist, no level names available at run-time.");
			}
			
			return levelNames.ToArray();
		}
		#endregion
	}
}
