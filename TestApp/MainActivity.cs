using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Codenutz.Controls;

namespace TestApp
{
    [Activity(Label = "TestApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

			var btn = FindViewById<Button> (Resource.Id.btn);
			btn.Click += btn_Click;

            // Get our button from the layout resource,
            // and attach an event to it
			var frag = new TestFragment1();
			var ft = FragmentManager.BeginTransaction ();
			ft.Add (Resource.Id.fragment_container, frag);
			ft.Commit ();


        }

        public class TestSuggestionsAdapter : DynamicFilteredArrayAdapter<String>
        {
            public TestSuggestionsAdapter(Context context, int resource, Func<string, List<string>> query) : base(context, resource, query)
            {
            }

            public TestSuggestionsAdapter(Context context, int resource, int textViewResourceId, Func<string, List<string>> query) : base(context, resource, textViewResourceId, query)
            {
            }
        }

		private void btn_Click(object sender, EventArgs args){
			var frag = new TestFragment2();
			var ft = FragmentManager.BeginTransaction ();
			ft.Replace(Resource.Id.fragment_container, frag);
			ft.AddToBackStack("test");
			ft.Commit ();
		}
    }
}

