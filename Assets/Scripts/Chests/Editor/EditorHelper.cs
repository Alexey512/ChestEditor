using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Chests
{
	public static class EditorHelper
	{
		public static string GetRewardInfoDisplayName(SerializedProperty property)
		{
			var typeProperty = property.FindPropertyRelative("type");
			ChestConfig.RewardType rewardType = (ChestConfig.RewardType)typeProperty.enumValueIndex;
			
			float weight = property.FindPropertyRelative("randomWeight").floatValue;

			switch (rewardType)
			{
				case ChestConfig.RewardType.Chest:
					var chestProperty = property.FindPropertyRelative("chest");
					var nestedChest = chestProperty.objectReferenceValue as ChestConfig;
					return nestedChest != null ?
						$"{rewardType}: Weight={weight}; Name={nestedChest.name}; Count={property.FindPropertyRelative("chestCount").intValue}" :
						$"{rewardType}: Weight={weight}; Count={property.FindPropertyRelative("chestCount").intValue}";
				case ChestConfig.RewardType.Hard:
					return $"{rewardType}: Weight={weight}; Range=[{property.FindPropertyRelative("hardRange.min").floatValue}-{property.FindPropertyRelative("hardRange.max").floatValue}]";
				case ChestConfig.RewardType.Soft:
					return $"{rewardType}: Weight={weight}; Range=[{property.FindPropertyRelative("softRange.min").intValue}-{property.FindPropertyRelative("softRange.max").intValue}]";
			}

			return $"{rewardType}:";
		}
	}
}
