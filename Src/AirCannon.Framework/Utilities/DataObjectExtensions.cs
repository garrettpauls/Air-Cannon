using System.Windows;

namespace AirCannon.Framework.Utilities
{
    public static class DataObjectExtensions
    {
        public static T GetData<T>(this IDataObject data)
        {
            return (T) data.GetData(typeof (T));
        }
    }
}
