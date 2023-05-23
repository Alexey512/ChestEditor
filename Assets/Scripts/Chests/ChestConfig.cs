using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Attributes;
using UnityEngine;
using RangeInt = Assets.Scripts.Common.RangeInt;

namespace Assets.Scripts.Chests
{
	[CreateAssetMenu(menuName = "Inventory/Chest")]
	public class ChestConfig: ScriptableObject 
	{
		public enum RewardType 
		{
			Soft,
			Hard,
			Chest,
		}

		[Serializable]
		public class RewardInfo
		{
			[Tooltip("Reward drop chance")]
			[Range(0, 100)]
			[ValidateProperty("Random weight must be greater than zero", "randomWeight", ">0")]
			public float randomWeight = 1;
			
			[Tooltip("Reward type")]
			public RewardType type;

			[Tooltip("Chest config asset")]
			[VisibleIf("type", RewardType.Chest)]
			[RequiredProperty]
			public ChestConfig chest;
			
			[Tooltip("Chests count")]
			[VisibleIf("type", RewardType.Chest)]
			[ValidateProperty("Chests count must be greater than zero", "chestCount", ">0")]
			public int chestCount;

			[Tooltip("Range random count of rewards")]
			[VisibleIf("type", RewardType.Hard)]
			[ValidateProperty("Range min must be greater than zero", "hardRange.min", ">0")]
			public RangeFloat hardRange;

			[Tooltip("Range random count of rewards")]
			[VisibleIf("type", RewardType.Soft)]
			[ValidateProperty("Range min must be greater than zero", "softRange.min", ">0")]
			public RangeInt softRange;
		}

		[SerializeField]
		[Tooltip("List of possible random rewards")]
		private RewardInfo[] _rewards;

		public RewardInfo[] Rewards => _rewards;
	}
}
