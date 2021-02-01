using AlfrescoTools.Service;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Data.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfrescoTools.Engine
{
    public class UploadDocumentEngine
    {
        #region Private properties
        private readonly ISession alfresco;
        

        #endregion

        #region Constructor

        public UploadDocumentEngine(ServiceManagerContainer requestFactory)
        {
            this.alfresco = requestFactory.Session;
            
        }

        #endregion

        #region Public methods

        public void Execute(Folder parentFolder, string documentName)
        {
            string SAVING_PATH = @"C:\GeminiPorting\AttachmentDownloaded\";

            // Check if document already exist, if not create it
            Document newDocument = null;

            //Document newDocument = (Document)parentFolder.GetChildren( GetObject(alfresco, parentFolder, documentName);

            if (newDocument == null)
            {
                // Setup document metadata
                Dictionary<String, Object> newDocumentProps =
                        new Dictionary<String, Object>();
                newDocumentProps.Add(PropertyIds.ObjectTypeId, "cmis:document");
                newDocumentProps.Add(PropertyIds.Name, documentName);

                // Setup document content
                String mimetype = "text/plain; charset=UTF-8";
                String documentText = "This is a test document!";
                byte[] bytes = File.ReadAllBytes(SAVING_PATH + documentName);

                IDictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add(PropertyIds.Name, documentName);

                // define object type as document, as we wanted to create document  
                properties.Add(PropertyIds.ObjectTypeId, "cmis:document");

                // read a empty document with empty bytes  
                // fileUpload1: is a .net file upload control  
                ContentStream contentStream = new ContentStream
                {
                    FileName = documentName,
                    MimeType = mimetype,
                    Length = bytes.Length,
                    Stream = new MemoryStream(bytes)
                };

                // this statment would create document in default repository  
                parentFolder.CreateDocument(properties, contentStream, null);

            }
        }
        #endregion
    }
}
