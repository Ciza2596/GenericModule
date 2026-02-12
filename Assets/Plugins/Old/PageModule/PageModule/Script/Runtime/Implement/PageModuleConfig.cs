using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaPageModule.Implement
{
	[CreateAssetMenu(fileName = "PageModuleConfig", menuName = "Ciza/PageModule/PageModuleConfig")]
	public class PageModuleConfig : ScriptableObject, IPageModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _pageRootName;

		[SerializeField]
		protected bool _isDontDestroyOnLoad;

		[Space]
		[SerializeField]
		protected Page[] _pagePrefabs;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string PageRootName => _pageRootName;

		public virtual bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public virtual MonoBehaviour[] GetPagePrefabs()
		{
			var pagePrefabs = new List<MonoBehaviour>();

			foreach (var pagePrefab in _pagePrefabs)
			{
				Assert.IsNotNull(pagePrefab, "[PageModuleConfig::GetPagePrefabs] Please check pagePrefabs. Lose a pagePrefab.");
				pagePrefabs.Add(pagePrefab);
			}

			return pagePrefabs.ToArray();
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_pageRootName = "[Page]";
			_isDontDestroyOnLoad = false;

			_pagePrefabs = Array.Empty<Page>();
		}
	}
}