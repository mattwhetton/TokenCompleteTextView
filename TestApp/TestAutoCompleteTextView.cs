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

        protected override TokenCompleteTextViewSavedState<string> CreateSavedState(IParcelable superState)
        {
            return new MyTokenCompleteTextViewSavedState(superState);
        }

        public override TokenClickStyle TokenClickStyle
        {
            get { return TokenClickStyle.SelectDeselect; }
        }

        public override bool AllowDuplicates
        {
            get { return false; }
        }

       
    }

    public class MyTokenCompleteTextViewSavedState : TokenCompleteTextViewSavedState<string>
    {
        public MyTokenCompleteTextViewSavedState() : base(source: null)
        {

        }

        public MyTokenCompleteTextViewSavedState(Parcel source) : base(source)
        {
        }

        public MyTokenCompleteTextViewSavedState(IParcelable superState) : base(superState)
        {
        }


        public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            base.WriteToParcel(dest, flags);

        }


        private static readonly Codenutz.Controls.GenericParcelableCreator<MyTokenCompleteTextViewSavedState> _creator = 
            new Codenutz.Controls.GenericParcelableCreator<MyTokenCompleteTextViewSavedState>((parcel) => new MyTokenCompleteTextViewSavedState(parcel));

        [ExportField("CREATOR")]
        public static Codenutz.Controls.GenericParcelableCreator<MyTokenCompleteTextViewSavedState> GetCreator()
        {
            return _creator;
        }
    }




    
}