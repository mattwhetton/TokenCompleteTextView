using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Annotation;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Object = Java.Lang.Object;
using Math = System.Math;
using String = System.String;
using Codenutz.Controls.Extensions;

namespace Codenutz.Controls
{
	public abstract class TokenCompleteTextView<T> : MultiAutoCompleteTextView, TextView.IOnEditorActionListener where T:class
	{
		#region Constants

		public const string TAG = "TokenAutoComplete";

		#endregion

		#region Private members

		private bool _isInitialized = false;
		private bool _isInInvalidate = false;
		private char[] _splitChars = { ',', ';' };
		private ITokenizer _tokenizer;
		private T _selectedItem;
		private bool _isHintVisible = false;
		private List<TokenImageSpan<T>> _hiddenSpans;
		private TokenDeleteStyle _deletionStyle = TokenDeleteStyle._Parent;
		private bool _shouldFocusNext = false;
		private TokenTextWatcher<T> _textWatcher;
		private TokenSpanWatcher<T> _spanWatcher;
		private string _prefix;
		private Layout _lastLayout = null;
	    private ObservableCollection<T> _items;

	    #endregion

		#region Properties

		public virtual TokenClickStyle TokenClickStyle { get; private set; }

		public bool IsTokenClickStyleSelectable
		{
			get
			{
				switch (TokenClickStyle)
				{
					case TokenClickStyle.Select:
					case TokenClickStyle.SelectDeselect:
						return true;
					default:
						return false;
				}
			}
		}

		public virtual int TokenLimit { get; protected set; }

		public virtual string Prefix
		{
			get { return _prefix; }
		    protected set
			{
				_prefix = String.Empty;
				if (EditableText != null)
					EditableText.Insert(0, value);
				_prefix = value;
				UpdateHint();
			}
		}

	    public ObservableCollection<T> Items
	    {
	        get { return _items ?? (_items = new ObservableCollection<T>()); }
	    }

	    public bool IsInSavingState { get; set; }

		public bool IsFocusChanging { get; set; }

		public ITokenListener<T> Listener { get; set; }

		public virtual char[] SplitChars
		{
			get { return _splitChars; }
		    protected set
			{
				if (value[0] == ' ')
					value = new[] { value.Length > 1 ? value[1] : '§', value[0]};
				_splitChars = value;

				SetTokenizer(new CharacterTokenizer(value));
			}
		}

		public virtual bool AllowDuplicates { get; private set; }

		public virtual bool PerformBestGuess { get; set; }

		public virtual bool AllowCollapse { get; set; }

		protected string CurrentCompletionText
		{
			get
			{
				if(_isHintVisible) return String.Empty;

				var end = SelectionEnd;
				var start = _tokenizer.FindTokenStart(EditableText, end);
				if (start < Prefix.Length)
				{
					start = Prefix.Length;
				}
				return TextUtils.Substring(EditableText, start, end);
			}
		}

		protected float MaxTextWidth
		{
			get { return Width - PaddingLeft - PaddingRight; }
		}

		#endregion

		#region Constructors

		protected TokenCompleteTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			Prefix = String.Empty;
			Initialize();
		}

		protected TokenCompleteTextView(Context context) : base(context)
		{
			Prefix = String.Empty;
			Initialize();
		}

		protected TokenCompleteTextView(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			Prefix = String.Empty;
			Initialize();
		}

