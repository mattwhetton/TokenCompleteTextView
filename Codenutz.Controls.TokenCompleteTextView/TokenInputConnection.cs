using Android.Views.InputMethods;

namespace Codenutz.Controls
{
	public class TokenInputConnection<T> : InputConnectionWrapper 
		where T:class
	{

		private TokenCompleteTextView<T> _tokenCompleteTextView;

		public TokenInputConnection(TokenCompleteTextView<T> tokenCompleteTextView, IInputConnection target, bool mutable) : base(target, mutable)
		{
			_tokenCompleteTextView = tokenCompleteTextView;
		}

		// This will fire if the soft keyboard delete key is pressed.
		// The onKeyPressed method does not always do this.
		public override bool DeleteSurroundingText(int beforeLength, int afterLength) {
			// Shouldn't be able to delete any text with tokens that are not removable
			if (!_tokenCompleteTextView.CanDeleteSelection(beforeLength)) return false;

			//Shouldn't be able to delete prefix, so don't do anything
			if (_tokenCompleteTextView.SelectionStart <= _tokenCompleteTextView.Prefix.Length) {
				beforeLength = 0;
				return _tokenCompleteTextView.DeleteSelectedObject(false) 
					|| base.DeleteSurroundingText(beforeLength, afterLength);
			}

			return base.DeleteSurroundingText(beforeLength, afterLength);
		}
	}
}