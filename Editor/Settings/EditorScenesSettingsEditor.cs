using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
	[CustomEditor(typeof(EditorScenesSettings))]
	public class EditorScenesSettingsEditor : UnityEditor.Editor
	{
		private const string NONE_VALUE = "(None)";
		private const int TITLE_MARGIN_TOP = 2;
		private const int TITLE_MARGIN_LEFT = 9;
		private const int TITLE_FONT_SIZE = 19;
		
		private const string SETTINGS_MENU_PATH = "Acciaio/Scenes";

		[SettingsProvider]
		public static SettingsProvider Provide()
		{
			return new(SETTINGS_MENU_PATH, SettingsScope.Project)
			{
				activateHandler = (searchContext, rootElement) =>
            	{
					var editor = UnityEditor.Editor.CreateEditor(EditorScenesSettings.GetOrCreateSettings());
					rootElement.Add(editor.CreateInspectorGUI());
				}
			};
		}

		private SerializedProperty _startupScene;
		private SerializedProperty _isActive;

		private void OnEnable()
		{
			_startupScene = serializedObject.FindProperty("_startupScene");
			_isActive = serializedObject.FindProperty("_isActive");
		}
		
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement rootElement = new();

			var isActive = serializedObject.FindProperty("_isActive");
			var startupScene = serializedObject.FindProperty("_startupScene");

			var container = new VisualElement();
			container.style.flexDirection = FlexDirection.Row;

			var title = new Label("Acciaio Scenes");
			title.style.marginLeft = TITLE_MARGIN_LEFT;
			title.style.marginTop = TITLE_MARGIN_TOP;
			title.style.fontSize = TITLE_FONT_SIZE;
			title.style.unityFontStyleAndWeight = FontStyle.Bold;

			var scenes = AcciaioEditor.BuildScenesListForField(true, NONE_VALUE).ToList();
			var defaultValue = startupScene.stringValue;
			if (string.IsNullOrEmpty(defaultValue)) defaultValue = NONE_VALUE;

			PopupField<string> popup = new("Editor Startup Scene", scenes, defaultValue);
			var grow = popup.style.flexGrow;
			grow.value = 1;
			var shrink = popup.style.flexShrink;
			shrink.value = 1;
			popup.style.flexGrow = grow;
			popup.style.flexShrink = shrink;
			popup.style.overflow = Overflow.Hidden;
			popup.SetEnabled(isActive.boolValue);

			popup.RegisterCallback<ChangeEvent<string>>(
				evt =>
				{
					var value = evt.newValue;
					if (value == NONE_VALUE) value = "";
					startupScene.stringValue = value;
					serializedObject.ApplyModifiedProperties();
				}
			);

			Toggle toggle = new()
			{
				value = isActive.boolValue
			};

			toggle.RegisterCallback<ChangeEvent<bool>>(
				evt => 
				{
					isActive.boolValue = evt.newValue; 
					popup.SetEnabled(isActive.boolValue);
					serializedObject.ApplyModifiedProperties();
				}
			);

			container.Add(toggle);
			container.Add(popup);
			rootElement.Add(title);
			rootElement.Add(new Label());
			rootElement.Add(container);

			return rootElement;
		}
	}
}