		protected TokenCompleteTextView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			Prefix = String.Empty;
			Initialize();
		}

		protected TokenCompleteTextView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
		{
			Prefix = String.Empty;
			Initialize();
		}

		#endregion
		
		#region Abstract methods

		abstract protected View GetViewForObject(T target);

		abstract protected T DefaultObject(String completionText);

		#endregion

		#region Methods

		public override void Invalidate()
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
			{
				Api16Invalidate();
			}

			base.Invalidate();
		}
		
		public override bool EnoughToFilter()
		{
			var text = EditableText;

			var end = SelectionEnd;
			if (end < 0 || _tokenizer == null)
				return false;

			var start = _tokenizer.FindTokenStart(text, end);
			if (start < Prefix.Length)
				start = Prefix.Length;

			return end - start >= Math.Max(Threshold, 1);
		}

		public override void PerformCompletion()
		{
			if (ListSelection == AdapterView.InvalidPosition && EnoughToFilter())
			{
				Object bestGuess;
				if (Adapter.Count > 0 && PerformBestGuess)
				{
					bestGuess = Adapter.GetItem(0);
				}
				else
				{
					var defaultObject = DefaultObject(CurrentCompletionText);
					bestGuess = new JavaObjectWrapper<T>(defaultObject);
				}
				
				var selection = ConvertSelectionToString(bestGuess);
				ReplaceText(selection);
			}
			else
			{
				base.PerformCompletion();
			}
		}

		protected override void PerformFiltering(ICharSequence text, int start, int end, int keyCode)
		{
			if (start < Prefix.Length)
			{
				start = Prefix.Length;
			}
			if (Filter != null)
			{
				Filter.InvokeFilter(text.SubSequenceFormatted(start, end), this);
			}
		}

		public override IInputConnection OnCreateInputConnection(EditorInfo outAttrs)
		{
			var conn = new TokenInputConnection<T>(base.OnCreateInputConnection(outAttrs), true,this);
			outAttrs.ImeOptions &= ImeFlags.NoEnterAction;
			outAttrs.ImeOptions |= ImeFlags.NoExtractUi;
			return conn;
		}

		public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
		{
			var handled = base.OnKeyUp(keyCode, e);
			if (_shouldFocusNext)
			{
				_shouldFocusNext = false;
				HandleDone();
			}
			return handled;
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			var handled = false;
			switch (keyCode)
			{
				case Keycode.Tab:
				case Keycode.Enter:
				case Keycode.DpadCenter:
					if (e.HasNoModifiers)
					{
						_shouldFocusNext = true;
						handled = true;
					}
					break;
				case Keycode.Del:
					handled = DeleteSelectedObject(false);
					break;
			}

			return handled || base.OnKeyDown(keyCode, e);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			var action = e.ActionMasked;
			var text = EditableText;
			var handled = false;

			if (TokenClickStyle == TokenClickStyle.None)
			{
				handled = base.OnTouchEvent(e);
			}

			if (IsFocused && text != null && _lastLayout != null && action == MotionEventActions.Up)
			{
				var offset = GetOffsetForPosition(e.GetX(), e.GetY());

				if (offset != -1)
				{
					var links = text.GetSpans<TokenImageSpan<T>>(offset, offset);

					if (links.Length > 0)
					{
						links[0].OnClick();
						handled = true;
					}
					else
					{
						ClearSelections();
					}
				}
			}

			if (!handled && TokenClickStyle != TokenClickStyle.None)
			{
				handled = base.OnTouchEvent(e);
			}

			return handled;
		}

		public override bool ExtractText(ExtractedTextRequest request, ExtractedText outText)
		{
			try
			{
				return base.ExtractText(request, outText);
			}
			catch (IndexOutOfBoundsException ignored)
			{
				Log.Debug(TAG, "ExtractText hit IndexOutOfBoundsException. This may be normal.", ignored);
				return false;
			}
		}
		
		protected override void OnFocusChanged(bool gainFocus, FocusSearchDirection direction, Rect previouslyFocusedRect)
		{
			base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);

			if (!HasFocus) PerformCompletion();

			if(AllowCollapse) PerformCollapse(HasFocus);
		}

		

		protected override void OnSelectionChanged(int selStart, int selEnd)
		{
			if (_isHintVisible)
			{
				selStart = 0;
			}

			selEnd = selStart;

			if (IsTokenClickStyleSelectable)
			{
				if(EditableText != null)
					ClearSelections();
			}

			if (Prefix != null && (selStart < Prefix.Length || selEnd < Prefix.Length))
			{
				SetSelection(Prefix.Length);
			}
			else
			{
				if (EditableText != null)
				{
					var spans = EditableText.GetSpans<TokenImageSpan<T>>(selStart, selEnd);
					foreach (var span in spans)
					{
						var spanEnd = EditableText.GetSpanEnd(span);
						if (selStart <= spanEnd && EditableText.GetSpanStart(span) < selStart)
						{
							if (spanEnd == EditableText.Length())
								SetSelection(spanEnd);
							else
								SetSelection(spanEnd + 1);
							return;
						}
					}
				}
				base.OnSelectionChanged(selStart, selEnd);
			}
		}

		protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);
			_lastLayout = Layout;
		}

		protected override void ReplaceText(ICharSequence text)
		{
			ClearComposingText();

			if(_selectedItem == null  || _selectedItem.ToString().Equals(String.Empty)) return;

			var ssb = BuildSpannableForText(text.ToString());
			var tokenSpan = BuildSpanForItem(_selectedItem);

			var editable = EditableText;
			var end = SelectionEnd;
			var start = _tokenizer.FindTokenStart(editable, end);
			if (start < Prefix.Length)
			{
				start = Prefix.Length;
			}

			var original = TextUtils.Substring(editable, start, end);

			if (editable != null)
			{
				if (tokenSpan == null)
				{
					editable.Replace(start, end, String.Empty);
				}else if (!AllowDuplicates && Items.Contains(tokenSpan.Token))
				{
					editable.Replace(start, end, String.Empty);
				}
				else
				{
					QwertyKeyListener.MarkAsReplaced(editable, start, end, original);
					editable.Replace(start, end, ssb);
					editable.SetSpan(tokenSpan,start, start + ssb.Length() - 1, SpanTypes.ExclusiveExclusive);
				}
			}
		}

		public override void SetTokenizer(ITokenizer t)
		{
			base.SetTokenizer(t);
			_tokenizer = t;
		}
		
		public void PerformCollapse(bool hasFocus)
		{
			try
			{
				IsFocusChanging = true;
				var text = EditableText;

				if (!hasFocus)
				{
					if (text != null && _lastLayout != null)
					{
						var lastPosition = _lastLayout.GetLineVisibleEnd(0);
						var tokens = text.GetSpans<TokenImageSpan<T>>(0, lastPosition);
						var count = Items.Count - tokens.Length;

						var countSpans = text.GetSpans<CountSpan>(0, lastPosition);

						if (count > 0 && countSpans.Length == 0)
						{
							lastPosition++;

							var cs = new CountSpan(count, Context, new Color(CurrentTextColor), (int)TextSize, (int)MaxTextWidth);
							text.Insert(lastPosition, cs.Text);

							var newWidth = Layout.GetDesiredWidth(text, 09, lastPosition + cs.Text.Length, _lastLayout.Paint);

							if (newWidth > MaxTextWidth)
							{
								text.Delete(lastPosition, lastPosition + cs.Text.Length);

								if (tokens.Length > 0)
								{
									var token = tokens[tokens.Length - 1];
									lastPosition = text.GetSpanStart(token);
									cs.SetCount(count + 1);
								}
								else
								{
									lastPosition = Prefix.Length;
								}

								text.Insert(lastPosition, cs.Text);
							}

							text.SetSpan(cs, lastPosition, lastPosition + cs.Text.Length, SpanTypes.ExclusiveExclusive);

							_hiddenSpans =
								new List<TokenImageSpan<T>>(text.GetSpans<TokenImageSpan<T>>(lastPosition + cs.Text.Length, text.Length()));
							foreach (var span in _hiddenSpans)
							{
								RemoveSpan(span);
							}
						}
					}
				}
				else
				{
					if (text != null)
					{
						var counts = text.GetSpans<CountSpan>(0, text.Length());
						foreach (var count in counts)
						{
							text.Delete(text.GetSpanStart(count), text.GetSpanEnd(count));
							text.RemoveSpan(count);
						}

						foreach (var span in _hiddenSpans)
						{
							InsertSpan( span);
						}
						_hiddenSpans.Clear();

						if (_isHintVisible)
						{
							SetSelection(Prefix.Length);
						}
						else
						{
							PostDelayed(() =>
							{
								SetSelection(text.Length());
							}, 10);
						}

						var watchers = EditableText.GetSpans<TokenSpanWatcher<T>>(0, EditableText.Length());
						if (watchers.Length == 0)
							text.SetSpan(_spanWatcher, 0, text.Length(), SpanTypes.InclusiveInclusive);

					}
				}
			}
			finally
			{
				IsFocusChanging = false;
			}
		}

		public void ClearSelections()
		{
			if (EditableText == null) return;

			var length = EditableText.Length();
			var tokens = EditableText.GetSpans<TokenImageSpan<T>>(0, length);
			foreach (var token in tokens)
			{
				token.View.Selected = false;
			}

			Invalidate();
		}

		public void RemoveSpan(TokenImageSpan<T> span)
		{
			var text = EditableText;
			if(EditableText == null)return;

			var spans = text.GetSpans<TokenSpanWatcher<T>>(0, text.Length());
			if(spans.Length == 0)
				_spanWatcher.OnSpanRemoved(text, span, text.GetSpanStart(span),text.GetSpanEnd(span));


			text.Delete(text.GetSpanStart(span), text.GetSpanEnd(span) + 1);

			if(AllowCollapse && !IsFocused)
				UpdateCountSpan();
		}
		
		public void InsertSpan(T item, string sourceText)
		{
			var ssb = BuildSpannableForText(sourceText);
			var tokenSpan = BuildSpanForItem(item);

			var editable = EditableText;

			if (!AllowCollapse || IsFocused || !_hiddenSpans.Any())
			{
				var offset = editable.Length();

				if (_isHintVisible)
				{
					offset = Prefix.Length;
					editable.Insert(offset, ssb);
				}
				else
				{
					var completionText = CurrentCompletionText;
					if (!string.IsNullOrEmpty(completionText))
					{
						offset = TextUtils.IndexOf(editable, completionText.ToAndroidString());
					}
					editable.Insert(offset, ssb);
				}
				editable.SetSpan(tokenSpan, offset, offset + ssb.Length() - 1, SpanTypes.ExclusiveExclusive);

				if (!IsFocused && AllowCollapse) PerformCollapse(false);

				if (!Items.Contains(tokenSpan.Token)) //not sure if this is right?!
				{
					_spanWatcher.OnSpanAdded(editable, tokenSpan, 0, 0);
				}
			}
			else
			{
				_hiddenSpans.Add(tokenSpan);
				_spanWatcher.OnSpanAdded(editable, tokenSpan, 0, 0);
				UpdateCountSpan();
			}

		}

		public void InsertSpan(T item)
		{
			InsertSpan(item, item.ToString());
		}

		public void InsertSpan(TokenImageSpan<T> span)
		{
			InsertSpan(span.Token);
		}

		public void InsertItem(T item, string sourceText)
		{
			Post(() =>
			{
				if (item == null) return;
				if (!AllowDuplicates && Items.Contains(item)) return;
				if (TokenLimit != -1 && Items.Count == TokenLimit) return;
				InsertSpan(item, sourceText);
				if (EditableText != null && IsFocused) SetSelection(EditableText.Length());
			});
		}

		public void AddItem(T item)
		{
			InsertItem(item, String.Empty);
		}

		public void RemoveItem(T item)
		{
			Post(() =>
			{
				var text = EditableText;
				if (text == null) return;

				var toRemove = new List<TokenImageSpan<T>>();
				foreach (var span in _hiddenSpans)
				{
					if(span.Token.Equals(item))
						toRemove.Add(span);
				}
				foreach (var span in toRemove)
				{
					_hiddenSpans.Remove(span);
					_spanWatcher.OnSpanRemoved(text, span, 0, 0);
				}

				UpdateCountSpan();

				var spans = text.GetSpans<TokenImageSpan<T>>(0, text.Length());
				foreach (var span in spans)
				{
					if(span.Token.Equals(item))
						RemoveSpan(span);
				}
			});
		}

		public void Clear()
		{
			Post(() =>
			{
				var text = EditableText;
				if (text == null) return;

				var spans = text.GetSpans<TokenImageSpan<T>>(0, text.Length());
				foreach (var span in spans)
				{
					RemoveSpan(span);
					_spanWatcher.OnSpanRemoved(text, span, text.GetSpanStart(span), text.GetSpanEnd(span));
				}
			});
		}

		public bool IsSplitChar(char c)
		{
			return SplitChars.Contains(c);
		}

		public void UpdateHint()
		{
			var text = EditableText;
			var hintText = Hint;
			if (text == null || hintText == null)
				return;

			if (Prefix.Length > 0)
			{
				var hints = text.GetSpans<HintSpan>(0, text.Length());
				HintSpan hint = null;

				var testLength = Prefix.Length;
				if (hints.Length > 0)
				{
					hint = hints[0];
					testLength += text.GetSpanEnd(hint) - text.GetSpanStart(hint);
				}

				if (text.Length() == testLength)
				{
					_isHintVisible = true;

					if (hint != null)
						return;

					var typeface = Typeface;
					var typefaceStyle = TypefaceStyle.Normal;
					if (typeface != null)
						typefaceStyle = typeface.Style;

					var colors = HintTextColors;

					var hintSpan = new HintSpan(null, typefaceStyle, (int) TextSize, colors, colors);
					text.Insert(Prefix.Length, hintText);
					text.SetSpan(hintSpan, Prefix.Length, Prefix.Length + Hint.Length, SpanTypes.ExclusiveExclusive);
					SetSelection(Prefix.Length);

				}
				else
				{
					if(hint == null)
						return;

					var spanStart = text.GetSpanStart(hint);
					var spanEnd = text.GetSpanEnd(hint);

					text.RemoveSpan(hint);
					text.Replace(spanStart, spanEnd, String.Empty);

					_isHintVisible = false;
				}

			}
		}

		public bool DeleteSelectedObject(bool handled)
		{
			if (IsTokenClickStyleSelectable)
			{
				if (EditableText == null) return handled;

				var spans = EditableText.GetSpans<TokenImageSpan<T>>(0, EditableText.Length());
				foreach (var span in spans)
				{
					if (span.View.Selected)
					{
						RemoveSpan(span);
						handled = true;
						break;
					}
				}
			}
			return handled;
		}

		protected override ICharSequence ConvertSelectionToStringFormatted(Object selectedItem)
		{
			_selectedItem = GetSelectedItem(selectedItem);

			ICharSequence result = null;
			switch (_deletionStyle)
			{
				case TokenDeleteStyle.Clear:
					result = String.Empty.ToAndroidString();
					break;
				case TokenDeleteStyle.PartialCompletion:
					result = CurrentCompletionText.ToAndroidString();
					break;
				case TokenDeleteStyle.ToString:
					result = selectedItem != null ? selectedItem.ToString().ToAndroidString() : String.Empty.ToAndroidString();
					break;
				case TokenDeleteStyle._Parent:
				default:
					result = base.ConvertSelectionToStringFormatted(selectedItem); 
					break;
			}
			return result;
		}

		private T GetSelectedItem(Object selectedItem)
		{
			if (selectedItem == null)
				return null;

			var wrappedSelection = selectedItem as JavaObjectWrapper<T>;

			if (wrappedSelection != null)
				return wrappedSelection.Obj;

			var propertyInfo = selectedItem.GetType().GetProperty("Instance");

			return propertyInfo == null ? null : propertyInfo.GetValue(selectedItem, null) as T;

		}

		private void Initialize()
		{
			if (_isInitialized) return;

			TokenLimit = -1;

			TokenClickStyle = TokenClickStyle.None;

			SetTokenizer(new CommaTokenizer());
			
			_spanWatcher = new TokenSpanWatcher<T>(this);
			_textWatcher = new TokenTextWatcher<T>(this);
			_hiddenSpans = new List<TokenImageSpan<T>>();

			AddListeners();

			SetTextIsSelectable(false);
			LongClickable = false;

			InputType = InputType | InputTypes.TextFlagNoSuggestions | InputTypes.TextFlagAutoComplete;
			SetHorizontallyScrolling(false);

			SetOnEditorActionListener(this);

			SetFilters(new IInputFilter[]
			{
				new TextFilter<T>(this)
			});

			//_deletionStyle = TokenDeleteStyle.Clear;
			_isInitialized = true;
		}

		[TargetApi(Value = (int)BuildVersionCodes.JellyBean)]
		private void Api16Invalidate()
		{
			if (_isInitialized && _isInInvalidate)
			{
				try
				{
					_isInInvalidate = true;
					SetShadowLayer(ShadowRadius, ShadowDx, ShadowDy, ShadowColor);
				}
				finally
				{
					_isInInvalidate = false;
				}
			}
		}

		private void AddListeners()
		{
			if (EditableText != null)
			{
				EditableText.SetSpan(_spanWatcher,0, EditableText.Length(), SpanTypes.InclusiveInclusive);
				AddTextChangedListener(_textWatcher);
			}
		}

		private void RemoveListeners()
		{
			if (EditableText != null)
			{
				var spanWatchers = EditableText.GetSpans<TokenSpanWatcher<T>>(0, EditableText.Length());
				foreach (var watcher in spanWatchers)
				{
					EditableText.RemoveSpan(watcher);
				}
				RemoveTextChangedListener(_textWatcher);
			}
		}

		private void HandleDone()
		{
			PerformCompletion();

			var imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(WindowToken, 0);
		}

		private SpannableStringBuilder BuildSpannableForText(string text)
		{
			char sentinel = SplitChars[0];
			return new SpannableStringBuilder(sentinel + _tokenizer.TerminateToken(text));
		}

		private TokenImageSpan<T> BuildSpanForItem(T item)
		{
			if (item == null)
				return null;

			var tokenView = GetViewForObject(item);
			return new TokenImageSpan<T>(tokenView, item, (int) MaxTextWidth, this);
		}

		private void UpdateCountSpan()
		{
			var text = EditableText;
			var counts = text.GetSpans<CountSpan>(0, text.Length());
			var newCount = _hiddenSpans.Count;
			foreach (var count in counts)
			{
				if (newCount == 0)
				{
					text.Delete(text.GetSpanStart(count), text.GetSpanEnd(count));
					text.RemoveSpan(count);
				}
				else
				{
					count.SetCount(_hiddenSpans.Count);
					text.SetSpan(count,text.GetSpanStart(count),text.GetSpanEnd(count),SpanTypes.ExclusiveExclusive);
				}
			}
		}

		#endregion
		
		#region IOnEditorActionListener members

		public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
		{
			if (actionId == ImeAction.Done)
			{
				HandleDone();
				return true;
			}
			return false;
		}

		#endregion
	}

	//public class SavedState<T> : View.BaseSavedState
	//{
	//	private string prefix;
	//	private bool allowCollpase;
	//	private bool allowDuplicates;
	//	private bool performBestGuess;
	//	private TokenClickStyle tokenClickStyle;
	//	private TokenDeleteStyle tokenDeleteStyle;
	//	private List<T> baseObjects; 
		
	//	public SavedState(Parcel source) : base(source)
	//	{

	//	}

	//	public SavedState(IParcelable superState) : base(superState)
	//	{
	//	}

	//	public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
	//	{
	//		base.WriteToParcel(dest, flags);

	//		dest.WriteSerializable();
	//	}
	//}
}