namespace Codenutz.Controls
{
	public enum TokenDeleteStyle {
		_Parent, //...do the parent behavior, not recommended
		Clear, //...clear the underlying text
		PartialCompletion, //...return the original text used for completion
		ToString //...replace the token with toString of the token object
	}
}