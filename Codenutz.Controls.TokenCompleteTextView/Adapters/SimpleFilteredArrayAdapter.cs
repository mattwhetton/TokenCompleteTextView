using System;
using System.Collections.Generic;
using Android.Content;

namespace Codenutz.Controls
{
	public class SimpleFilteredArrayAdapter<T> : FilteredArrayAdapter<T>
	{
		private readonly Func<T, string, bool> _filter;

		public SimpleFilteredArrayAdapter(Context context, int resource, Func<T, string, bool> filter, T[] objects) : base(context, resource, objects)
		{
			_filter = filter;
		}

		public SimpleFilteredArrayAdapter(Context context, int resource, Func<T, string, bool> filter, int textViewResourceId, T[] objects)
			: base(context, resource, textViewResourceId, objects)
		{
			_filter = filter;
		}

		public SimpleFilteredArrayAdapter(Context context, int resource, Func<T, string, bool> filter, List<T> objects)
			: base(context, resource, objects)
		{
			_filter = filter;
		}

		public SimpleFilteredArrayAdapter(Context context, int resource, Func<T, string, bool> filter, int textViewResourceId, IList<T> objects)
			: base(context, resource, textViewResourceId, objects)
		{
			_filter = filter;
		}

		protected override bool KeepObject(T obj, string mask)
		{
			return _filter.Invoke(obj, mask);
		}
	}
}