using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TestApp
{
    public class TestFragment2 : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //var x =  base.OnCreateView(inflater, container, savedInstanceState);
			var x = inflater.Inflate(Resource.Layout.template2,container,false);
			            TestAutoCompleteTextView tv = x.FindViewById<TestAutoCompleteTextView>(Resource.Id.txtTarget);
		
			return x;
        }
        
    }
}