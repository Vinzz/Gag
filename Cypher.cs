/*
 * Created vincent.tollu@gmail.com
 * IDE: SharpDevelop
 * Distributed under the GPL
 */
 
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;

namespace Gag
{
    public abstract class CypherMgr
    {
        public static string Encrypt( string strText, string strEncrKey ) 
        { 
        	if(strText != null)
        	{
	            byte[] IV = { System.Convert.ToByte( 0X12 ), System.Convert.ToByte( 0X34 ), System.Convert.ToByte( 0X56 ), System.Convert.ToByte( 0X78 ), System.Convert.ToByte( 0X90 ), System.Convert.ToByte( 0XAB ), System.Convert.ToByte( 0XCD ), System.Convert.ToByte( 0XEF ) }; 
	            try 
	            { 
	                byte[] bykey = System.Text.Encoding.UTF8.GetBytes( strEncrKey.Substring( 0, 8) ); 
	                byte[] InputByteArray = System.Text.Encoding.UTF8.GetBytes( strText ); 
	                DESCryptoServiceProvider des = new DESCryptoServiceProvider(); 
	                MemoryStream ms = new MemoryStream(); 
	                CryptoStream cs = new CryptoStream( ms, des.CreateEncryptor( bykey, IV ), CryptoStreamMode.Write ); 
	                cs.Write( InputByteArray, 0, InputByteArray.Length ); 
	                cs.FlushFinalBlock(); 
	                return Convert.ToBase64String( ms.ToArray() ); 
	            } 
	            catch ( Exception ex ) 
	            { 
	                return ex.Message; 
	            }
        	}
        	else return "";
        } 
        
        public static string Decrypt( string strText, string sDecrKey ) 
        { 
            byte[] IV = { System.Convert.ToByte( 0X12 ), System.Convert.ToByte( 0X34 ), System.Convert.ToByte( 0X56 ), System.Convert.ToByte( 0X78 ), System.Convert.ToByte( 0X90 ), System.Convert.ToByte( 0XAB ), System.Convert.ToByte( 0XCD ), System.Convert.ToByte( 0XEF ) }; 
            byte[] inputByteArray = new byte[ strText.Length + 1 ]; 
            try 
            { 
                byte[] byKey = System.Text.Encoding.UTF8.GetBytes(  sDecrKey.Substring( 0, 8) ); 
                DESCryptoServiceProvider des = new DESCryptoServiceProvider(); 
                inputByteArray = Convert.FromBase64String( strText ); 
                MemoryStream ms = new MemoryStream(); 
                CryptoStream cs = new CryptoStream( ms, des.CreateDecryptor( byKey, IV ), CryptoStreamMode.Write ); 
                cs.Write( inputByteArray, 0, inputByteArray.Length ); 
                cs.FlushFinalBlock(); 
                System.Text.Encoding encoding = System.Text.Encoding.UTF8; 
                return encoding.GetString( ms.ToArray() ); 
            } 
            catch ( Exception ex ) 
            { 
                return ex.Message; 
            } 
        }
    }
}
