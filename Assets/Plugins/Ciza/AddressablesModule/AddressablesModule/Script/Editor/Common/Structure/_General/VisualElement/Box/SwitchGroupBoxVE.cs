using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAddressablesModule.Editor
{
	public class SwitchGroupBoxVE : BBoxVE
	{
		// VARIABLE: -----------------------------------------------------------------------------
		
		protected override string[] USSPaths => new[] { "Box" };

		protected override string[] RootClasses => new[] { "switchgroupbox-root" };
		protected override string[] HeadClasses => new[] { "switchgroupbox-head" };
		protected override string[] BodyClasses => new[] { "switchgroupbox-body" };

		protected virtual string[] ActiveBodyClasses => new[] { "switchgroupbox-active" };

		[field: NonSerialized]
		protected Action<SerializedPropertyChangeEvent> OnChangeValue { get; }

		[field: NonSerialized]
		protected SwitchGroup SwitchGroup { get; }

		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public SwitchGroupBoxVE(SwitchGroup switchGroup, Action<SerializedPropertyChangeEvent> onChangeValue)
		{
			SwitchGroup = switchGroup;
			OnChangeValue = onChangeValue;
		}
		
		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void DerivedInitialize(string title, IContent content, VisualElement headAdditional)
		{
			base.DerivedInitialize(title, content, headAdditional);

			SetupHead();
			SetupBody();
		}

		protected override void DerivedRefresh(bool isForce)
		{
			var membersByValue = SwitchGroup.GetMembersByValue().Select(m => m.propertyPath).ToArray();
			var hasChild = membersByValue.Length > 0;

			foreach (var child in _body.Children())
				child.SetIsVisible(membersByValue.Contains(child.name));

			foreach (var activeBodyClass in ActiveBodyClasses)
				_body.EnableInClassList(activeBodyClass, hasChild);
		}

		protected override void OnHeadClick() { }

		protected virtual void SetupHead()
		{
			var leaderProperty = SwitchGroup.LeaderProperty;
			var fieldVE = new PropertyField(leaderProperty);
			fieldVE.BindProperty(leaderProperty);
			fieldVE.RegisterValueChangeCallback(OnValueChanged);
			fieldVE.style.flexGrow = 1;
			fieldVE.name = leaderProperty.propertyPath;
			if (OnChangeValue != null)
				fieldVE.RegisterValueChangeCallback(OnChangeValue.Invoke);
			AddHeadLeftContent(fieldVE);
		}

		protected virtual void SetupBody()
		{
			foreach (var member in SwitchGroup.AllMembers)
			{
				var fieldVE = new PropertyField(member);
				fieldVE.BindProperty(member);
				fieldVE.name = member.propertyPath;
				if (OnChangeValue != null)
					fieldVE.RegisterValueChangeCallback(OnChangeValue.Invoke);
				_body.Add(fieldVE);
			}
		}

		protected virtual void OnValueChanged(SerializedPropertyChangeEvent evt)
		{
			Refresh();
		}
	}
}
