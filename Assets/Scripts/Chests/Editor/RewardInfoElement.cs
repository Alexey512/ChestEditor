using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEditor;

namespace Assets.Scripts.Chests
{
	public class RewardInfoElement: ValidateObjectElement
	{
		public RewardInfoElement(SerializedProperty property) : base(property)
		{
		}

		protected override string GetDisplayName(SerializedProperty property)
		{
			return EditorHelper.GetRewardInfoDisplayName(property);
		}
	}
}
