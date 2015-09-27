using Android.Views;

namespace Codenutz.Controls
{
	public abstract class TokenImageSpan : ViewSpan
	{
		protected TokenImageSpan(View view, int maxWidth) : base(view, maxWidth)
		{
		}
	}

	public class TokenImageSpan<T> : TokenImageSpan where T:class
	{
		private readonly TokenCompleteTextView<T> _tokenCompleteTextView;

		public T Token { get; private set; }

		public TokenImageSpan(View view, T token, int maxWidth, TokenCompleteTextView<T> tokenCompleteTextView)
			: base(view, maxWidth)
		{
			_tokenCompleteTextView = tokenCompleteTextView;
			Token = token;
		}

		public void OnClick()
		{
			var text = _tokenCompleteTextView.EditableText;
			if(text == null) return;

			var tokenClickStyle = _tokenCompleteTextView.TokenClickStyle;
			switch (tokenClickStyle)
			{
				case TokenClickStyle.Select:
				case TokenClickStyle.SelectDeselect:
					if (!View.Selected)
					{
						_tokenCompleteTextView.ClearSelections();
						View.Selected = true;
						break;
					}

					if (tokenClickStyle == TokenClickStyle.SelectDeselect)
					{
						View.Selected = false;
						_tokenCompleteTextView.Invalidate();
					}

					break;
				case TokenClickStyle.Delete:
					_tokenCompleteTextView.RemoveSpan(this);
					break;
				default:
					if (_tokenCompleteTextView.SelectionStart != text.GetSpanEnd(this) + 1)
					{
						_tokenCompleteTextView.SetSelection(text.GetSpanEnd(this) + 1);
					}
					break;
			}
		}


	}
}