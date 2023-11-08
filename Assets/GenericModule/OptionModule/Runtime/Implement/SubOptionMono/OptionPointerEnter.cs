using UnityEngine.EventSystems;

namespace CizaOptionModule.Implement
{
	public class OptionPointerEnter : OptionSubMono, IPointerEnterHandler
	{
		public void OnPointerEnter(PointerEventData eventData) =>
			_option.PointerEnter();
	}
}
