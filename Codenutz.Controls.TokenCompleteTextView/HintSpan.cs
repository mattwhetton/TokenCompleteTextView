using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace Codenutz.Controls
{
	public class HintSpan : TextAppearanceSpan
	{
		protected HintSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public HintSpan(Context context, int appearance) : base(context, appearance)
		{
		}

		public HintSpan(Context context, int appearance, int colorList) : base(context, appearance, colorList)
		{
		}

		public HintSpan(Parcel src) : base(src)
		{
		}

		public HintSpan(string family, TypefaceStyle style, int size, ColorStateList color, ColorStateList linkColor) : base(family, style, size, color, linkColor)
		{
		}
	}
}