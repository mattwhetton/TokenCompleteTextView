using System;
using System.Collections.Generic;
using Android.Text;
using Android.Widget;
using Java.Lang;

namespace Codenutz.Controls
{
	public class CharacterTokenizer : MultiAutoCompleteTextView.ITokenizer
	{
		#region Properties

		public List<char> SplitChar { get; set; }

		#endregion

		#region Constructor and Disposal

		public CharacterTokenizer() : this(',')
		{
		}
		
		public CharacterTokenizer(char splitChar)
			: this(new char[]{splitChar})
		{
		}
		
		public CharacterTokenizer(char[] splitChar) : base()
		{
			SplitChar = new List<char>(splitChar);
		}

		public void Dispose()
		{
		}

		#endregion

		#region Public Methods

		public int FindTokenStart(ICharSequence text, int cursor)
		{
			int i = cursor;

			while (i > 0 && !SplitChar.Contains(text.CharAt(i - 1)))
			{
				i--;
			}
			while (i < cursor && text.CharAt(i) == ' ')
			{
				i++;
			}

			return i;
		}

		public int FindTokenEnd(ICharSequence text, int cursor)
		{
			int i = cursor;
			int len = text.Length();

			while (i < len)
			{
				if (SplitChar.Contains(text.CharAt(i)))
				{
					return i;
				}
				else
				{
					i++;
				}
			}

			return len;
		}

		public ICharSequence TerminateTokenFormatted(ICharSequence text)
		{
			 int i = text.Length();

        while (i > 0 && text.CharAt(i - 1) == ' ') {
            i--;
        }

        if (i > 0 && SplitChar.Contains(text.CharAt(i - 1))) {
            return text;
        } else {
            // Try not to use a space as a token character
	        var token = (SplitChar.Count > 1 && SplitChar[0] == ' ' ? SplitChar[1] : SplitChar[0]) + " ";
            
			if (text is ISpanned) {
                SpannableString sp = new SpannableString(text + token);

				TextUtils.CopySpansFrom((ISpanned)text, 0, text.Length(), null, sp, 0);
                return sp;
            } else {
                return (text + token).ToAndroidString();
            }
        }
		}


		#endregion

		#region Unused

		public IntPtr Handle { get; private set; }

		#endregion
	}
}