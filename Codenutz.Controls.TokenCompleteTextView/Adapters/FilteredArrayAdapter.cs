using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Widget;
using Java.Lang;

namespace Codenutz.Controls
{
	public abstract class FilteredArrayAdapter<T> : ArrayAdapter<T>
	{
		#region Private members

		private IList<T> _originalObjects;
		private Filter _filter;

		#endregion

		#region Properties

		public override Filter Filter
		{
			get
			{
				return _filter ?? (_filter = new SimpleFilter(_originalObjects, this));
			}
		}

		#endregion

		#region Constructors

		public FilteredArrayAdapter(Context context, int resource, T[] objects)
			: this(context, resource, 0, objects)
		{
			
		}

		public FilteredArrayAdapter(Context context, int resource, int textViewResourceId, T[] objects)
			: this(context, resource, textViewResourceId, objects.ToList())
		{
			
		}

		public FilteredArrayAdapter(Context context, int resource, List<T> objects)
			: this(context, resource, 0, objects)
		{
			
		}

		public FilteredArrayAdapter(Context context, int resource, int textViewResourceId, IList<T> objects)
			: base(context, resource, textViewResourceId, objects.ToList())
		{
			_originalObjects = objects;
			
		}

		#endregion

		#region Public methods

		protected abstract bool KeepObject(T obj, string mask);

		public override void NotifyDataSetChanged()
		{
			((SimpleFilter)Filter).SetSourceObjects(_originalObjects);
			base.NotifyDataSetChanged();
		}

		public override void NotifyDataSetInvalidated()
		{
			((SimpleFilter)Filter).SetSourceObjects(_originalObjects);
			base.NotifyDataSetInvalidated();
		}

		#endregion

		#region Nested Types
		
		private class SimpleFilter : FilterBase<T>
		{
			private readonly object _syncRoot = new object();

			private List<T> _sourceObjects;


			public SimpleFilter(IList<T> objects, FilteredArrayAdapter<T> filteredArrayAdapter)
				: base(filteredArrayAdapter)
			{
				SetSourceObjects(objects);
			}

			public void SetSourceObjects(IList<T> objects)
			{
				lock (_syncRoot)
				{
					_sourceObjects = new List<T>(objects);
				}
			}

			protected override FilterResults PerformFiltering(ICharSequence chars)
			{
				var result = new FilterResults();
				if (chars != null && chars.Length() > 0)
				{
					var mask = chars.ToString();
					var keptObjects = new List<T>();

					foreach (var sourceObject in _sourceObjects)
					{
						if (((FilteredArrayAdapter<T>)FilteredArrayAdapter).KeepObject(sourceObject, mask))
							keptObjects.Add(sourceObject);
					}

					result.Count = keptObjects.Count;
					result.Values = new JavaObjectWrapper<List<T>>(keptObjects);
				}
				else
				{
					// add all objects
					result.Values = new JavaObjectWrapper<List<T>>(_sourceObjects);
					result.Count = _sourceObjects.Count;
				}
				return result;
			}
		}

		#endregion
	}
}