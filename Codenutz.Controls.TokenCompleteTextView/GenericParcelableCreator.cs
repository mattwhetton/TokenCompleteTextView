using System;
using Android.OS;

namespace Codenutz.Controls
{
    public sealed class GenericParcelableCreator<T> : Java.Lang.Object, IParcelableCreator
        where T : Java.Lang.Object, new()
    {
        private readonly Func<Parcel, T> _createFunc;
        
        public GenericParcelableCreator(Func<Parcel, T> createFromParcelFunc)
        {
            _createFunc = createFromParcelFunc;
        }

        #region IParcelableCreator Implementation

        public Java.Lang.Object CreateFromParcel(Parcel source)
        {
            return _createFunc(source);
        }

        public Java.Lang.Object[] NewArray(int size)
        {
            return new T[size];
        }

        #endregion
    }
}