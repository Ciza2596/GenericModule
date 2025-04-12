using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BListVE<TItemVE> : VisualElement, IListVE where TItemVE : BItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------
		[NonSerialized]
		protected readonly List<TItemVE> _itemVEs = new List<TItemVE>();

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public bool IsInitialized { get; private set; }

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BListVE() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual int GetItemIndexOf(VisualElement item)
		{
			for (int i = 0; i < _itemVEs.Count; i++)
				if (_itemVEs[i] == item)
					return i;

			return -1;
		}

		public virtual int ClosestItemIndex(float cursorY)
		{
			var minDistance = Mathf.Infinity;
			var minIndex = -1;

			for (int i = 0; i < _itemVEs.Count; ++i)
			{
				if (_itemVEs[i] == null)
					continue;

				var center = _itemVEs[i].worldBound.y + _itemVEs[i].worldBound.height * 0.5f;
				var distance = center - cursorY;
				var distanceAbsolute = Math.Abs(distance);

				if (minIndex != -1 && distanceAbsolute > minDistance)
					continue;

				minIndex = distance >= 0 ? i : i + 1;
				minDistance = distanceAbsolute;
			}

			return minIndex;
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void RefreshItemDragUI(int sourceIndex, int targetIndex)
		{
			var items = _itemVEs;
			if (items.Count <= 0)
				return;

			foreach (var item in items)
				item.DisplayAsNormal();

			if (sourceIndex != -1)
				items[sourceIndex].DisplayAsDrag();

			if (targetIndex != -1)
			{
				if (targetIndex < items.Count)
					items[targetIndex].DisplayAsTargetAbove();

				else
					items[^1].DisplayAsTargetBelow();
			}
		}

		public abstract void MoveItems(int sourceIndex, int destinationIndex);

		public void Initialize()
		{
			if (IsInitialized)
				return;

			IsInitialized = true;
			DerivedInitialize();
		}

		public virtual void Refresh() { }

		protected virtual void DerivedInitialize() { }

		protected virtual void MoveItems(SerializedProperty arrayProperty, int sourceIndex, int destinationIndex)
		{
			arrayProperty.MoveArrayElement(sourceIndex, GetDestinationIndex(arrayProperty, destinationIndex));
			SerializationUtils.ApplyUnregisteredSerialization(arrayProperty.serializedObject);
		}

		protected virtual int GetDestinationIndex(SerializedProperty arrayProperty, int destinationIndex)
		{
			arrayProperty.serializedObject.Update();
			destinationIndex = Math.Max(destinationIndex, 0);
			destinationIndex = Math.Min(destinationIndex, arrayProperty.arraySize - 1);
			return destinationIndex;
		}
	}
}