using Android.Graphics;
using Android.Text.Style;
using Android.Views;
using Java.Lang;

namespace Codenutz.Controls
{
	public class ViewSpan : ReplacementSpan
	{
		#region Private members

		private int _maxWidth;

		#endregion

		#region Properties

		public View View { get; set; }

		#endregion

		#region Constructor

		public ViewSpan(View view, int maxWidth) : base()
		{
			View = view;
			_maxWidth = maxWidth;

			View.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent,
				ViewGroup.LayoutParams.WrapContent);
		}

		#endregion

		#region Private methods

		private void PrepView()
		{
			var widthSpec = View.MeasureSpec.MakeMeasureSpec(_maxWidth, MeasureSpecMode.AtMost);
			var heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

			View.Measure(widthSpec, heightSpec);
			View.Layout(0, 0, View.MeasuredWidth, View.MeasuredHeight);
		}

		#endregion

		#region Public methods

		public override void Draw(Canvas canvas, ICharSequence text, int start, int end, float x, int top, int y, int bottom, Paint paint)
		{
			PrepView();

			canvas.Save();

			//Centering the token looks like a better strategy that aligning the bottom
			int padding = (bottom - top - View.Bottom) / 2;
			canvas.Translate(x, bottom - View.Bottom - padding);
			View.Draw(canvas);
			canvas.Restore();
		}

		public override int GetSize(Paint paint, ICharSequence text, int start, int end, Paint.FontMetricsInt fm)
		{
			PrepView();

			if (fm != null)
			{
				//We need to make sure the layout allots enough space for the view
				int height = View.MeasuredHeight;
				int need = height - (fm.Descent - fm.Ascent);
				if (need > 0)
				{
					int ascent = need / 2;
					//This makes sure the text drawing area will be tall enough for the view
					fm.Descent += need - ascent;
					fm.Ascent -= ascent;
					fm.Bottom += need - ascent;
					fm.Top -= need / 2;
				}
			}

			return View.Right;
		}

		#endregion
	}
}
