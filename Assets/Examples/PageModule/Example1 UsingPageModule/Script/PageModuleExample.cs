using CizaPageModule.Implement;
using UnityEngine;

namespace CizaPageModule.Example1
{
	public class PageModuleExample : MonoBehaviour
	{
		//private variable
		[SerializeField]
		private PageModuleConfig _pageModuleConfig;

		//public variable
		public PageModule PageModule { get; private set; }

		//unity callback
		private async void Awake()
		{
			PageModule = new PageModule(_pageModuleConfig);
			PageModule.Create<TitlePage>(nameof(TitlePage));

			await PageModule.Show(nameof(TitlePage));
		}
	}
}
