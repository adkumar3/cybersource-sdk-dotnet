using System;
using System.Xml;

namespace CyberSource.Clients
{
	/// <summary>
    /// Exception that is thrown when the server throws a fault.
    /// It is only applicable to the Xml client.
	/// </summary>
	public class FaultException : ApplicationException
	{
		private XmlDocument mFaultDocument = null;
		private XmlQualifiedName mCode = null;
		private string mMessage = null;
		private string mRequestID = null;

		internal FaultException( XmlDocument faultDocument, string nspace )
		{
			mFaultDocument = faultDocument;
			ExtractFields( nspace );
		}

		/// <summary>
		/// Gets the fault document returned by the server.
		/// </summary>
		public XmlDocument FaultDocument
		{
			get
			{
				return( mFaultDocument );
			}
		}

		/// <summary>
		/// Gets the fault code.
		/// </summary>
		public XmlQualifiedName Code
		{
			get
			{
				return( mCode );
			}
		}

		/// <summary>
		/// Gets the fault string.
		/// </summary>
		override public string Message
		{
			get
			{
				return( mMessage );
			}
		}

		/// <summary>
		/// Gets the request id generated by CyberSource.  It will return null
		/// if the request id had not been generated when the server exception
		/// occurred.
		/// </summary>
		public string RequestID
		{
			get
			{
				return( mRequestID );
			}
		}

		private void ExtractFields( string nspace )
		{
			if (mFaultDocument != null)
			{
				// faultstring goes into mMessage
				mMessage = GetTextValue( "faultstring", String.Empty );

				// requestID goes into mRequestID
				mRequestID = GetTextValue(
								"requestID", nspace );

				// faultcode goes into mCode.  If it has a prefix, look up
				// the namespace for that prefix.
				XmlNode faultCodeText = GetText( "faultcode", String.Empty );
                string faultCode = faultCodeText.Value;
				char[] COLON = {':'};
				string[] parts = faultCode.Split( COLON, 2 );
				if (parts.Length == 2)
				{
					string codeNameSpace 
						= faultCodeText.GetNamespaceOfPrefix( parts[0] );

					mCode = new XmlQualifiedName( parts[1], codeNameSpace );
				}
				else
				{
					mCode = new XmlQualifiedName( faultCode );
				}
			}
		}

		private string GetTextValue( string name, string nspace )
		{
			XmlText text = GetText( name, nspace );
			if (text != null)
			{
				return( text.Value );
			}

			return( null );
		}

		private XmlText GetText( string name, string nspace )
		{
			XmlNodeList nodes 
				= mFaultDocument.GetElementsByTagName( name, nspace );
			
			if (nodes.Count > 0 && nodes[0].HasChildNodes)
			{
				return( (XmlText) nodes[0].ChildNodes[0] );
			}

			return( null );
		}

        /// <summary>
        /// Returns the string representation of this exception suitable
        /// for logging.
        /// </summary>
        public string LogString
        {
            get
            {
                return (mFaultDocument.OuterXml);
            }
        }
	}
}
