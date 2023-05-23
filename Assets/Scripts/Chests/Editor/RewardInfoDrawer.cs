using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Chests
{
	[CustomPropertyDrawer(typeof(ChestConfig.RewardInfo))]
	public class RewardInfoDrawer: ValidateObjectDrawer
	{
#if USE_IMGUI_EDITOR		

		private RewardInfoControl _control = new RewardInfoControl();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_control.OnGUI(position, property, label);
		}
#else
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new RewardInfoElement(property);
		}
#endif		
	}
}
