﻿/*************************
 * 
 * 字符串操作类
 * 
 **************************/
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace XGame
{
	public static class XStringTools
	{

		/// <summary>
		/// 切分文件名，文件后缀
		/// </summary>
		/// <param name="path">全路径</param>
		/// <param name="fileName">文件名</param>
		/// <param name="ext">后缀名</param>
		/// <param name="trimExt">去除后缀文件名</param>
		public static void SplitFileName(string path, out string fileName, out string ext, out string trimExt)
		{
			fileName = "";
			ext = "";
			trimExt = "";
			int refSlash = path.LastIndexOf('/');
			if (refSlash >= 0)
				fileName = path.Substring(refSlash + 1).ToLower();
			int refPoint = path.LastIndexOf('.');
			if (refPoint >= 0)
				ext = path.Substring(refPoint + 1).ToLower();
			if (!string.IsNullOrEmpty(fileName))
				trimExt = fileName.Replace("." + ext, "");
		}

		public static string GetAssetBundleExtName(string ext)
		{
			if (ext.Equals("jpg") || ext.Equals("png"))
			{
				return "_tex";
			}
			else if (ext.Equals("mat"))
			{
				return "_mat";
			}
			else if (ext.Equals("anim"))
			{
				return "_ani";
			}
			else if (ext.Equals("font"))
			{
				return "_fnt";
			}
			else
				return null;
		}

		public static string StringToMD5(string str)
		{
			if (string.IsNullOrEmpty(str))
				return "";

			return ToMD5(Encoding.UTF8.GetBytes(str));
		}

		public static string FileToMd5(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return "";

			FileStream file = new FileStream(filePath, FileMode.Open);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			return ToMD5(retVal);
		}

		public static string ToMD5(byte[] data)
		{
			MD5 md5 = MD5.Create();
			data = md5.ComputeHash(data);

			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; ++i)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}

		public static string CompressString(string str)
		{
			var compressBeforeByte = Encoding.UTF8.GetBytes(str);// Encoding.GetEncoding("UTF-8").GetBytes(str);
			var compressAfterByte = Compress(compressBeforeByte);
			string compressString = Convert.ToBase64String(compressAfterByte);
			return compressString;
		}

		public static string DecompressString(string str)
		{
			var compressBeforeByte = Convert.FromBase64String(str);
			var compressAfterByte = Decompress(compressBeforeByte);
			string compressString = Encoding.UTF8.GetString(compressAfterByte);// Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
			return compressString;
		}

        /// <summary>
        /// Compress，该函数在安卓平台不一定支持，使用时请务必注意，ZipUtil类有另外的实现，可以支持.
        /// </summary>
        public static byte[] Compress(byte[] data)
		{
			try
			{
				var ms = new MemoryStream();
				var zip = new GZipStream(ms, CompressionMode.Compress, true);

                zip.Write(data, 0, data.Length);
				zip.Close();
				var buffer = new byte[ms.Length];
				ms.Position = 0;
				ms.Read(buffer, 0, buffer.Length);
				ms.Close();
				return buffer;

			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		/// <summary>
		/// Decompress，该函数在安卓平台不一定支持，使用时请务必注意，ZipUtil类有另外的实现，可以支持.
		/// </summary>
		public static byte[] Decompress(byte[] data)
		{
			try
			{
				var ms = new MemoryStream(data,2,data.Length-2);
				var zip = new DeflateStream(ms, CompressionMode.Decompress, true);
				var msreader = new MemoryStream();
				var buffer = new byte[100];
				while (true)
				{
					int reader = zip.Read(buffer, 0, buffer.Length);
					if (reader <= 0)
					{
						break;
					}
					msreader.Write(buffer, 0, reader);
				}
				zip.Close();
				ms.Close();
				msreader.Position = 0;
				buffer = msreader.ToArray();
				msreader.Close();
				return buffer;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		/// <summary>
		/// 提取字符串包含的ID
		/// </summary>
		public static string SplitPathId(string path)
		{
			string fileId = "";
			int startPos = path.LastIndexOf("/", System.StringComparison.Ordinal);
			if (startPos != -1) //如果是路径,则取文件名
			{
				path = path.Substring(startPos + 1);
			}
			startPos = path.IndexOf("_", System.StringComparison.Ordinal);
			if (startPos != -1) 
			{
				fileId = path.Substring(0, startPos);//取得编号
			}
			
			return fileId;
		}

	}
}