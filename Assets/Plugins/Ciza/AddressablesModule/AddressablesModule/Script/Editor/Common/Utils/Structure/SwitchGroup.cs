using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Scripting;

namespace CizaAddressablesModule
{
	public class SwitchGroup
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly List<SwitchGroupMember> _members = new List<SwitchGroupMember>();

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public virtual string GroupName { get; }

		[field: NonSerialized]
		public virtual int SiblingIndex { get; protected set; }

		[field: NonSerialized]
		public virtual SerializedProperty LeaderProperty { get; protected set; }

		public virtual SerializedProperty[] AllMembers => _members.Select(x => x.Member).ToArray();

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public SwitchGroup(string groupName)
		{
			GroupName = groupName;
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void SetLeaderProperty(SerializedProperty leaderProperty) =>
			LeaderProperty = leaderProperty;

		public virtual void AddMember(SerializedProperty property, object value, bool isInverted) =>
			_members.Add(new SwitchGroupMember(property, value, isInverted));

		public virtual void SetSiblingIndex(int index) =>
			SiblingIndex = index;

		public virtual SerializedProperty[] GetMembersByValue()
		{
			if (LeaderProperty.propertyType == SerializedPropertyType.Enum)
				return _members.Where(x => CheckMemberByAssociatedValue_Enum(x, LeaderProperty.enumValueFlag)).Select(x => x.Member).ToArray();

			return _members.Where(x => CheckMemberByAssociatedValue_Object(x, LeaderProperty.boxedValue)).Select(x => x.Member).ToArray();
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual bool CheckMemberByAssociatedValue_Enum(SwitchGroupMember member, int leaderValue)
		{
			var valueType = member.AssociatedValue.GetType();
			if (!valueType.IsEnum && !valueType.IsArray)
				return false;

			var isFlags = valueType.IsDefined(typeof(FlagsAttribute), false);
			if (member.AssociatedValue is object[] values)
			{
				if (isFlags)
					return member.IsInverted ? values.All(value => (leaderValue & (int)value) == 0) : values.All(value => (leaderValue & (int)value) != 0);

				return member.IsInverted ? values.All(value => (int)value != leaderValue) : values.Any(value => (int)value == leaderValue);
			}

			var intValue = (int)(member.AssociatedValue);

			if (isFlags)
				return member.IsInverted ? (leaderValue & intValue) == 0 : (leaderValue & intValue) != 0;

			return member.IsInverted ? intValue != leaderValue : intValue == leaderValue;
		}

		protected virtual bool CheckMemberByAssociatedValue_Object(SwitchGroupMember member, object leaderValue)
		{
			if (member.AssociatedValue is object[] values)
				return member.IsInverted ? values.All(value => value.Equals(leaderValue)) : values.Any(value => value.Equals(leaderValue));

			return member.IsInverted ? !member.AssociatedValue.Equals(leaderValue) : member.AssociatedValue.Equals(leaderValue);
		}

		public class SwitchGroupMember
		{
			public SerializedProperty Member { get; }
			public object AssociatedValue { get; }
			public bool IsInverted { get; }

			[Preserve]
			public SwitchGroupMember(SerializedProperty property, object value, bool isInverted)
			{
				Member = property;
				AssociatedValue = value;
				IsInverted = isInverted;
			}
		}
	}
}

