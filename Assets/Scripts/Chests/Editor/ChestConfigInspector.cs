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

		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();

			var rewardsList = new ListView();

			container.Add(rewardsList);

			InspectorElement.FillDefaultInspector(container, serializedObject, this);

			return container;
		}
	}
}
