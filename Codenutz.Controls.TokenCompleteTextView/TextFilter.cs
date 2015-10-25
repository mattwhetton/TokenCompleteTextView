using Android.Text;
using Java.Lang;
using String = System.String;

namespace Codenutz.Controls
{
	public class TextFilter<T> : Java.Lang.Object, IInputFilter where T:class
	{
		protected readonly TokenCompleteTextView<T> TokenCompleteTextView;

		public TextFilter(TokenCompleteTextView<T> tokenCompleteTextView)
		{
			TokenCompleteTextView = tokenCompleteTextView;
		}

		public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
		{
			if (TokenCompleteTextView.TokenLimit != -1 && TokenCompleteTextView.Items.Count == TokenCompleteTextView.TokenLimit)
			{
				return String.Empty.ToAndroidString();
			}
			else if (source.Length() == 1)
			{
				if (TokenCompleteTextView.IsSplitChar(source.CharAt(0)))
				{
					TokenCompleteTextView.PerformCompletion();
					return String.Empty.ToAndroidString();
				}	
			}

			if (dstart < TokenCompleteTextView.Prefix.Length)
			{
			    var prefixFormatted = TokenCompleteTextView.Prefix.ToAndroidString();
			    if (dend <= TokenCompleteTextView.Prefix.Length)
                {
                    //Don't do anything
                    return prefixFormatted.SubSequenceFormatted(dstart, dend);
                }
                else
                {
                    //Delete everything up to the prefix
                    return prefixFormatted.SubSequenceFormatted(dstart, TokenCompleteTextView.Prefix.Length);
                }
			}

		    return null;
		}


	}
}