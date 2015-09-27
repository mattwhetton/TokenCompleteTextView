namespace Codenutz.Controls
{
	public class JavaObjectWrapper<T> : Java.Lang.Object
	{
		public JavaObjectWrapper()
		{
			
		}

		public JavaObjectWrapper(T obj)
		{
			Obj = obj;
		}

		public T Obj { get; set; }

		public override string ToString()
		{
			return Obj.ToString();
		}
	}
}