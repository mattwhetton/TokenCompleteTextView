namespace Codenutz.Controls
{
	public interface ITokenListener<in T> where T:class
	{
		void OnTokenAdded(T token);

		void OnTokenRemoved(T token);
	}
}