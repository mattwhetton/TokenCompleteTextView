using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;

namespace Codenutz.Controls
{
    public abstract class TokenCompleteTextViewSavedState<T> : View.BaseSavedState
    {
        public string Prefix { get; set; }

        public bool AllowCollapse { get; set; }
        public bool AllowDuplicates { get; set; }
        public bool PerformBestGuess { get; set; }
        public TokenClickStyle TokenClickStyle { get; set; }
        public TokenDeleteStyle TokenDeleteStyle { get; set; }
        public ObservableCollection<T> Items { get; set; }
        public char[] SplitChars { get; set; }

        
        public TokenCompleteTextViewSavedState(Parcel source) : base(source)
        {

        }

        public TokenCompleteTextViewSavedState(IParcelable superState) : base(superState)
        {
        }


    }
}