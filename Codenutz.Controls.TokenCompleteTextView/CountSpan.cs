using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;

namespace Codenutz.Controls
{
	public class CountSpan : ViewSpan
	{
		public string Text { get; set; }

		public CountSpan(int count, Context context, Color textColor, int textSize, int maxWidth)
			: base(new TextView(context), maxWidth)
		{
			TextView v = (TextView)View;
			v.SetTextColor(textColor);
			
			v.SetTextSize(ComplexUnitType.Px, textSize);
			SetCount(count);
		}

		public void SetCount(int c)
		{
			Text = "+" + c;
			((TextView)View).SetText(Text, TextView.BufferType.Normal);
		}
	}
}