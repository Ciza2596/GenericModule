using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAddressablesModule.Editor
{
	public abstract class BListVE<TItemVE> : VisualElement, IListVE where TItemVE : BItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------
		
		[NonSerialized]
		protected readonly List<TItemVE> _itemVEs = new List<TItemVE>();
		
		[NonSerialized]
		protected readonly VisualElement _dropIndicator = new VisualElement() { pickingMode = PickingMode.Ignore };
		
		protected virtual VisualElement DropIndicatorLayer => this;
		protected abstract string[] DropIndicatorClasses { get; }
		
		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public bool IsInitialized { get; private set; }
		
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

		public virtual bool TryGetClosestItemIndex(float cursorY, out int itemIndex)
		{
			itemIndex = ClosestItemIndex(cursorY);
			return itemIndex >= 0;
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BListVE() { }

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize()
		{
			if (IsInitialized)
				return;
			
			foreach (var c in DropIndicatorClasses)
				_dropIndicator.AddToClassList(c);

			IsInitialized = true;
			DerivedInitialize();
			
			DropIndicatorLayer.Add(_dropIndicator);
		}

		public abstract void Refresh();
		protected virtual void DerivedInitialize() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void RefreshItemDragUI(int sourceIndex, int targetIndex)
		{
			var items = _itemVEs;
			if (items.Count <= 0)
			{
				_dropIndicator.style.display = DisplayStyle.None;
				return;
			}

			foreach (var item in items)
				item.DisplayAsNormal();

			if (sourceIndex >= 0 && sourceIndex < items.Count)
				items[sourceIndex].DisplayAsDrag();

			if (targetIndex >= 0 && targetIndex <= items.Count)
			{
				var localY = targetIndex < items.Count ? items[targetIndex].localBound.y : items[^1].localBound.yMax;
				var point = items[0].ChangeCoordinatesTo(DropIndicatorLayer, new Vector2(0, localY));
				
				_dropIndicator.SetAnchoredPosition(AnchorKinds.Top, point);
				_dropIndicator.style.display = DisplayStyle.Flex;
			}
			else
				_dropIndicator.style.display = DisplayStyle.None;
		}

		public abstract void MoveItem(int sourceIndex, int destinationIndex);

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void MoveItem(SerializedProperty arrayProperty, int sourceIndex, int destinationIndex)
		{
			arrayProperty.MoveArrayElement(sourceIndex, GetDestinationIndex(arrayProperty, destinationIndex));
			SerializationUtils.ApplyUnregisteredSerialization(arrayProperty.serializedObject);
		}

		protected virtual int GetDestinationIndex(SerializedProperty arrayProperty, int destinationIndex)
		{
			arrayProperty.serializedObject.Update();
			return Math.Clamp(destinationIndex, 0, arrayProperty.arraySize - 1);
		}
		
	}
}