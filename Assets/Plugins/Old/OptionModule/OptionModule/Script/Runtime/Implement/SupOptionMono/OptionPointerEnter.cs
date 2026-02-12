using UnityEngine.EventSystems;

namespace CizaOptionModule.Implement
{
	public class OptionPointerEnter : OptionSup, IPointerEnterHandler
	{
		public void OnPointerEnter(PointerEventData eventData) =>
			Option.PointerEnter();
	}
}
