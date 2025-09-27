using System;
using System.Collections.Generic;
using CizaPageModule.Implement;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaTransitionModule.Implement
{
	[CreateAssetMenu(fileName = "TransitionModuleConfig", menuName = "Ciza/TransitionModule/TransitionModuleConfig")]
	public class TransitionModuleConfig : ScriptableObject, ITransitionModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _rootName;

		[SerializeField]
		protected bool _isDontDestroyOnLoad;

		[Space]
		[SerializeField]
		protected Page[] _pagePrefabs;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string PageRootName => _rootName;
		public virtual bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public virtual MonoBehaviour[] GetPagePrefabs()
		{
			var pagePrefabs = new List<MonoBehaviour>();

			foreach (var pagePrefab in _pagePrefabs)
			{
				Assert.IsNotNull(pagePrefab, "[TransitionModuleConfig::GetPagePrefabs] Please check pagePrefabs. Lose a pagePrefab.");
				pagePrefabs.Add(pagePrefab);
			}

			return pagePrefabs.ToArray();
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_rootName = "[TransitionModule]";
			_isDontDestroyOnLoad = true;

			_pagePrefabs = Array.Empty<Page>();
		}
	}
}