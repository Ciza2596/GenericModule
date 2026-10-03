using System;
using UnityEngine.Scripting;

namespace CizaAddressablesModule
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SwitchGroupAttribute : Attribute
	{
		private readonly string _groupName;
		private readonly object _associatedValue;

		public bool IsLeader => _associatedValue is Type;
		public bool IsInverted { get; private set; }
		public string GetGroupName() => _groupName;
		public object GetAssociatedValue() => _associatedValue;

		[Preserve]
		public SwitchGroupAttribute(string groupName, bool isInverted, params object[] associatedValue)
		{
			_groupName = groupName;
			_associatedValue = associatedValue.Length == 1 ? associatedValue[0] : associatedValue;
			IsInverted = isInverted;
		}

		[Preserve]
		public SwitchGroupAttribute(string groupName, params object[] associatedValue) : this(groupName, false, associatedValue) { }
	}
}

