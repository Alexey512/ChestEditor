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
		public override VisualElement CreateInspectorGUI()
		{
			var container = new VisualElement();
			container.Add(new PropertyField(serializedObject.FindProperty("_rewards")));
			return container;
		}
	}
}
