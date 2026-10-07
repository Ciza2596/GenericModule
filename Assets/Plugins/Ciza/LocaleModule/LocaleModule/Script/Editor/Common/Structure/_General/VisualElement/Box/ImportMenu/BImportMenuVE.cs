using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
	public abstract class BImportMenuVE : VisualElement
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly FileFilterListVE _filterListVE;
		
		[NonSerialized]
		protected readonly PathListVE _pathListVE;
		
		[NonSerialized]
		protected readonly VisualElement _additionalFieldContainer = new VisualElement();
		[NonSerialized]
		protected readonly VisualElement _buttonContainer = new VisualElement();
		[NonSerialized]
		protected readonly Button _importButton = new Button() { text = "Import" };
		[NonSerialized]
		protected readonly Button _cancelButton = new Button() { text = "Cancel" };
		
		[field:NonSerialized]
		protected virtual BoxVE Root { get; }

		protected virtual string[] USSPaths => new[] { "Box" };
		protected virtual string[] RootClasses => new[] { "box-import-menu" };
		protected virtual string[] AdditionalFieldContainerClasses => new[] { "box-import-menu-additional-fields" };
		protected virtual string[] ButtonContainerClasses => new[] { "box-import-menu-button-container" };
		
		
		protected virtual SerializedProperty RootProperty { get; }
		protected virtual string RootKey { get; }
		protected virtual string FileFilterListKey => RootKey + ".FileFilterList";
		protected virtual string PathListKey => RootKey + ".PathList";
		
		protected virtual string[] GetFilteredAssetPaths(bool isSort)
		{
			var fileFilters = _filterListVE.Items;
			var paths = _pathListVE.Items.Where(AssetDatabase.IsValidFolder).ToArray();

			var results = new List<string>();
			if (fileFilters is not { Length: > 0 })
				results.AddRange(AssetDatabase.FindAssets(string.Empty, paths).Select(AssetDatabase.GUIDToAssetPath).Where(Path.HasExtension));
			else
			{
				foreach (var filter in fileFilters)
				{
					var filterString = $"{filter.NamePrefix} {filter.NameSuffix} ";
					var assetPaths = AssetDatabase.FindAssets(filterString.Trim(), paths).Select(AssetDatabase.GUIDToAssetPath);

					var filteredPaths = assetPaths.Where(path => CheckIsFilePathMatched(path, filter));
					results.AddRange(filteredPaths);
				}
			}

			var distinctResult = results.Distinct();
			
			if (!isSort) 
				return distinctResult.ToArray();
			
			var comparer = Comparer<string>.Create(EditorUtility.NaturalCompare);
			distinctResult = distinctResult.OrderBy(Path.GetFileNameWithoutExtension, comparer).ThenBy(path => path, comparer);

			return distinctResult.ToArray();

			bool CheckIsFilePathMatched(string path, FileFilter filter)
			{
				if (!path.CheckHasValue() || !path.Contains('/') || !path.Contains('.'))
					return false;

				var fileName = Path.GetFileNameWithoutExtension(path);
				var extension = Path.GetExtension(path);
				var isPrefixMatched = !filter.NamePrefix.CheckHasValue() || fileName.StartsWith(filter.NamePrefix);
				var isSuffixMatched = !filter.NameSuffix.CheckHasValue() || fileName.EndsWith(filter.NameSuffix);
				var isExtensionMatched = filter.Extension == "." || extension.Equals(filter.Extension);
				return isPrefixMatched && isSuffixMatched && isExtensionMatched;
			}
		}

		// EVENT: ---------------------------------------------------------------------------------
		public event Action OnRequestClose;

		// CONSTRUCTOR: ---------------------------------------------------------------------------

		[Preserve]
		public BImportMenuVE(SerializedProperty property, BoxVE root)
		{
			Root = root;
			RootProperty = property;
			RootKey = property.propertyPath;
			_filterListVE = new FileFilterListVE(FileFilterListKey);
			_filterListVE.Initialize();
			_pathListVE = new PathListVE(PathListKey);
			_pathListVE.Initialize();
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void Initialize()
		{
			foreach (var styleSheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(styleSheet);

			foreach (var rootClass in RootClasses)
				AddToClassList(rootClass);
			
			var fileFilterListBoxVE = new BoxVE(RootKey + ".FileFilterListBox");
			fileFilterListBoxVE.Initialize("File Filter List", _filterListVE);
			Add(fileFilterListBoxVE);
			
			var pathListBoxVE = new BoxVE(RootKey + ".PathListBox");
			pathListBoxVE.Initialize("Path List", _pathListVE);
			Add(pathListBoxVE);

			foreach (var additionalFieldContainerClass in AdditionalFieldContainerClasses)
				_additionalFieldContainer.AddToClassList(additionalFieldContainerClass);

			foreach (var buttonContainerClass in ButtonContainerClasses)
				_buttonContainer.AddToClassList(buttonContainerClass);

			_importButton.clicked += OnImportButtonClicked;
			_cancelButton.clicked += OnCancelButtonClicked;

			_buttonContainer.Add(_importButton);
			_buttonContainer.Add(_cancelButton);

			Add(_additionalFieldContainer);
			Add(_buttonContainer);
		}

		// PROTECT METHOD: ------------------------------------------------------------------------

		protected void AddAdditionalField(VisualElement field)
		{
			if (field != null)
				_additionalFieldContainer.Add(field);
		}

		protected void Close() =>
			OnRequestClose?.Invoke();

		protected abstract bool TryImport();
		

		protected virtual void OnImportButtonClicked()
		{
			if (TryImport())
				Close();
		}

		protected virtual void OnCancelButtonClicked() =>
			Close();
	}
}
