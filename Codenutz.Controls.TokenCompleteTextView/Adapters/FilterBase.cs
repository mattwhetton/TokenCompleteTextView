using System.Collections.Generic;
using Android.Widget;
using Java.Lang;

namespace Codenutz.Controls
{
	public abstract class FilterBase<T> : Filter
	{
		protected readonly ArrayAdapter<T> FilteredArrayAdapter;

		public FilterBase(ArrayAdapter<T> filteredArrayAdapter)
		{
			FilteredArrayAdapter = filteredArrayAdapter;
		}

		protected override void PublishResults(ICharSequence constraint, FilterResults results)
		{
			FilteredArrayAdapter.Clear();
			if (results.Count > 0)
			{
				var values = (JavaObjectWrapper<List<T>>)results.Values;
				FilteredArrayAdapter.AddAll(values.Obj);
				FilteredArrayAdapter.NotifyDataSetChanged();
			}
			else
			{
				FilteredArrayAdapter.NotifyDataSetInvalidated();
			}
		}
	}
}