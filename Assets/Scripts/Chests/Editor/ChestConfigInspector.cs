using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Chests
{
	[CustomEditor(typeof(ChestConfig), true)]
	public class ChestConfigInspector: Editor
	{
		[SerializeField]
		private VisualTreeAsset _editorAsset;
		
		private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();

		protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
		{
			outSerializedProperties.Clear();
			using (var iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
					}
					while (iterator.NextVisible(false));
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			return;

			GetSerializedProperties(ref _serializedProperties);

			serializedObject.Update();

			foreach (var property in _serializedProperties)
			{
				if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
				{
					using (new EditorGUI.DisabledScope(disabled: true))
					{
						EditorGUILayout.PropertyField(property);
					}
				}
				else
				{
					EditorGUILayout.PropertyField(property);
					EditorGUILayout.HelpBox("!!!", MessageType.Error);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		public override VisualElement CreateInspectorGUI()
		{
			//return base.CreateInspectorGUI();

			var container = new VisualElement();

			var rewardsList = new ListView();

			//property.propertyType == SerializedPropertyType.Float

			//var minField = new PropertyField(property.FindPropertyRelative("min"));
			//var maxField = new PropertyField(property.FindPropertyRelative("max"));
			//container.Add(minField);
			//container.Add(maxField);

			container.Add(rewardsList);

			InspectorElement.FillDefaultInspector(container, serializedObject, this);

			return container;
		}
	}
}
