using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
	public abstract class BListVE<TItem> : VisualElement, IListVE where TItem : BItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------
		[NonSerialized]
		protected readonly List<TItem> _items = new List<TItem>();

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public bool IsInitialized { get; private set; }


		[Preserve]
		protected BListVE() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual int GetItemIndexOf(VisualElement item)
		{
			for (int i = 0; i < _items.Count; i++)
				if (_items[i] == item)
					return i;

			return -1;
		}

		public virtual int ClosestItemIndex(float cursorY)
		{
			var minDistance = Mathf.Infinity;
			var minIndex = -1;

			for (int i = 0; i < _items.Count; ++i)
			{
				if (_items[i] == null)
					continue;

				var center = _items[i].worldBound.y + _items[i].worldBound.height * 0.5f;
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
			var items = _items;
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


		protected virtual void MoveItems(SerializedObject rootSerializedObject, SerializedProperty arrayProperty, int sourceIndex, int destinationIndex)
		{
			rootSerializedObject.Update();

			destinationIndex = Math.Max(destinationIndex, 0);
			destinationIndex = Math.Min(destinationIndex, arrayProperty.arraySize);

			if (sourceIndex < destinationIndex)
				destinationIndex -= 1;

			arrayProperty.MoveArrayElement(sourceIndex, destinationIndex);
			SerializationUtils.ApplyUnregisteredSerialization(rootSerializedObject);
		}
	}
}