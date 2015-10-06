using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Codenutz.Controls;
using Java.Interop;

namespace TestApp
{
    public class TestAutoCompleteTextView : TokenCompleteTextView<String>
    {
        #region Constructors

        public TestAutoCompleteTextView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public TestAutoCompleteTextView(Context context) : base(context)
        {
        }

        public TestAutoCompleteTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public TestAutoCompleteTextView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        public TestAutoCompleteTextView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        protected override View GetViewForObject(string target)
        {
            var layoutInflater = (LayoutInflater) Context.GetSystemService(Context.LayoutInflaterService);
            var view = (LinearLayout) layoutInflater.Inflate(Resource.Layout.item_template1, null, false);
            view.FindViewById<TextView>(Resource.Id.txtItemTemplateText).Text = target;

            return view;
        }

        protected override string DefaultObject(string completionText)
        {
            return "xxx";
        }

        public override TokenClickStyle TokenClickStyle
        {
            get { return TokenClickStyle.SelectDeselect; }
        }

        public override bool AllowDuplicates
        {
            get { return false; }
        }

        public override IParcelable OnSaveInstanceState()
        {
            //return base.OnSaveInstanceState();

            RemoveListeners();

            //_savingState = true;
            var superState = base.OnSaveInstanceState();
            //_savingState = false;
            var state = new MySavedState(superState);

            state.Prefix = Prefix;
            state.AllowCollapse = AllowCollapse;
            state.AllowDuplicates = AllowDuplicates;
            state.PerformBestGuess = PerformBestGuess;
            state.TokenClickStyle = TokenClickStyle;
            state.TokenDeleteStyle = TokenDeletionStyle;
            state.Items = Items;
            state.SplitChars = SplitChars;

            AddListeners();

            return state;
        }

        public override void OnRestoreInstanceState(IParcelable state)
        {
            if (!(state is MySavedState))
            {
                base.OnRestoreInstanceState(state);
                return;
            }

            var ss = (MySavedState) state;
            base.OnRestoreInstanceState(ss.SuperState);

            Text = ss.Prefix;
            Prefix = ss.Prefix;
            UpdateHint();
            AllowCollapse = ss.AllowCollapse;
            AllowDuplicates = ss.AllowDuplicates;
            PerformBestGuess = ss.PerformBestGuess;
            TokenClickStyle = ss.TokenClickStyle;
            TokenDeletionStyle = ss.TokenDeleteStyle;
            SplitChars = ss.SplitChars;

            AddListeners();
            foreach (var item in ss.Items)
            {
                AddItem(item);
            }

            // Collapse the view if necessary
            if (!IsFocused && AllowCollapse)
            {
                Post(() =>
                {
                    PerformCollapse(IsFocused);
                });
            }

        }
    }

    public class MySavedState : SavedState<string>
    {
        public MySavedState() : base(source: null)
        {

        }

        public MySavedState(Parcel source) : base(source)
        {
        }

        public MySavedState(IParcelable superState) : base(superState)
        {
        }


        public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);

        }


        private static readonly Codenutz.Controls.GenericParcelableCreator<MySavedState> _creator = 
            new Codenutz.Controls.GenericParcelableCreator<MySavedState>((parcel) => new MySavedState(parcel));

        [ExportField("CREATOR")]
        public static Codenutz.Controls.GenericParcelableCreator<MySavedState> GetCreator()
        {
            return _creator;
        }
    }




    
}