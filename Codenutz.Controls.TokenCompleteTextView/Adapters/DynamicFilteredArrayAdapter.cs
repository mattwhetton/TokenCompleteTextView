using System;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;
using Java.Lang;

namespace Codenutz.Controls
{
	public class DynamicFilteredArrayAdapter<T> : ArrayAdapter<T>
	{
		private readonly Func<string, List<T>> _query;

		#region Private members
        
		private Filter _filter;
	    private IList<T> _items; 

		#endregion

		#region Properties

		public override Filter Filter => _filter ?? (_filter = new DynamicFilter(this, _query));

	    public IList<T> Items { get; set; }

	    #endregion

		#region Constructors

		public DynamicFilteredArrayAdapter(Context context, int resource, Func<string, List<T>> query)
			: this(context, resource, 0, query)
		{
		}

		public DynamicFilteredArrayAdapter(Context context, int resource, int textViewResourceId, Func<string, List<T>> query)
			: base(context, resource, textViewResourceId, new List<T>())
		{
			_query = query;
		}

		#endregion
		
		#region Nested Types

		private class DynamicFilter : FilterBase<T>
		{
		    private readonly DynamicFilteredArrayAdapter<T> _filteredArrayAdapter;
		    private readonly Func<string, List<T>> _query;

			public DynamicFilter(DynamicFilteredArrayAdapter<T> filteredArrayAdapter, Func<string, List<T>> query)
				: base(filteredArrayAdapter)
			{
			    _filteredArrayAdapter = filteredArrayAdapter;
			    _query = query;
			}

		    protected override FilterResults PerformFiltering(ICharSequence chars)
			{
		        var result = new FilterResults();
				var mask = chars?.ToString();

				var queryResults = _query(mask);
				result.Values = new JavaObjectWrapper<List<T>>(queryResults);
				result.Count = queryResults.Count;

				return result;
			}

		    protected override void PublishResults(ICharSequence constraint, FilterResults results)
		    {
		        // ReSharper disable once UseNullPropagation
		        if (results != null)
		        {
		            var values = results.Values as JavaObjectWrapper<List<T>>;
		            if (values != null)
		                _filteredArrayAdapter.Items = values.Obj;
		        }
		        base.PublishResults(constraint, results);
		    }
		}

		#endregion
	}
}
