using System.Linq;
using Android.Text;

namespace Codenutz.Controls.Extensions
{
	public static class IEditableExtensions
	{

		public static T[] GetSpans<T>(this IEditable editable, int start, int end)
		{
			var spans = editable.GetSpans(start, end, Java.Lang.Class.FromType(typeof (T)));
			var results = spans.Cast<T>().ToArray();
			return results;
		}
	}
}