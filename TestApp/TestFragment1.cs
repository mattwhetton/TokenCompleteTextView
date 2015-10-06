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

using Codenutz.Controls;

namespace TestApp
{
    public class TestFragment1 : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.template1, container, false);

            var objects = new List<string>() {
                            "test1",
                            "test2",
                            "test3",
                            "test4",
                        };
            
            var adapter = new MyAdapter(Activity, Resource.Layout.item_template1, Resource.Id.txtItemTemplateText,objects);
            view.FindViewById<TestAutoCompleteTextView>(Resource.Id.txtTarget).Adapter = adapter;
            
            return view;
        }
        
    }

    public class MyAdapter : FilteredArrayAdapter<string>
    {
        protected override bool KeepObject(string obj, string mask)
        {
            return true;
        }

        public MyAdapter(Context context, int resource, string[] objects) : base(context, resource, objects)
        {
        }

        public MyAdapter(Context context, int resource, int textViewResourceId, string[] objects) : base(context, resource, textViewResourceId, objects)
        {
        }

        public MyAdapter(Context context, int resource, List<string> objects) : base(context, resource, objects)
        {
        }

        public MyAdapter(Context context, int resource, int textViewResourceId, IList<string> objects) : base(context, resource, textViewResourceId, objects)
        {
        }
    }
}