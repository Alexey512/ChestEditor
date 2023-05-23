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
			switch (rewardType)
			{
				case ChestConfig.RewardType.Chest:
					var chestProperty = property.FindPropertyRelative("chest");
					var nestedChest = chestProperty.objectReferenceValue as ChestConfig;
					return nestedChest != null ?
						$"{rewardType}: Name = {nestedChest.name}, Count = {property.FindPropertyRelative("chestCount").intValue}" :
						$"{rewardType}: Count = {property.FindPropertyRelative("chestCount").intValue}";
				case ChestConfig.RewardType.Hard:
					return $"{rewardType}: Range = [{property.FindPropertyRelative("hardRange.min").floatValue}/{property.FindPropertyRelative("hardRange.max").floatValue}]";
				case ChestConfig.RewardType.Soft:
					return $"{rewardType}: Range = [{property.FindPropertyRelative("softRange.min").intValue}/{property.FindPropertyRelative("softRange.max").intValue}]";
			}

			return $"{rewardType}:";
		}
	}
}
