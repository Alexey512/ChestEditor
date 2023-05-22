using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Attributes;
using Assets.Scripts.Common.Editor;
using Assets.Scripts.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Chests
{
	[CustomPropertyDrawer(typeof(ChestConfig.RewardInfo))]
	public class ValidateObjectDrawer: PropertyDrawer
	{
#if false		
		private ValidateObjectControl _objectControl = new ValidateObjectControl();
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_objectControl.OnGUI(position, property, label);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return _objectControl.GetPropertyHeight(property, label);
		}
#else
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new ValidateObjectElement(property);
		}
#endif

	}
}
