using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SourceProlongation
{
    public static class MyStatic
    {
        /// <summary>
        /// Возвращает копию объекта. Любого. Да.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException(@"The type must be serializable.", "source");
            }

            //Нулевые объекты не могут быть сериализованы
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T FindParent<T>(this FrameworkElement element) where T : FrameworkElement
        {
            DependencyObject current = element;

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        public static void SaveClass<T>(T instance, string path)
        {
            var formatter = new BinaryFormatter();

            var fs = new FileStream(path, FileMode.Create);
            var gzipStream = new GZipStream(fs, CompressionMode.Compress);

            try
            {
                formatter.Serialize(gzipStream, instance);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                gzipStream.Close();
                fs.Close();
            }
        }

        public static string CleanPath(string path)
        {
            return Path.GetInvalidPathChars()
                .Aggregate(path, (current, c) => current.Replace(c.ToString(), String.Empty));
        }
    }
}
