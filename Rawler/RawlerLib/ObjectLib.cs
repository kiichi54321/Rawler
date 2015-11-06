using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;

namespace RawlerLib
{
    internal class ObjectLib
    {
        /// <summary>
        /// オブジェクトの中身を書き出します。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectPropertiesString(object obj)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(obj.GetType().ToString() + "¥n");

            //プロパティを列挙する。
            System.Reflection.PropertyInfo[] properties = obj.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                //読込み可能なプロパティのみを対象とする。
                if (properties[i].CanRead)
                {
                    System.Reflection.ParameterInfo[] param =
                             properties[i].GetGetMethod().GetParameters();
                    if ((param != null) && (param.Length > 0))
                    {
                        continue;
                    }

                    //プロパティから値を取得し、その文字列表記を保存する。
                    object v = properties[i].GetValue(obj, null);
                    
                    sb.Append(properties[i].Name);
                    sb.Append(" = ");
                    sb.Append("¥'" + v.ToString() + "¥'¥n");

                }
            }

            return sb.ToString();
        }

        public static bool SaveXML(object obj, Type type, string filename)
        {
            if (filename.Length == 0)
            {
                return false;
            }
            bool flag = true;
            //XmlSerializerオブジェクトを作成
            //書き込むオブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(type);
            //ファイルを開く
            System.IO.FileStream fs =
                new System.IO.FileStream(filename, System.IO.FileMode.Create);
            try
            {
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(fs, obj);
            }
            catch
            {
                flag = false;
            }
            finally
            {
                if (fs != null)
                {
                    //閉じる
                    fs.Close();
                }
            }
            return flag;

        }

        public static object LoadXML(Type type, string filename)
        {
            System.IO.Stream stream = null;
            
            object tmp = null;
            try
            {
                stream = System.IO.File.OpenRead(filename);
                XmlSerializer serializer = new XmlSerializer(type);
                tmp = serializer.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
            return tmp;
        }

        /// <summary>
        /// オブジェクトの内容をファイルから読み込み復元する
        /// </summary>
        /// <param name="path">読み込むファイル名</param>
        /// <returns>復元されたオブジェクト</returns>
        public static object LoadFromBinaryFile(string path)
        {
            FileStream fs = new FileStream(path,
                FileMode.Open,
                FileAccess.Read);
            BinaryFormatter f = new BinaryFormatter();
            //読み込んで逆シリアル化する
            object obj = f.Deserialize(fs);
            fs.Close();

            return obj;
        }

        /// <summary>
        /// オブジェクトの内容をファイルから読み込み復元する
        /// </summary>
        /// <param name="path">読み込むファイル名</param>
        /// <returns>復元されたオブジェクト</returns>
        public static object LoadFromZipBinaryFile(string gzipFile)
        {
            //展開する書庫のFileStreamを作成する
            System.IO.FileStream gzipFileStrm = new System.IO.FileStream(
                gzipFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //圧縮解除モードのGZipStreamを作成する
            System.IO.Compression.GZipStream gzipStrm =
                new System.IO.Compression.GZipStream(gzipFileStrm,
                    System.IO.Compression.CompressionMode.Decompress);


            BinaryFormatter f = new BinaryFormatter();
            //読み込んで逆シリアル化する
            object obj = f.Deserialize(gzipStrm);
            gzipStrm.Close();

            return obj;
        }


        /// <summary>
        /// オブジェクトの内容をファイルに保存する
        /// </summary>
        /// <param name="obj">保存するオブジェクト</param>
        /// <param name="path">保存先のファイル名</param>
        public static void SaveToBinaryFile(object obj, string path)
        {
            FileStream fs = new FileStream(path,
                FileMode.Create,
                FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            //シリアル化して書き込む
            bf.Serialize(fs, obj);
            fs.Close();
        }


        /// <summary>
        /// オブジェクトを圧縮して書き込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        public static void WriteWithCompress(string filePath, object obj)
        {
            //using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            //{
            //    using (DeflateStream ds = new DeflateStream(stream, CompressionMode.Compress, true))
            //    {
            //        IFormatter formatter = new BinaryFormatter();
            //        formatter.Serialize(ds, obj);
            //    }
            //}

            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                buffer = ms.ToArray();
            }
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    ds.Write(buffer, 0, buffer.Length);
                }
                buffer = ms.ToArray();
            }
            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 圧縮されたオブジェクトを解凍して読み込み
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static object ReadWithDecompress(string filePath)
        {
            object obj;
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (DeflateStream ds = new DeflateStream(stream, CompressionMode.Decompress))
            {
                IFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(ds);
            }
            return obj;
        }


        /// <summary>
        /// オブジェクトの内容をファイルに保存する
        /// </summary>
        /// <param name="obj">保存するオブジェクト</param>
        /// <param name="path">保存先のファイル名</param>
        public static void SaveToZipBinaryFile(object obj, string gzipFile)
        {
            //作成する圧縮ファイルのFileStreamを作成する
            System.IO.FileStream compFileStrm =
                new System.IO.FileStream(gzipFile, System.IO.FileMode.Create);
            //圧縮モードのGZipStreamを作成する
            System.IO.Compression.GZipStream gzipStrm =
                new System.IO.Compression.GZipStream(compFileStrm,
                    System.IO.Compression.CompressionMode.Compress);


            BinaryFormatter bf = new BinaryFormatter();
            //シリアル化して書き込む
            bf.Serialize(gzipStrm, obj);
            gzipStrm.Close();
        }


        /// <summary>
        /// クラスのフィールドをコピーします。
        /// </summary>
        /// <param name="orignal"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool FildCopy(object orignal, object target)
        {
            if (orignal == null)
            {
                return false;
            }
            if (orignal.GetType().Equals(target.GetType()))
            {
                PropertyInfo[] fields = orignal.GetType().GetProperties();
                foreach (PropertyInfo field in fields)
                {
                    if (field.CanRead)
                    {
                        object val = field.GetValue(orignal, null);
                        if (field.CanWrite)
                        {
                            
                            field.SetValue(target, val, null);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
