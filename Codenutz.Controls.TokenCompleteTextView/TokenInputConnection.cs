using Android.Views.InputMethods;

namespace Codenutz.Controls
{
	public class TokenInputConnection<T> : InputConnectionWrapper where T:class
	{
		public TokenCompleteTextView<T> TokenCompleteTextView { get; set; }

		public TokenInputConnection(IInputConnection target, bool mutable, TokenCompleteTextView<T> tokenCompleteTextView) : base(target, mutable)
		{
			TokenCompleteTextView = tokenCompleteTextView;
		}

		public override bool DeleteSurroundingText(int beforeLength, int afterLength)
		{
			if (TokenCompleteTextView.SelectionStart <= TokenCompleteTextView.Prefix.Length)
				beforeLength = 0;

			return TokenCompleteTextView.DeleteSelectedObject(false) || base.DeleteSurroundingText(beforeLength, afterLength);
		}
	}
}