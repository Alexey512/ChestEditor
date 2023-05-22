using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ConditionAttribute: PropertyAttribute
	{
		public readonly string conditionField;
		
		public readonly object compareValue;

		public ConditionAttribute(string conditionField, object compareValue = null)
		{
			this.conditionField = conditionField;
			this.compareValue = compareValue;
		}
	}
}
