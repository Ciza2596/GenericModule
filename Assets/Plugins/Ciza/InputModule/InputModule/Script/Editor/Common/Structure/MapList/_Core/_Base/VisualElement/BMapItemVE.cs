using UnityEditor;
using UnityEngine.Scripting;

namespace CizaInputModule.Editor.MapListVisual
{
	public abstract class BMapItemVE : ItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected virtual string KeyPath => "_key";
		protected virtual string IsEnablePath => "_isEnable";
		protected virtual string ValuePath => "_value";

		protected virtual SerializedProperty KeyProperty => ItemProperty.FindPropertyRelative(KeyPath);
		protected virtual SerializedProperty IsEnableProperty => ItemProperty.FindPropertyRelative(IsEnablePath);
		protected virtual SerializedProperty ValueProperty => ItemProperty.FindPropertyRelative(ValuePath);

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public override string Title => Key;

		public virtual string Key
		{
			get => KeyProperty.GetValue<string>();
			protected set => KeyProperty.SetValue(value);
		}

		public override bool IsEnable
		{
			get => IsEnableProperty.GetValue<bool>();
			protected set => IsEnableProperty.SetValue(value);
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BMapItemVE(BMapListVE root, SerializedProperty itemProperty) : base(root, itemProperty) { }
	}
}