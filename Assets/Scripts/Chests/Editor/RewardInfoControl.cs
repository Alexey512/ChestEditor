using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEditor;

namespace Assets.Scripts.Chests
{
	public class RewardInfoControl: ValidateObjectControl
	{
		protected override string GetDisplayName(SerializedProperty property)
		{
			return EditorHelper.GetRewardInfoDisplayName(property);
		}
	}
}
