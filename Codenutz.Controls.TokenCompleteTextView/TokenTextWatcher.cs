using System.Collections.Generic;
using Android.Text;
using Java.Lang;
using Codenutz.Controls.Extensions;

namespace Codenutz.Controls
{
	public class TokenTextWatcher<T> : Java.Lang.Object, ITextWatcher where T:class
	{
		protected readonly TokenCompleteTextView<T> TokenCompleteTextView;

		private List<TokenImageSpan> _spansToRemove = new List<TokenImageSpan>();

		public TokenTextWatcher(TokenCompleteTextView<T> tokenCompleteTextView)
		{
			TokenCompleteTextView = tokenCompleteTextView;
		}


		public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
		{
			var text = TokenCompleteTextView.EditableText;
			if (count > 0 && text != null)
			{
				int end = start + count;

				if (text.CharAt(start) == ' ')
				{
					start -= 1;
				}

				var tokens = text.GetSpans<TokenImageSpan>(start, end);

				_spansToRemove = new List<TokenImageSpan>();
				foreach (var token in tokens)
				{
					if (text.GetSpanStart(token) < end && start < text.GetSpanEnd(token))
					{
						_spansToRemove.Add(token);
					}
				}

			}
		}

		public void AfterTextChanged(IEditable text)
		{
			var spansToCopy = new List<TokenImageSpan>(_spansToRemove);
			foreach (var token in spansToCopy)
			{
				var spanStart = text.GetSpanStart(token);
				var spanEnd = text.GetSpanEnd(token);

				RemoveToken(token, text);

				spanEnd--;

				if (spanEnd >= 0 && TokenCompleteTextView.IsSplitChar(text.CharAt(spanEnd)))
				{
					text.Delete(spanEnd, spanEnd + 1);
				}
				if (spanStart >= 0 && TokenCompleteTextView.IsSplitChar(text.CharAt(spanStart)))
				{
					text.Delete(spanStart, spanStart + 1);
				}
			}

			TokenCompleteTextView.ClearSelections();
			TokenCompleteTextView.UpdateHint();
		}


		public void OnTextChanged(ICharSequence s, int start, int before, int count)
		{
			//no action
		}

		public void RemoveToken(TokenImageSpan token, IEditable text)
		{
			text.RemoveSpan(token);
		}
	}
}