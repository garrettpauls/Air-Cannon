using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace AirCannon.Framework.Utilities
{
    public class Resource
    {
        private readonly Uri mResourceUri;

        private Resource(Uri resourceUri)
        {
            mResourceUri = resourceUri;
        }

        private StreamResourceInfo GetResource()
        {
            var resource = Application.GetResourceStream(mResourceUri);
            if (resource == null)
            {
                throw new ArgumentException(
                    string.Format("Resource not found at path '{0}' or not property compiled as 'Resource'",
                                  mResourceUri.AbsoluteUri));
            }
            return resource;
        }

        public Stream AsStream()
        {
            return GetResource().Stream;
        }

        public Icon AsIcon()
        {
            return Icon.FromHandle(((Bitmap) Image.FromStream(AsStream())).GetHicon());
        }

        public static Resource FromUri(string resourceUri)
        {
            return new Resource(new Uri(resourceUri));
        }
    }
}
