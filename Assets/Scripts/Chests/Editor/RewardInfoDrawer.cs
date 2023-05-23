using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEditor;

namespace Assets.Scripts.Chests
{
	[CustomPropertyDrawer(typeof(ChestConfig.RewardInfo))]
	public class RewardInfoDrawer: ValidateObjectDrawer
	{
	}
}
