using Android.Text;
using Java.Lang;

namespace Codenutz.Controls
{
	public class TokenSpanWatcher<T> : Java.Lang.Object, ISpanWatcher where T:class
	{
		protected TokenCompleteTextView<T> TokenCompleteTextView { get; set; }

		public TokenSpanWatcher(TokenCompleteTextView<T> tokenCompleteTextView)
		{
			TokenCompleteTextView = tokenCompleteTextView;
		}

		public void OnSpanAdded(ISpannable text, Object what, int start, int end)
		{
			var token = what as TokenImageSpan<T>;
			if (token != null && !TokenCompleteTextView.IsInSavingState && !TokenCompleteTextView.IsFocusChanging)
			{
				TokenCompleteTextView.Items.Add(token.Token);

				if(TokenCompleteTextView.Listener != null)
					TokenCompleteTextView.Listener.OnTokenAdded(token.Token);
			}
		}

		public void OnSpanChanged(ISpannable text, Object what, int ostart, int oend, int nstart, int nend)
		{
			//no action
		}

		public void OnSpanRemoved(ISpannable text, Object what, int start, int end)
		{
			var token = what as TokenImageSpan<T>;
			if (token != null && !TokenCompleteTextView.IsInSavingState && !TokenCompleteTextView.IsFocusChanging)
			{
				if (TokenCompleteTextView.Items.Contains(token.Token))
				{
					TokenCompleteTextView.Items.Remove(token.Token);
				}

				if (TokenCompleteTextView.Listener != null)
					TokenCompleteTextView.Listener.OnTokenRemoved(token.Token);
			}
		}
	}
}