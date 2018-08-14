using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.SqlServer.Server;
using SalesForceCLR.WebService;
using PartnerService = SalesForceCLR.WebService;
using SalesForceCLR.MetaDataService;
using MetaService = SalesForceCLR.MetaDataService;

using System.Globalization;

namespace Salesforce
{
    
    public class CLR
    {

        public CLR()
        {
        }

        #region Result Structures

        struct ValidationRule
        {
            public string ObjectName;
            public string RuleName;
            public Boolean Active;
        }

        //********************************************************************************************************************************
        //upload attachment results structure
        //********************************************************************************************************************************
        struct AttachmentStruct
        {
            public Boolean Uploaded;
            public string Message;
            
        }
        private static void Fill_AttachmentStruct(
            object source,
            out SqlBoolean Uploaded,
            out SqlString Message)
        {
            SaveResults obj = (SaveResults)source;
            Uploaded = obj.Success;
            Message = obj.Message;
        }

        //********************************************************************************************************************************
        // upsert / delete results structure
        //********************************************************************************************************************************
        struct SaveResults
        {
            public SqlString ObjectType;
            public SqlString Action;
            public SqlString Id;
            public SqlString ExternalId;
            public SqlBoolean Success;
            public SqlString Message;
        }
        private static void Fill_SaveResults(
            object source,
            out SqlString ObjectType,
            out SqlString Action,
            out SqlString Id,
            out SqlString ExternalId,
            out SqlBoolean Success,
            out SqlString Message)
        {
            SaveResults obj = (SaveResults)source;
            ObjectType = obj.ObjectType;
            Action = obj.Action;
            Id = obj.Id;
            ExternalId = obj.ExternalId;
            Success = obj.Success;
            Message = obj.Message;
        }

        //********************************************************************************************************************************
        // generic results structure
        //********************************************************************************************************************************
        struct GenericResults
        {
            public SqlString ObjectType;
            public SqlString Id;
            public SqlString Name;
            public SqlString CreatedById;
            public SqlDateTime CreatedDate;
            public SqlString LastModifiedById;
            public SqlDateTime LastModifiedDate;
            public SqlDateTime LastActivityDate;
            public SqlDateTime SystemModstamp;
            public SqlXml Data;
        }
        private static void Fill_GenericResults(
            object source,
            out SqlString ObjectType,
            out SqlString Id,
            out SqlString Name,
            out SqlString CreatedById,
            out SqlDateTime CreatedDate,
            out SqlString LastModifiedById,
            out SqlDateTime LastModifiedDate,
            out SqlDateTime LastActivityDate,
            out SqlDateTime SystemModstamp,
            out SqlXml Data)
        {
            GenericResults obj = (GenericResults)source;
            ObjectType = obj.ObjectType;
            Id = obj.Id;
            Name = obj.Name;
            CreatedById = obj.CreatedById;
            CreatedDate = obj.CreatedDate;
            LastModifiedById = obj.LastModifiedById;
            LastModifiedDate = obj.LastModifiedDate;
            LastActivityDate = obj.LastActivityDate;
            SystemModstamp = obj.SystemModstamp;
            Data = obj.Data;
        }

        //********************************************************************************************************************************
        // Schema results structure
        //********************************************************************************************************************************
        struct SchemaResults
        {
            public SqlString Source;
        }
        private static void Fill_SchemaResults(
            object sourceObject,
            out SqlString Source)
        {
            SchemaResults obj = (SchemaResults)sourceObject;
            Source = obj.Source;
        }

        //********************************************************************************************************************************
        // RelationShips structure
        //********************************************************************************************************************************
        /*      cascadeDelete: true
                cascadeDeleteField: true
                childSObject: "TopicAssignment"
                childSObjectField: "TopicAssignment"
                deprecatedAndHidden: false
                deprecatedAndHiddenField: false
                @field: "EntityId"
                fieldField: "EntityId"
                junctionIdListNames: null
                junctionIdListNamesField: null
                junctionReferenceTo: null
                junctionReferenceToField: null
                relationshipName: "TopicAssignments"
                relationshipNameField: "TopicAssignments"
                restrictedDelete: false
                restrictedDeleteField: false
                restrictedDeleteFieldSpecified: false
                IsMasterDetail: false
        */


        struct Relationships
        {
            public SqlString ParentObject;
            public SqlString ChildObject;
            public SqlString ChildField;
            public SqlString RelationshipName;
            public SqlBoolean CascadeDelete;
            public SqlBoolean RestrictedDelete;
            public SqlBoolean IsMasterDetail;
        }
        private static void Fill_Relationships(
            object sourceObject,
            out SqlString ParentObject,
            out SqlString ChildObject,
            out SqlString ChildField,
            out SqlString RelationshipName,
            out SqlBoolean CascadeDelete,
            out SqlBoolean RestrictedDelete,
            out SqlBoolean IsMasterDetail
            )
        {
            Relationships obj = (Relationships)sourceObject;
            ParentObject = obj.ParentObject;
            ChildObject = obj.ChildObject;
            ChildField = obj.ChildField;
            RelationshipName = obj.RelationshipName;
            CascadeDelete = obj.CascadeDelete;
            RestrictedDelete = obj.RestrictedDelete;
            IsMasterDetail = obj.IsMasterDetail;
        }

        //********************************************************************************************************************************
        // Replication status results structure
        //********************************************************************************************************************************
        struct ReplicationResults
        {
            public SqlString ObjectName;
            public SqlString LastId;
            public SqlDateTime SystemModstamp;
            public SqlInt32 ObjectCount;
            public SqlInt32 ChangeCount;
            public SqlXml SchemaChanges;
            public SqlXml Changes;

        }
        private static void Fill_ReplicationResults(
            object sourceObject,
            out SqlString ObjectName,
            out SqlString LastId,
            out SqlDateTime SystemModstamp,
            out SqlInt32 ObjectCount,
            out SqlInt32 ChangeCount,
            out SqlXml SchemaChanges,
            out SqlXml Changes
            )
        {
            ReplicationResults obj = (ReplicationResults)sourceObject;
            ObjectName = obj.ObjectName;
            LastId = obj.LastId;
            SystemModstamp = obj.SystemModstamp;
            ObjectCount = obj.ObjectCount;
            ChangeCount = obj.ChangeCount;
            SchemaChanges = obj.SchemaChanges;
            Changes = obj.Changes;
        }


        //********************************************************************************************************************************
        // ObjectDescribe results structure
        //********************************************************************************************************************************
        struct ObjectDescribeResults
        {
            public SqlString Name;
            public SqlBoolean Activateable;
            public SqlBoolean Createable;
            public SqlBoolean Custom;
            public SqlBoolean CustomSetting;
            public SqlBoolean Deletable;
            public SqlBoolean DeprecatedAndHidden;
            public SqlBoolean FeedEnabled;
            public SqlString KeyPrefix;
            public SqlString Label;
            public SqlString LabelPlural;
            public SqlBoolean Layoutable;
            public SqlBoolean Mergeable;
            public SqlBoolean Queryable;
            public SqlBoolean Replicateable;
            public SqlBoolean Retrieveable;
            public SqlBoolean Searchable;
            public SqlBoolean Triggerable;
            public SqlBoolean Undeletable;
            public SqlBoolean Updateable;
        }
        private static void Fill_ObjectDescribeResults(
            object sourceObject,
            out SqlString Name,
            out SqlBoolean Activateable,
            out SqlBoolean Createable,
            out SqlBoolean Custom,
            out SqlBoolean CustomSetting,
            out SqlBoolean Deletable,
            out SqlBoolean DeprecatedAndHidden,
            out SqlBoolean FeedEnabled,
            out SqlString KeyPrefix,
            out SqlString Label,
            out SqlString LabelPlural,
            out SqlBoolean Layoutable,
            out SqlBoolean Mergeable,
            out SqlBoolean Queryable,
            out SqlBoolean Replicateable,
            out SqlBoolean Retrieveable,
            out SqlBoolean Searchable,
            out SqlBoolean Triggerable,
            out SqlBoolean Undeletable,
            out SqlBoolean Updateable)
        {
            ObjectDescribeResults obj = (ObjectDescribeResults)sourceObject;
            Name = obj.Name;
            Activateable = obj.Activateable;
            Createable = obj.Createable;
            Custom = obj.Custom;
            CustomSetting = obj.CustomSetting;
            Deletable = obj.Deletable;
            DeprecatedAndHidden = obj.DeprecatedAndHidden;
            FeedEnabled = obj.FeedEnabled;
            KeyPrefix = obj.KeyPrefix;
            Label = obj.Label;
            LabelPlural = obj.LabelPlural;
            Layoutable = obj.Layoutable;
            Mergeable = obj.Mergeable;
            Queryable = obj.Queryable;
            Replicateable = obj.Replicateable;
            Retrieveable = obj.Retrieveable;
            Searchable = obj.Searchable;
            Triggerable = obj.Triggerable;
            Undeletable = obj.Undeletable;
            Updateable = obj.Updateable;
        }


        #endregion

        #region Class Methods

        public static int GetConnectionFromDatabase(ref string salesForceUserID, ref string salesForcePassword, ref string salesForceToken, ref string serviceURL, ref string dataGatewayServiceURL)
        {
            int spid = 0;
            string sql = string.Empty;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;


            using (SqlConnection cn = new SqlConnection("context connection=true;"))
            {
                cn.Open();
                sql = "Select Top 1 @@SPID spid, salesForceUserID, salesForcePassword, salesForceToken, IsNull(ServiceUrl,'') ServiceUrl, IsNull(DataGatewayServiceUrl,'') DataGatewayServiceUrl From SalesForceAPI Where Mode = (Select Top 1 Mode From SalesForceAPIMode)";
                SqlCommand cmd = new SqlCommand(sql, cn);
                using (var rdrConnection = cmd.ExecuteReader())
                {
                    while (rdrConnection.Read())
                    {
                        if (!rdrConnection.IsDBNull(0)) spid = rdrConnection.GetInt16(0);
                        if (!rdrConnection.IsDBNull(1)) salesForceUserID = rdrConnection.GetString(1);
                        if (!rdrConnection.IsDBNull(2)) salesForcePassword = rdrConnection.GetString(2);
                        if (!rdrConnection.IsDBNull(3)) salesForceToken = rdrConnection.GetString(3);
                        if (!rdrConnection.IsDBNull(4)) serviceURL = rdrConnection.GetString(4);
                        if (!rdrConnection.IsDBNull(5)) dataGatewayServiceURL = rdrConnection.GetString(5);

                    }
                }
                cn.Close();
            }
            return spid;
        }

        public static SforceService CreateSFConnection(ref LoginResult sfLoginResult, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            SforceService sfPartnerBinding = new SforceService();
            salesForcePassword = DecryptPwd(salesForcePassword, salesForceToken).Value;

            // override the url for the service endpoint if it is passed. 
            if (!string.IsNullOrEmpty(serviceURL))
            {
                sfPartnerBinding.Url = serviceURL;
            }

            if (!string.IsNullOrEmpty(salesForceToken) && salesForceToken != "<NO-TOKEN>")
            {
                salesForcePassword += salesForceToken;
            }

            sfLoginResult = sfPartnerBinding.login(salesForceUserID, salesForcePassword);
            sfPartnerBinding.SessionHeaderValue = new PartnerService.SessionHeader();
            sfPartnerBinding.SessionHeaderValue.sessionId = sfLoginResult.sessionId;
            int idx1 = sfLoginResult.serverUrl.IndexOf(@"/services/");
            sfPartnerBinding.Url = sfLoginResult.serverUrl;
            return sfPartnerBinding;
        }

        public static MetadataService CreateMetaDataConnection(LoginResult sfLoginResult)
        {

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            MetadataService mtDataService = new MetadataService();
            mtDataService.SessionHeaderValue = new MetaService.SessionHeader();
            mtDataService.SessionHeaderValue.sessionId = sfLoginResult.sessionId;

            mtDataService.Url = sfLoginResult.metadataServerUrl;
            return mtDataService;
        }

        #endregion

        #region Encryption

        [SqlFunction()]
        public static SqlString EncryptPwd(string pwd, string key)
        {
            byte[] computedKey = System.Text.Encoding.ASCII.GetBytes(key.PadRight(24, 'Z').Substring(0, 24));
            byte[] iv = System.Text.Encoding.ASCII.GetBytes("USAZBGAW");
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();

            return Base64Encode(Transform(pwd, provider.CreateEncryptor(computedKey, iv)));
        }

        private static SqlString DecryptPwd(string encryptedText, string key)
        {
            string decodedText = Base64Decode(encryptedText);
            byte[] computedKey = System.Text.Encoding.ASCII.GetBytes(key.PadRight(24, 'Z').Substring(0, 24));
            byte[] iv = System.Text.Encoding.ASCII.GetBytes("USAZBGAW");
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();

            return Transform(decodedText, provider.CreateDecryptor(computedKey, iv));
        }

        private static string Transform(string text, ICryptoTransform transform)
        {
            if (text == null)
            {
                return null;
            }
            
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                {
                    byte[] input = System.Text.Encoding.Default.GetBytes(text);
                    cryptoStream.Write(input, 0, input.Length);
                    cryptoStream.FlushFinalBlock();

                    return System.Text.Encoding.Default.GetString(stream.ToArray());
                }
            }
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        #endregion

        #region Special Functions

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read)]
        public static SqlInt32 sfGetCount(string objectName, string filter)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;
            
            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);

            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            String sql = "Select Count() From " + objectName + (String.IsNullOrEmpty(filter) ? "" : " Where " + filter);

            QueryResult qr = sfPartnerBinding.query(sql);
            return qr.size;
        }

        /*
        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_AttachmentStruct", TableDefinition = "Uploaded bit, Message nvarchar(max)")]
        public static IEnumerable sfUploadAttachment(string objectName, string id, string docName, string url)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;

            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);

            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

            AttachmentStruct result = new AttachmentStruct();
            try {
                String sql = "Select Id From " + objectName + " Where Id='" + id + "'";
                QueryResult qr = sfPartnerBinding.query(sql);

                if (qr.size == 0) {
                    result.Uploaded = false;
                    result.Message = "Parent object not found.";
                }
                else
                {
                    byte[] inbuff = File.ReadAllBytes(@"filename");

                    MessageElement[] attachment = new MessageElement[3];
                    attachment[0] = new MessageElement(new QName("Name"), "Example.doc");
                    attachment[1] = new MessageElement(new PartnerService.Mes("Body"), inbuff);
                    attachment[2] = new MessageElement(new QName("ParentId"), id);

                    soattach[0].set_any(attachment);
                    soattach[0].setType("Attachment");

                    MessageElement[] attachment = new MessageElement[3];
                    Attachment attachment = new Attachment();
                    attachment.Body = inbuff;
                    attachment.Name = String.IsNullOrEmpty(docName) ? "Document_" + DateTime.Now.ToString("yyyyMMddHHmmss") : docName;
                    attachment.IsPrivate = false;
                    attachment.ParentId = id;

                    sObject[] document = new sObject[] { attachment };

                    PartnerService.SaveResult[] apiResponse =  sfPartnerBinding.create(document);

                    if (apiResponse == null || apiResponse.Length == 0) {
                        result.Uploaded = false;
                        result.Message = "Document could not be attached.";
                    } else
                    {
                        result.Uploaded = apiResponse[0].success;
                        if (apiResponse[0].errors != null)
                        {
                            result.Message = string.Empty;
                            foreach (PartnerService.Error err in apiResponse[0].errors)
                            {
                                result.Message += err.message + "\r\n";
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                result.Uploaded = false;
                result.Message = ex.Message;
            }

            yield return result;
        }
        */
        #endregion

        #region Upsert Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_SaveResults", TableDefinition = "ObjectType nvarchar(100), Action nvarchar(50), Id nvarchar(100), ExternalId nvarchar(100), Success bit, Message nvarchar(max)")]
        public static IEnumerable sfUpsert(SqlXml data)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            return _sfUpsert(data, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
        }

        [SqlFunction(FillRowMethodName = "Fill_SaveResults", TableDefinition = "ObjectType nvarchar(100), Action nvarchar(50), Id nvarchar(100), ExternalId nvarchar(100), Success bit, Message nvarchar(max)")]
        private static IEnumerable _sfUpsert(SqlXml data, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(data.Value);

            int batchSize = 100;

            XmlAttribute xa = xd.DocumentElement.Attributes["BatchSize"];
            if (xa != null)
            {
                batchSize = Convert.ToInt32(xa.Value);
            }
        
            return processUpsert(batchSize, null, xd, sfPartnerBinding, sfLoginResult);

        }

        [SqlFunction(FillRowMethodName = "Fill_SaveResults", TableDefinition = "ObjectType nvarchar(100), Action nvarchar(50), Id nvarchar(100), ExternalId nvarchar(100), Success bit, Message nvarchar(max)")]
        private static IEnumerable processUpsert(int batchSize, string parentId, XmlDocument xd, SforceService sfPartnerBinding, LoginResult sfLoginResult)
        {
            MetadataService mtDataService = new MetadataService();
            Hashtable htObjectTypes = new Hashtable();
            Hashtable lookupResults = new Hashtable();

            Metadata[] metaResults = null;
            Hashtable htRules = new Hashtable();
            XmlNodeList xnlValidationRules = xd.SelectNodes("/*/DisableValidationRule");
            if (xnlValidationRules != null && xnlValidationRules.Count !=0 && sfLoginResult != null)
            {
                mtDataService = CreateMetaDataConnection(sfLoginResult);
                List<string> customObjects = new List<string>();
                foreach (XmlNode xnRule in xnlValidationRules)
                {
                    if (!customObjects.Contains(xnRule.Attributes["Object"].Value))
                    {
                        customObjects.Add(xnRule.Attributes["Object"].Value);
                    }

                    ValidationRule newRule = new ValidationRule();
                    newRule.ObjectName = xnRule.Attributes["Object"].Value;
                    newRule.RuleName = xnRule.Attributes["RuleName"].Value;
                    newRule.Active = false;

                    htRules.Add(newRule.ObjectName + "." + newRule.RuleName, newRule);

                }

                metaResults = mtDataService.readMetadata("CustomObject", customObjects.ToArray());
                foreach (Metadata obj in metaResults)
                {

                    CustomObject co = (CustomObject)obj;
                    foreach (MetaService.ValidationRule rule in co.validationRules)
                    {
                        string key = co.fullName + "." + rule.fullName;
                        if (rule.active == false && htRules.ContainsKey(key))
                        {
                            htRules.Remove(key);
                            SaveResults result = new SaveResults();
                            result.ObjectType = co.fullName;
                            result.Action = "Disable Validation Rule";
                            result.Id = rule.fullName;
                            result.ExternalId = "";
                            result.Success = false;
                            result.Message = "Validation Rule is already disabled";

                            yield return result;
                        }
                        else if (rule.active == true && htRules.ContainsKey(key))
                        {
                            rule.active = false;
                        }
                    }
                }

                if (htRules.Count > 0)
                {
                    MetaService.SaveResult[] mdUpdateResults = mtDataService.updateMetadata(metaResults);
                    foreach (MetaService.SaveResult mdUpdateResult in mdUpdateResults)
                    {
                        if (htRules.ContainsKey(mdUpdateResult.fullName))
                        {
                            SaveResults result = new SaveResults();
                            result.ObjectType = mdUpdateResult.fullName;
                            result.Action = "Disable Validation Rule";
                            result.Id = mdUpdateResult.fullName;
                            result.ExternalId = "";
                            result.Success = mdUpdateResult.success;
                            result.Message = mdUpdateResult.success ? "" : mdUpdateResult.errors.ToString();

                            yield return result;
                        }
                    }
                } else
                {
                    xnlValidationRules = null;
                }
            }


            XmlNodeList xnlObjects = xd.SelectNodes("/*/*[not(name()='DisableValidationRule')]");
            if (xnlObjects != null)
            {
                foreach (XmlNode xn in xnlObjects)
                {
                    htObjectTypes[xn.LocalName] = xn.LocalName;
                }
            }

            foreach (object salesforceObject in htObjectTypes.Keys)
            {
                string objectName = salesforceObject.ToString();
                XmlNodeList xnlData = xd.SelectNodes("/*/" + objectName);
                if (xnlData == null)
                {
                    SaveResults result = new SaveResults();
                    result.ObjectType = objectName;
                    result.Action = "None";
                    result.Id = "";
                    result.ExternalId = "";
                    result.Success = false;
                    result.Message = "Object type not defined.";

                    yield return result;
                    break;
                }

                DescribeSObjectResult sObjectDefinition = sfPartnerBinding.describeSObject(objectName);
                SortedList<string, Field> fieldList = new SortedList<string, Field>();

                for (int i = 0; i < sObjectDefinition.fields.Length; i++)
                {
                    if (sObjectDefinition.fields[i].updateable || sObjectDefinition.fields[i].createable || sObjectDefinition.fields[i].name=="Id") {
                        fieldList.Add(sObjectDefinition.fields[i].name, sObjectDefinition.fields[i]);
                    }
                }

                Type objType = sObjectDefinition.GetType();
                PropertyInfo[] propsObject = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                sObject[] objs = new sObject[batchSize];
                XmlNode[] nodes = new XmlNode[batchSize];
                int x = 0;
                foreach (XmlNode xn in xnlData)
                {

                    sObject obj = new sObject();
                    obj.type = objectName;
                    objs[x] = obj;

                    nodes[x] = xn;

                    XmlDocument doc = new XmlDocument();
                    XmlElement[] elements = new XmlElement[xn.Attributes.Count];

                    int fieldCounter = 0;

                    foreach (XmlAttribute attr in xn.Attributes)
                    {
                        string attributeName = attr.LocalName;
                        string attributeValue = attr.Value;

                        if (attributeName == "ParentField" && !String.IsNullOrEmpty(parentId))
                        {
                            attributeName = attributeValue;
                            attributeValue = parentId;
                        }

                        if (fieldList.ContainsKey(attributeName))
                        {
                            Field fld = fieldList[attributeName];

                            elements[fieldCounter] = doc.CreateElement(attributeName);

                            if (fieldCounter == 0)
                            {
                                XmlAttribute attExternalId = doc.CreateAttribute("ExternalId");
                                attExternalId.Value = xn.Attributes["ExternalId"] != null ? xn.Attributes["ExternalId"].Value : "";
                                elements[fieldCounter].Attributes.Append(attExternalId);
                            }

                            if (attr.Value.ToLower() == "<null>")
                            {
                                elements[fieldCounter].InnerText = "";
                            }
                            else if (fld.soapType == soapType.xsdboolean && (attributeValue == "1" || attributeValue == "0"))
                            {
                                elements[fieldCounter].InnerText = attributeValue == "1" ? "true" : "false";
                            }
                            else
                            {
                                if ((fld.idLookup || fld.referenceTo != null) && fld.name != "Name" && attributeValue.ToLower().StartsWith("select"))
                                {
                                    string lookupResult = null;
                                    if (lookupResults.ContainsKey(attributeValue))
                                    {
                                        lookupResult = lookupResults[attributeValue].ToString();
                                    }
                                    else
                                    {
                                        QueryResult qr = sfPartnerBinding.query(attributeValue);
                                        if (qr.size == 1 && qr.records[0].Id != null)
                                        {
                                            lookupResult = qr.records[0].Id;
                                        }
                                        lookupResults.Add(attributeValue, lookupResult);
                                    }
                                    attributeValue = lookupResult;
                                }

                                elements[fieldCounter].InnerText = attributeValue;
                                
                            }

                            fieldCounter++;
                        }
                    }
                    obj.Any = elements;

                    x++;
                    if (x == batchSize)
                    {
                        foreach (SaveResults result in populateResults(sfPartnerBinding, objectName, objs, nodes, batchSize))
                        {
                            yield return result;
                        }
                        objs = new sObject[batchSize];
                        nodes = new XmlNode[batchSize];
                        x = 0;
                    }
                }
                // make sure to get the remaining sObjects
                if (x != 0)
                {
                    Array.Resize(ref objs, x);
                    Array.Resize(ref nodes, x);
                    foreach (SaveResults result in populateResults(sfPartnerBinding, objectName, objs, nodes, batchSize))
                    {
                        yield return result;
                    }
                }
            }

            if (xnlValidationRules != null && xnlValidationRules.Count != 0 && sfLoginResult != null)
            {
                foreach (Metadata obj in metaResults)
                {
                    CustomObject co = (CustomObject)obj;
                    foreach (MetaService.ValidationRule rule in co.validationRules)
                    {
                        string key = co.fullName + "." + rule.fullName;
                        if (rule.active == false && htRules.ContainsKey(key))
                        {
                            rule.active = true;
                        }
                    }
                }

                MetaService.SaveResult[] mdUpdateResults = mtDataService.updateMetadata(metaResults);
                foreach (MetaService.SaveResult mdUpdateResult in mdUpdateResults)
                {
                    if (htRules.ContainsKey(mdUpdateResult.fullName))
                    {
                        SaveResults result = new SaveResults();
                        result.ObjectType = mdUpdateResult.fullName;
                        result.Action = "Enable Validation Rule";
                        result.Id = mdUpdateResult.fullName;
                        result.ExternalId = "";
                        result.Success = mdUpdateResult.success;
                        result.Message = mdUpdateResult.success ? "" : mdUpdateResult.errors.ToString();

                        yield return result;
                    }
                }
            }

        }

        [SqlFunction(FillRowMethodName = "Fill_SaveResults", TableDefinition = "ObjectType nvarchar(100), Action nvarchar(50), Id nvarchar(100), ExternalId nvarchar(100), Success bit, Message nvarchar(max)")]
        private static IEnumerable populateResults(SforceService sfPartnerBinding, string objectName, sObject[] objects, XmlNode[] nodes, int batchSize)
        {
            PartnerService.UpsertResult[] responses = sfPartnerBinding.upsert("Id", objects);

            int resultPosition = 0;
            foreach (PartnerService.UpsertResult ur in responses)
            {
                SaveResults result = new SaveResults();
                result.ObjectType = objectName;
                if (ur.created)
                    result.Action = "Created";
                else
                    result.Action = "Update";
                result.Id = ur.id;
                string externalId = "";
                if (objects[resultPosition].Any[0].Attributes["ExternalId"] != null) {
                    externalId = objects[resultPosition].Any[0].Attributes["ExternalId"].Value;
                };
                result.ExternalId = String.IsNullOrEmpty(externalId) ? ur.id : externalId;

                result.Success = ur.success;

                if (ur.errors != null)
                {
                    result.Message = string.Empty;
                    foreach (PartnerService.Error err in ur.errors)
                    {
                        result.Message += err.message + "\r\n";
                    }
                }

                //string parentField = objects[resultPosition].Any[0].Attributes[parentField] != null ? objects[resultPosition].Any[0].Attributes["ParentField"].Value : "";
                if (ur.success && nodes[0].SelectNodes("/*/*") != null) {
                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(nodes[resultPosition].OuterXml);
                    foreach (SaveResults saveResult in processUpsert(batchSize, ur.id, xd, sfPartnerBinding, null))
                    {
                        yield return saveResult;
                    }
                }

                //results.Add(result);

                resultPosition++;
                yield return result;
            }
        }

        #endregion

        #region Delete Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_SaveResults", TableDefinition = "ObjectType nvarchar(100), Action nvarchar(50), Id nvarchar(100), ExternalId nvarchar(100), Success bit, Message nvarchar(max)")]
        public static IEnumerable sfDelete(SqlXml data)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);

            return _sfDelete(data, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
        }

        [SqlFunction(FillRowMethodName = "Fill_SaveResults", TableDefinition = "ObjectType nvarchar(100), Action nvarchar(50), Id nvarchar(100), ExternalId nvarchar(100), Success bit, Message nvarchar(max)")]
        private static IEnumerable _sfDelete(SqlXml data, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            int batchSize = 1000;

            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(data.Value);

            XmlAttribute xa = xd.DocumentElement.Attributes["BatchSize"];
            if (xa != null)
            {
                batchSize = System.Convert.ToInt32(xa.Value);
            }

            Hashtable htObjectTypes = new Hashtable();
            XmlNodeList xnlObjects = xd.SelectNodes("/*/*[not(name()='DisableValidationRule')]");
            if (xnlObjects != null)
            {
                foreach (XmlNode xn in xnlObjects)
                {
                    htObjectTypes[xn.LocalName] = xn.LocalName;
                }
            }

            foreach (object salesforceObject in htObjectTypes.Keys)
            {
                string objectName = salesforceObject.ToString();
                XmlNodeList xnlData = xd.SelectNodes("/*/" + objectName);

                string[] ids = new string[batchSize];
                int x = 0;
                foreach (XmlNode xn in xnlData)
                {
                    ids[x] = (xn.Attributes["Id"] != null) ? xn.Attributes["Id"].Value : "";
                    x++;
                    if (x == batchSize)
                    {
                        PartnerService.DeleteResult[] responses = sfPartnerBinding.delete(ids);

                        foreach (PartnerService.DeleteResult ur in responses)
                        {
                            SaveResults result = new SaveResults();
                            result.ObjectType = objectName;
                            result.Action = "Deleted";
                            result.Id = ur.id;
                            result.Success = ur.success;

                            if (ur.errors != null)
                            {
                                result.Message = string.Empty;
                                foreach (PartnerService.Error err in ur.errors)
                                {
                                    result.Message += err.message + "\r\n";
                                }
                            }

                            yield return result;

                        }

                        ids = new string[batchSize];
                        x = 0;
                    }
                }
                // make sure to get the remaining sObjects
                if (x != 0)
                {
                    Array.Resize(ref ids, x);
                    PartnerService.DeleteResult[] responses = sfPartnerBinding.delete(ids);
                    foreach (PartnerService.DeleteResult ur in responses)
                    {
                        SaveResults result = new SaveResults();
                        result.ObjectType = objectName;
                        result.Action = "Deleted";
                        result.Id = ur.id;
                        result.Success = ur.success;

                        if (ur.errors != null)
                        {
                            result.Message = string.Empty;
                            foreach (PartnerService.Error err in ur.errors)
                            {
                                result.Message += err.message + "\r\n";
                            }
                        }
                        yield return result;
                    }
                }

            }
        }

        #endregion

        #region GetData Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_GenericResults", TableDefinition = "ObjectType nvarchar(80), Id nvarchar(18), Name nvarchar(80), CreatedById nvarchar(18), CreatedDate DateTime, LastModifiedById nvarchar(18), LastModifiedDate DateTime, LastActivityDate DateTime, SystemModstamp datetime, Data xml")]
        public static IEnumerable sfGetData(string objectName, string filter)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            return _sfGetData(objectName, filter, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
        }

        [SqlFunction(FillRowMethodName = "Fill_GenericResults", TableDefinition = "ObjectType nvarchar(80), Id nvarchar(18), Name nvarchar(80), CreatedById nvarchar(18), CreatedDate DateTime, LastModifiedById nvarchar(18), LastModifiedDate DateTime, LastActivityDate DateTime, SystemModstamp datetime, Data xml")]
        private static IEnumerable _sfGetData(string objectName, string filter, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            DescribeSObjectResult sObjectDefinition = sfPartnerBinding.describeSObject(objectName);
            SortedList<string, Field> fieldList = new SortedList<string, Field>();

            string sql = "";
            string fields = "";

            if (filter.ToLower().StartsWith("select "))
            {
                sql = filter;

                Match match = Regex.Match(filter, "select(.*)from", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string parsedFields = "," + match.Groups[1].Value.Replace(" ","") + ",";
                    for (int i = 0; i < sObjectDefinition.fields.Length; i++)
                    {
                        if (parsedFields.Contains("," + sObjectDefinition.fields[i].name +",")) { 
                            fields += "," + sObjectDefinition.fields[i].name;
                            fieldList.Add(sObjectDefinition.fields[i].name, sObjectDefinition.fields[i]);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < sObjectDefinition.fields.Length; i++)
                {
                    fields += "," + sObjectDefinition.fields[i].name;
                    fieldList.Add(sObjectDefinition.fields[i].name, sObjectDefinition.fields[i]);
                }

                fields = fields.Substring(1);

                sql = "Select " + fields + " From " + objectName;
                if (!string.IsNullOrEmpty(filter))
                {
                    int limit = 0;
                    if (Int32.TryParse(filter, out limit))
                    {
                        sql += " Limit " + filter;
                    }
                    else
                    {
                        sql += " Where " + filter;
                    }
                }
            }

            QueryResult qr = sfPartnerBinding.query(sql);
            if (qr.size == 0) yield break;

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;

            bool getMore = true;
            while (getMore)
            {
                for (int i = 0; i < qr.records.Length; i++)
                {
                    sObject sfObject = qr.records[i];
                    GenericResults so = new GenericResults();
                    so.ObjectType = objectName;

                    using (MemoryStream msObject = new MemoryStream())
                    {
                        XmlWriter xw = XmlWriter.Create(msObject, xws);
                        xw.WriteStartElement(objectName);


                        XmlElement[] elementList = sfObject.Any;

                        foreach (XmlElement xe in elementList)
                        {
                            if (!fieldList.ContainsKey(xe.LocalName))
                            {
                                continue;
                            }

                            string value = xe.InnerText;
                            if (value != "")
                            {
                                // hack to fix scientific notation issue in salesforce
                                Field fld = fieldList[xe.LocalName];
                                if (fld.soapType == soapType.xsddouble && value.Contains("E"))
                                {
                                    xw.WriteAttributeString(xe.LocalName, Decimal.Parse(value, NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint).ToString());
                                }
                                else
                                    xw.WriteAttributeString(xe.LocalName, value);
                            }
                            switch (xe.LocalName)
                            {
                                case "Id":
                                    so.Id = sfObject.Id;
                                    break;
                                case "Name":
                                    so.Name = value;
                                    break;
                                case "IsDeleted":
                                    break;
                                case "CreatedById":
                                    so.CreatedById = value;
                                    break;
                                case "CreatedDate":
                                    if (value != "") so.CreatedDate = System.Convert.ToDateTime(value);
                                    break;
                                case "LastModifiedById":
                                    so.LastModifiedById = value;
                                    break;
                                case "LastModifiedDate":
                                    if (value != "") so.LastModifiedDate = System.Convert.ToDateTime(value);
                                    break;
                                case "LastActivityDate":
                                    if (value != "") so.LastActivityDate = System.Convert.ToDateTime(value);
                                    break;
                                case "SystemModstamp":
                                    if (value != "") so.SystemModstamp = System.Convert.ToDateTime(value);
                                    break;
                            }

                        }
                        xw.WriteEndElement();
                        xw.Flush();

                        so.Data = new SqlXml(msObject);

                        yield return so;
                    }
                }
                //x = qr.records.Length;
                getMore = (!qr.done);
                if (getMore)
                    qr = sfPartnerBinding.queryMore(qr.queryLocator);
            }
        }

        #endregion

        #region GetXml Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read)]
        public static SqlXml sfGetXml(string objectName, string filter)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);

            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            DescribeSObjectResult sObjectDefinition = sfPartnerBinding.describeSObject(objectName);
            SortedList<string, Field> fieldList = new SortedList<string, Field>();

            string sql = "";
            if (filter.ToLower().StartsWith("select "))
            {
                sql = filter;
            }
            else
            {
                string fields = "";
                for (int i = 0; i < sObjectDefinition.fields.Length; i++)
                {
                    fields += "," + sObjectDefinition.fields[i].name;
                    fieldList.Add(sObjectDefinition.fields[i].name, sObjectDefinition.fields[i]);
                }

                fields = fields.Substring(1);

                sql = "Select " + fields + " From " + objectName;
                if (!string.IsNullOrEmpty(filter))
                {
                    int limit = 0;
                    if (Int32.TryParse(filter, out limit))
                    {
                        sql += " Limit " + filter;
                    }
                    else
                    {
                        sql += " Where " + filter;
                    }
                }
            }

            QueryResult qr = sfPartnerBinding.query(sql);
            if (qr.size == 0) return null;

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;


            MemoryStream msObject = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(msObject, xws);
            xw.WriteStartElement("Data");
            bool getMore = true;
            while (getMore)
            {
                for (int i = 0; i < qr.records.Length; i++)
                {
                    sObject sfObject = qr.records[i];
                    xw.WriteStartElement(objectName);
                    XmlElement[] elementList = sfObject.Any;
                    foreach (XmlElement xe in elementList)
                    {
                        string value = xe.InnerText;
                        if (value != "")
                        {
                            // hack to fix scientific notation issue in salesforce
                            Field fld = fieldList[xe.LocalName];
                            if (fld.soapType == soapType.xsddouble)
                            {
                                xw.WriteAttributeString(xe.LocalName, Double.Parse(value).ToString());
                            }
                            else
                                xw.WriteAttributeString(xe.LocalName, value);
                        }
                    }
                    xw.WriteEndElement();
                    xw.Flush();
                }
                //x = qr.records.Length;
                getMore = (!qr.done);
                if (getMore)
                    qr = sfPartnerBinding.queryMore(qr.queryLocator);
            }
            xw.WriteEndElement();
            xw.Flush();
            return new SqlXml(msObject);
        }

        #endregion

        #region Schema Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_SchemaResults", TableDefinition = "Source nvarchar(max)")]
        public static IEnumerable sfCreateUDF(string objectName)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            return _sfCreateUDF(objectName, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

        }

        [SqlFunction(FillRowMethodName = "Fill_SchemaResults", TableDefinition = "Source nvarchar(max)")]
        private static IEnumerable _sfCreateUDF(string objectName, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            DescribeSObjectResult sObjectResult = sfPartnerBinding.describeSObject(objectName);
            SortedList<string, Field> fieldList = new SortedList<string, Field>();
            for (int i = 0; i < sObjectResult.fields.Length; i++)
            {
                Field fld = sObjectResult.fields[i];
                fieldList.Add(fld.name, fld);
            }

            String displayName = "sf" + objectName;
            /*if (Regex.Matches(Regex.Escape(objectName), "__").Count > 1 && sObjectResult.custom)
            {
                displayName = Regex.Matches(Regex.Escape(objectName), "(.*?)_")[0].Groups[0]  + sObjectResult.label.Replace(" ", ""); 
            }
            */
            yield return new SchemaResults { Source = "Create Function [dbo].[" + displayName + "](@Criteria nvarchar(max)) " };
            yield return new SchemaResults { Source = "Returns @Results Table (" };
            yield return new SchemaResults { Source = "\t\tId varchar(18) Collate SQL_Latin1_General_CP1_CS_AS Not Null" };
            if (fieldList.ContainsKey("Name") || fieldList.ContainsKey("name")) yield return new SchemaResults { Source = "\t\t,Name varchar(80)" };

            foreach (string key in fieldList.Keys)
            {
                Field fld = fieldList[key];
                if (fld.name != "Id" && fld.name != "Name")
                {
                    string dataType = string.Empty;
                    dataType = GetSqlDataType(fld, true);
                    SchemaResults sr = new SchemaResults();
                    sr.Source = string.Format("\t\t,[{0}] {1} -- {2}", fld.name, dataType, fld.label);
                    yield return sr;
                }
            }
            yield return new SchemaResults { Source = ")" };
            yield return new SchemaResults { Source = "As" };
            yield return new SchemaResults { Source = "Begin" };
            yield return new SchemaResults { Source = "" };
            yield return new SchemaResults { Source = "" };
            yield return new SchemaResults { Source = "\tInsert\t@Results" };
            yield return new SchemaResults { Source = "\tSelect\tRowset.Col.value('@Id','varchar(18)') as Id" };

            if (fieldList.ContainsKey("Name") || fieldList.ContainsKey("name"))
            {
                yield return new SchemaResults { Source = "\t\t\t,Rowset.Col.value('@Name','varchar(80)') as Name" };
            }


            foreach (string key in fieldList.Keys)
            {
                Field fld = fieldList[key];
                if (fld.name != "Id" && fld.name != "Name")
                {
                    string dataType = GetSqlDataType(fld, false);
                    SchemaResults sr = new SchemaResults();
                    sr.Source = string.Format("\t\t\t,Rowset.Col.value('@{0}','{1}') as [{0}]", fld.name, dataType);
                    yield return sr;
                }
            }

            yield return new SchemaResults { Source = "\tFrom\tdbo.sfGetData('" + objectName + "', @Criteria)" };
            yield return new SchemaResults { Source = "\t\t\tCross Apply Data.nodes('" + objectName + "') As Rowset(Col)" };
            yield return new SchemaResults { Source = "" };
            yield return new SchemaResults { Source = "\tReturn" };
            yield return new SchemaResults { Source = "End" };
        }

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_ObjectDescribeResults", TableDefinition = "Name nvarchar(80), Activateable bit, Createable bit, Custom bit, CustomSetting bit, Deletable bit, DeprecatedAndHidden bit, FeedEnabled bit, KeyPrefix nvarchar(80), Label  nvarchar(80), LabelPlural  nvarchar(80), Layoutable bit, Mergeable bit, Queryable bit, Replicateable bit, Retrieveable bit, Searchable bit, Triggerable bit, Undeletable bit, Updateable bit")]
        public static IEnumerable sfGetObjectList()
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            return _sfGetObjectList(salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

        }

        [SqlFunction(FillRowMethodName = "Fill_ObjectDescribeResults", TableDefinition = "Name nvarchar(80), Activateable bit, Createable bit, Custom bit, CustomSetting bit, Deletable bit, DeprecatedAndHidden bit, FeedEnabled bit, KeyPrefix nvarchar(80), Label  nvarchar(80), LabelPlural  nvarchar(80), Layoutable bit, Mergeable bit, Queryable bit, Replicateable bit, Retrieveable bit, Searchable bit, Triggerable bit, Undeletable bit, Updateable bit")]
        private static IEnumerable _sfGetObjectList(string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            DescribeGlobalResult sObjectsResult = sfPartnerBinding.describeGlobal();
            foreach (DescribeGlobalSObjectResult obj in sObjectsResult.sobjects)
            {
                if (obj.custom || obj.queryable)
                    yield return new ObjectDescribeResults { Name = obj.name,
                        Activateable = obj.activateable,
                        Createable = obj.createable,
                        Custom = obj.custom,
                        CustomSetting = obj.customSetting,
                        Deletable = obj.deletable,
                        DeprecatedAndHidden = obj.deprecatedAndHidden,
                        FeedEnabled = obj.feedEnabled,
                        KeyPrefix = obj.keyPrefix,
                        Label = obj.label,
                        LabelPlural = obj.labelPlural,
                        Layoutable = obj.layoutable,
                        Mergeable = obj.mergeable,
                        Queryable = obj.queryable,
                        Replicateable = obj.replicateable,
                        Retrieveable = obj.retrieveable,
                        Searchable = obj.searchable,
                        Triggerable = obj.triggerable,
                        Undeletable = obj.undeletable,
                        Updateable = obj.updateable };

            }

        }

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_Relationships", TableDefinition = "ParentObject nvarchar(100), ChildObject nvarchar(100), ChildField nvarchar(100), RelationshipName nvarchar(100), CascadeDelete bit, RestrictedDelete bit, IsMasterDetail bit")]
        public static IEnumerable sfGetRelationships(string objectName)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            return _sfGetRelationships(objectName, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

        }

        [SqlFunction(FillRowMethodName = "Fill_Relationships", TableDefinition = "ParentObject nvarchar(100), ChildObject nvarchar(100), ChildField nvarchar(100), RelationshipName nvarchar(100), CascadeDelete bit, RestrictedDelete bit, IsMasterDetail bit")]
        private static IEnumerable _sfGetRelationships(string objectName, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            DescribeSObjectResult sObjectResult = null;

            SortedSet<string> processedRelationships = new SortedSet<string>();
            SortedSet<string> processedPairs = new SortedSet<string>();

            sObjectResult = sfPartnerBinding.describeSObject(objectName);

            if (sObjectResult.childRelationships != null )
            {
                foreach (ChildRelationship cr in sObjectResult.childRelationships)
                {
                        switch (cr.childSObject)
                        {
                            case "ActivityHistory":
                            case "AttachedContentDocument":
                            case "AttachedContentNote":
                            case "Attachment":
                            case "CollaborationGroupRecord":
                            case "CombinedAttachment":
                            case "ContentDistribution":
                            case "ContentDocumentLink":
                            case "ContentVersion":
                            case "DuplicateRecordItem":
                            case "EmailMessage":
                            case "EntitySubscription":
                            case "Event":
                            case "FeedComment":
                            case "FeedItem":
                            case "Note":
                            case "NoteAndAttachment":
                            case "OpenActivity":
                            case "OutgoingEmail":
                            case "ProcessInstance":
                            case "ProcessInstanceHistory":
                            case "Task":
                            case "TopicAssignment":
                                break;
                            default:
                                DescribeSObjectResult sChildObject = sfPartnerBinding.describeSObject(cr.childSObject);
                                SortedList<string, Field> childFieldList = new SortedList<string, Field>();
                                for (int i = 0; i < sChildObject.fields.Length; i++)
                                {
                                    Field fld = sChildObject.fields[i];
                                    childFieldList.Add(fld.name, fld);
                                }

                                Field cf;
                                childFieldList.TryGetValue(cr.field, out cf);

                                yield return new Relationships
                                {
                                    ParentObject = objectName,
                                    ChildObject = cr.childSObject,
                                    ChildField = cr.field,
                                    RelationshipName = cr.relationshipName,
                                    CascadeDelete = cr.cascadeDelete,
                                    RestrictedDelete = cr.restrictedDelete,
                                    IsMasterDetail = cf.relationshipOrder == 1
                                };
                                processedRelationships.Add(cr.relationshipName);
                             
                                break;
                        }
                }
            }
        }

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_SchemaResults", TableDefinition = "Source nvarchar(max)")]
        public static IEnumerable sfGetColumns(string objectName)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            return _sfGetColumns(objectName, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);

        }

        [SqlFunction(FillRowMethodName = "Fill_SchemaResults", TableDefinition = "Source nvarchar(max)")]
        private static IEnumerable _sfGetColumns(string objectName, string salesForceUserID, string salesForcePassword, string salesForceToken, string serviceURL)
        {
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            DescribeSObjectResult sObjectResult = sfPartnerBinding.describeSObject(objectName);
            SortedList<string, Field> fieldList = new SortedList<string, Field>();
            for (int i = 0; i < sObjectResult.fields.Length; i++)
            {
                SchemaResults sr = new SchemaResults();
                sr.Source = sObjectResult.fields[i].name;
                yield return sr;
            }
        }

        private static string GetSqlDataType(Field fld, Boolean includeCollation)
        {
            string dataType = string.Empty;
            switch (fld.soapType)
            {
                case soapType.tnsID:
                    dataType = "varchar(18)";
                    if (includeCollation) dataType += " Collate SQL_Latin1_General_CP1_CS_AS";
                    break;
                case soapType.xsdanyType:
                    dataType = "nvarchar(max)";
                    break;
                case soapType.xsdbase64Binary:
                    dataType = "varbinary(max)";
                    break;
                case soapType.xsdboolean:
                    dataType = "bit";
                    break;
                case soapType.xsddate:
                    dataType = "date";
                    break;
                case soapType.xsddateTime:
                    dataType = "datetime";
                    break;
                case soapType.xsddouble:
                    dataType = string.Format("decimal({0},{1})", fld.precision, fld.scale);
                    break;
                case soapType.xsdint:
                    dataType = "int";
                    break;
                case soapType.xsdstring:
                    if (fld.length > 4000)
                        dataType = "nvarchar(max)";
                    else
                        dataType = string.Format("nvarchar({0})", fld.length);
                    break;
                case soapType.xsdtime:
                    dataType = "time";
                    break;
                default:
                    dataType = "nvarchar(max)";
                    break;
            }
            return dataType;
        }

        #endregion  

        #region Replication Methods

        /* [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read, FillRowMethodName = "Fill_ReplicationResults", TableDefinition = "ObjecName nvarchar(80), LastModifiedDate datetime, SystemModstamp datetime, ObjectCount int, ChangeCount int, SchemaChanges xml, Changes xml")] */
        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read)]
        public static SqlXml sfReplicate(string batchId, SqlBoolean useTempProcedures)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string sql = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);

            try
            {
                Hashtable htObjects = new Hashtable();
                LoginResult sfLoginResult = null;
                SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
                MemoryStream ms = new MemoryStream();
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.OmitXmlDeclaration = true;

                XmlWriter xw = XmlWriter.Create(ms, xws);
                xw.WriteStartDocument();
                xw.WriteStartElement("Replication");

                using (SqlConnection cn = new SqlConnection("context connection=true;"))
                {
                    cn.Open();
                    sql = String.Format("Select ObjectName, BatchSize, Resync, LastId, SystemModStamp, ReplicateDeletes From SalesForceReplObjects Where BatchId = '{0}' Order By ObjectName", batchId);
                    SqlCommand cmd = new SqlCommand(sql, cn);

                    using (var rdrObject = cmd.ExecuteReader())
                    {
                        while (rdrObject.Read())
                        {
                            Hashtable htObject = new Hashtable();
                            htObject.Add("ObjectName", rdrObject.GetString(0));
                            htObject.Add("batchSize", rdrObject.GetInt32(1));
                            htObject.Add("resync", rdrObject.GetBoolean(2));
                            htObject.Add("lastId", !rdrObject.IsDBNull(3) ? rdrObject.GetString(3) : "");
                            htObject.Add("systemModStamp", !rdrObject.IsDBNull(4) ? rdrObject.GetDateTime(4) : new DateTime(1900, 1, 1));
                            htObject.Add("replicateDeletes", rdrObject.GetBoolean(5));
                            htObjects.Add(rdrObject.GetString(0), htObject);
                        }
                    }
                    cn.Close();


                    foreach (object obj in htObjects.Values)
                    {
                        Hashtable ht = (Hashtable)obj;

                        string objectName = (string)ht["ObjectName"];
                        int batchsize = (int)ht["batchSize"];
                        bool resync = (bool)ht["resync"];
                        bool replicateDeletes = (bool)ht["replicateDeletes"];
                        DateTime systemModStampDate = (DateTime)ht["systemModStamp"];
                        string lastId = (string)ht["lastId"];


                        string filter = string.Empty;
                        string tsColumn = objectName.EndsWith("_History") ? "CreatedDate" : "SystemModStamp";
                        if (resync)
                        {
                            filter = string.Format("{2} > {0} ORDER BY {2} ASC, Id ASC LIMIT {1} ", systemModStampDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), batchsize, tsColumn);
                        }
                        else {
                            if (batchsize != 0)
                                filter = string.Format("({3} = {0} and Id > '{1}') or ({3} > {0}) ORDER BY {3} ASC, Id ASC LIMIT {2} ", systemModStampDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), lastId, batchsize, tsColumn);
                            else
                                filter = string.Format("({2} = {0} and Id > '{1}') or ({3} > {0}) ORDER BY {2} ASC, Id ASC", systemModStampDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"), lastId, tsColumn);
                        }
                        DescribeSObjectResult sObjectDefinition = sfPartnerBinding.describeSObject(objectName);
                        SortedList<string, Field> fieldList = new SortedList<string, Field>();
                        for (int i = 0; i < sObjectDefinition.fields.Length; i++)
                        {
                            Field fld = sObjectDefinition.fields[i];
                            fieldList.Add(fld.name, fld);
                        }

                        xw.WriteStartElement("ChangeSet");
                        {
                            xw.WriteAttributeString("ObjectName", objectName);
                            XmlDocument xdChanges = GetReplicationChanges(sfPartnerBinding, sObjectDefinition, objectName, filter);
                            if (xdChanges != null)
                            {
                                xw.WriteAttributeString("ChangeCount", xdChanges.SelectNodes("/Data/*").Count.ToString());
                                xw.WriteNode(xdChanges.CreateNavigator(), false);
                            }
                            else
                            {
                                xw.WriteAttributeString("ChangeCount", "0");
                                xw.WriteStartElement("Data");
                                xw.WriteEndElement();
                            }

                            xw.WriteStartElement("SchemaChanges");
                            GetTableSchema(xw, fieldList, objectName, replicateDeletes);

                            if (replicateDeletes)
                            {
                                GetTableSchema(xw, fieldList, objectName + "_Deleted", false);
                            }
                            xw.WriteEndElement();
                            CreateTempProcedure(xw, fieldList, objectName, replicateDeletes, (bool)useTempProcedures);
                        }
                        xw.WriteEndElement();
                        xw.Flush();
                    }

                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Flush();

                    return new SqlXml(ms);
                }
            }
            catch (Exception ex)
            {
                MemoryStream msError = new MemoryStream();
                XmlWriterSettings xwsError = new XmlWriterSettings();
                xwsError.Indent = true;
                xwsError.OmitXmlDeclaration = true;

                XmlWriter xwError = XmlWriter.Create(msError, xwsError);
                xwError.WriteStartElement("Error");
                xwError.WriteElementString("Message", ex.Message);
                if (ex.InnerException != null)
                    xwError.WriteElementString("InnerException", ex.InnerException.Message);
                xwError.WriteEndElement();
                xwError.Flush();
                return new SqlXml(msError);
            }



        }

        private static XmlDocument GetReplicationChanges(SforceService sfPartnerBinding, DescribeSObjectResult sObjectDefinition, string objectName, string filter)
        {
            try
            {
                SortedList<string, Field> fieldList = new SortedList<string, Field>();
                XmlDocument xdResult = new XmlDocument();
                string sql = "";

                string fields = "";
                for (int i = 0; i < sObjectDefinition.fields.Length; i++)
                {
                    fields += "," + sObjectDefinition.fields[i].name;
                    fieldList.Add(sObjectDefinition.fields[i].name, sObjectDefinition.fields[i]);
                }
                fields = fields.Substring(1);
                sql = string.Format("Select {0} From {1} Where {2}", fields, objectName, filter);


                QueryResult qr = sfPartnerBinding.queryAll(sql);
                if (qr.size == 0) return null;

                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.OmitXmlDeclaration = true;


                using (MemoryStream msObject = new MemoryStream())
                //using (FileStream msObject = new FileStream(@"d:\test.xml", FileMode.Create, System.IO.FileAccess.Write))
                {
                    XmlWriter xw = XmlWriter.Create(msObject, xws);

                    xw.WriteStartElement("Data");
                    xw.WriteElementString("SOQL", sql);

                    bool getMore = true;
                    while (getMore)
                    {
                        for (int i = 0; i < qr.records.Length; i++)
                        {
                            sObject sfObject = qr.records[i];
                            xw.WriteStartElement(objectName);
                            XmlElement[] elementList = sfObject.Any;
                            foreach (XmlElement xe in elementList)
                            {
                                string value = xe.InnerText;
                                if (value != "")
                                {
                                    // hack to fix scientific notation issue in salesforce
                                    Field fld = fieldList[xe.LocalName];
                                    if (fld.soapType == soapType.xsddouble && value.Contains("E"))
                                    {
                                        xw.WriteAttributeString(xe.LocalName, Decimal.Parse(value, NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint).ToString());
                                    }
                                    else
                                        xw.WriteAttributeString(xe.LocalName, value);
                                }
                            }
                            xw.WriteEndElement();
                            xw.Flush();
                        }
                        //x = qr.records.Length;
                        getMore = (!qr.done);
                        if (getMore)
                            qr = sfPartnerBinding.queryMore(qr.queryLocator);
                    }
                    xw.WriteEndElement();
                    xw.Flush();

                    msObject.Position = 0;
                    xdResult.Load(msObject);
                }
                return xdResult;
            }
            catch (Exception ex)
            {
                XmlDocument xdError = new XmlDocument();
                MemoryStream msError = new MemoryStream();
                XmlWriterSettings xwsError = new XmlWriterSettings();
                xwsError.Indent = true;
                xwsError.OmitXmlDeclaration = true;

                XmlWriter xwError = XmlWriter.Create(msError, xwsError);
                xwError.WriteStartElement("Error");
                xwError.WriteElementString("Message", ex.Message);
                if (ex.InnerException != null)
                    xwError.WriteElementString("InnerException", ex.InnerException.Message);
                xwError.WriteEndElement();
                xwError.Flush();
                msError.Position = 0;
                xdError.Load(msError);
                return xdError;
            }
        }

        private static void GetTableSchema(XmlWriter xmlWriter, SortedList<string, Field> fieldList, string objectName, bool replicateDeletes)
        {
            try
            {
                // Check to see if it exists first
                StringBuilder sb = new StringBuilder();
                SortedList<string, string> slResult = new SortedList<string, string>();

                string sql = string.Format("Select Name From sysColumns Where id = object_Id('{0}')", objectName);

                using (SqlConnection cn = new SqlConnection("context connection=true;"))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    using (var rdrFields = cmd.ExecuteReader())
                    {
                        while (rdrFields.Read())
                        {
                            slResult.Add(rdrFields.GetString(0), rdrFields.GetString(0));
                        }
                    }
                    cn.Close();

                    

                    if (slResult.Count == 0)
                    {
                        sb.AppendLine("Create Table [" + objectName + "](");
                        sb.AppendLine("\t\tId varchar(18) Collate SQL_Latin1_General_CP1_CS_AS Not Null");
                        slResult.Add("Id", "Id");
                        if (fieldList.ContainsKey("Name") || fieldList.ContainsKey("name"))
                        {
                            Field fld = fieldList["Name"];
                            sb.AppendLine(string.Format("\t\t,{0} nvarchar({1})", fld.name, fld.length));
                            slResult.Add("Name", "Name");
                        }
                        foreach (string key in fieldList.Keys)
                        {
                            Field fld = fieldList[key];
                            if (fld.name != "Id" && fld.name != "Name")
                            {
                                string dataType = GetSqlDataType(fld, true);
                                sb.AppendLine(string.Format("\t\t,[{0}] {1}", fld.name, dataType));
                            }
                        }
                        sb.AppendLine(")");

                    }
                    else
                    {
                        foreach (string fieldName in slResult.Keys)
                        {
                            if (!fieldList.ContainsKey(fieldName))
                            {
                                xmlWriter.WriteStartElement("Delete");
                                xmlWriter.WriteAttributeString("Name", fieldName);
                                xmlWriter.WriteEndElement();
                                sb.AppendLine(string.Format("Alter Table [{0}] Drop Column [{1}]", objectName, fieldName));
                            }

                        }

                        foreach (string fieldName in fieldList.Keys)
                        {
                            if (!slResult.ContainsKey(fieldName))
                            {
                                string dataType = GetSqlDataType(fieldList[fieldName], true);
                                xmlWriter.WriteStartElement("Add");
                                xmlWriter.WriteAttributeString("Name", fieldName);
                                xmlWriter.WriteAttributeString("DataType", dataType);
                                xmlWriter.WriteEndElement();
                                sb.AppendLine(string.Format("Alter Table [{0}] Add [{1}] {2}", objectName, fieldName, dataType));

                            }
                        }



                    }
                    if (sb.Length != 0)
                        xmlWriter.WriteElementString("Sql", sb.ToString());

                    xmlWriter.Flush();

                }
            }
            catch (Exception ex)
            {

                xmlWriter.WriteStartElement("Error");
                xmlWriter.WriteElementString("Message", ex.Message);
                if (ex.InnerException != null)
                    xmlWriter.WriteElementString("InnerException", ex.InnerException.Message);
                xmlWriter.WriteEndElement();
            }
        }

        private static void CreateTempProcedure(XmlWriter xmlWriter, SortedList<string, Field> fieldList, string objectName, bool replicateDeletes, bool useTempProcedures)
        {
            try
            {
                // Check to see if it exists first
                StringBuilder sb = new StringBuilder();
                StringBuilder sbInsertList = new StringBuilder();
                StringBuilder sbSelectList = new StringBuilder();
                string tsColumn = objectName.EndsWith("_History") ? "CreatedDate" : "SystemModStamp";

                sbInsertList.AppendLine("\t\t\tId");
                sbSelectList.AppendLine("\t\t\tRowset.Col.value('@Id','varchar(18)') as Id");
                foreach (string key in fieldList.Keys)
                {
                    Field fld = fieldList[key];
                    if (fld.name != "Id")
                    {
                        sbInsertList.AppendLine(string.Format("\t\t\t,[{0}]", fld.name));
                        sbSelectList.AppendLine(string.Format("\t\t\t,Rowset.Col.value('@{0}','{1}') as [{2}]", fld.name, GetSqlDataType(fld, false), fld.name));
                    }
                }

                sb.AppendLine("Create procedure " + (useTempProcedures ? "#" : "repl_") + "<ProcedureName> (@Xml Xml)");
                sb.AppendLine("As");
                sb.AppendLine("Set NoCount On");
                sb.AppendLine("");
                sb.AppendLine(string.Format("\tSelect "));
                sb.Append(sbSelectList);
                sb.AppendLine(string.Format("\tInto\t#tmp_{0}", objectName));
                sb.AppendLine(string.Format("\tFrom\t@Xml.nodes('/Data/{0}') As Rowset(Col)", objectName));
                sb.AppendLine("");
                sb.AppendLine(string.Format("\tDelete From [{0}] Where Id in (Select Id Collate SQL_Latin1_General_CP1_CS_AS From #tmp_{0})", objectName));
                sb.AppendLine("");
                sb.AppendLine(string.Format("\tInsert\t[{0}] (", objectName));
                sb.Append(sbInsertList);
                sb.AppendLine(string.Format("\t)"));
                sb.AppendLine(string.Format("\tSelect "));
                sb.Append(sbInsertList);
                sb.AppendLine(string.Format("\tFrom\t#tmp_{0}" , objectName));
                sb.AppendLine(string.Format("\tWhere\tIsDeleted=0"));

                if (replicateDeletes)
                {
                    sb.AppendLine(string.Format("\tDelete From [{0}_Deleted] Where Id in (Select Id Collate SQL_Latin1_General_CP1_CS_AS From #tmp_{0})", objectName));
                    sb.AppendLine("");
                    sb.AppendLine(string.Format("\tInsert [{0}_Deleted] (", objectName));
                    sb.Append(sbInsertList);
                    sb.AppendLine(string.Format("\t)"));
                    sb.AppendLine(string.Format("\tSelect "));
                    sb.Append(sbInsertList);
                    sb.AppendLine(string.Format("\tFrom\t#tmp_{0}",objectName));
                    sb.AppendLine(string.Format("\tWhere\t IsDeleted=1"));
                    sb.AppendLine("");
                }

                sb.AppendLine("");
                sb.AppendLine(string.Format("\tSelect Top 1 '{0}' ObjectName, Id, {1}, (Select Count(*) From [{0}]), (Select Count(*) From #tmp_{0}) From [{0}] Order By {1} Desc, Id Desc", objectName, tsColumn));
                xmlWriter.WriteElementString("Procedure", sb.ToString());
                xmlWriter.Flush();
            }
            catch (Exception ex)
            {

                xmlWriter.WriteStartElement("Error");
                xmlWriter.WriteElementString("Message", ex.Message);
                if (ex.InnerException != null)
                    xmlWriter.WriteElementString("InnerException", ex.InnerException.Message);
                xmlWriter.WriteEndElement();
            }
        }

        #endregion

        #region ValidationRule Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read)]
        public static SqlXml sfGetValidationRules(SqlString ObjectName)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);
            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = CreateSFConnection(ref sfLoginResult, salesForceUserID, salesForcePassword, salesForceToken, serviceURL);
            MetadataService mtDataService = CreateMetaDataConnection(sfLoginResult);


            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;
            MemoryStream msObject = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(msObject, xws);
            xw.WriteStartElement("ObjectName");

            Metadata[] results = mtDataService.readMetadata("CustomObject", new String[] { ObjectName.ToString() });

            if (results.Length != 1)
            {
                xw.WriteAttributeString("Error", "Object not found");
                xw.WriteEndElement();
                xw.Flush();
                return new SqlXml(msObject);
            }
            else
            {

                CustomObject co = null;
                try
                {
                    co = (CustomObject)results[0];
                }
                catch (Exception ex)
                {
                    xw.WriteAttributeString("Error", "Object is not a custom object");
                    xw.WriteEndElement();
                    xw.Flush();
                    return new SqlXml(msObject);
                }

                foreach (MetaService.ValidationRule rule in co.validationRules)
                {
                    xw.WriteStartElement("ValidationRule");
                    xw.WriteAttributeString("fullName", rule.fullName);
                    xw.WriteAttributeString("active", rule.active.ToString());
                    xw.WriteAttributeString("description", rule.description);
                    xw.WriteAttributeString("errorConditionFormula", rule.errorConditionFormula);
                    xw.WriteAttributeString("errorMessage", rule.errorMessage);
                    xw.WriteAttributeString("errorDisplayField", rule.errorDisplayField);
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.Flush();
                return new SqlXml(msObject);
            }

        }


        #endregion

        #region Meta Data Methods

        [SqlFunction(SystemDataAccess = SystemDataAccessKind.Read, DataAccess = DataAccessKind.Read)]
        public static SqlXml sfGetMetaData(SqlDecimal apiVersion)
        {
            string salesForceUserID = string.Empty;
            string salesForcePassword = string.Empty;
            string salesForceToken = string.Empty;
            string serviceURL = string.Empty;
            string dataGatewayServiceUrl = string.Empty;


            int spid = GetConnectionFromDatabase(ref salesForceUserID, ref salesForcePassword, ref salesForceToken, ref serviceURL, ref dataGatewayServiceUrl);

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            LoginResult sfLoginResult = null;
            SforceService sfPartnerBinding = new SforceService();
            salesForcePassword = DecryptPwd(salesForcePassword, salesForceToken).Value;

            // override the url for the service endpoint if it is passed. 
            if (!string.IsNullOrEmpty(serviceURL))
            {
                sfPartnerBinding.Url = serviceURL;
            }

            if (!string.IsNullOrEmpty(salesForceToken))
            {
                salesForcePassword += salesForceToken;
            }

            sfLoginResult = sfPartnerBinding.login(salesForceUserID, salesForcePassword);
            sfPartnerBinding.SessionHeaderValue = new PartnerService.SessionHeader();
            sfPartnerBinding.SessionHeaderValue.sessionId = sfLoginResult.sessionId;
            int idx1 = sfLoginResult.serverUrl.IndexOf(@"/services/");
            sfPartnerBinding.Url = sfLoginResult.serverUrl;

            Metadata mtData = new Metadata();
            MetadataService mtDataService = new MetadataService();
            mtDataService.SessionHeaderValue = new MetaService.SessionHeader();
            mtDataService.SessionHeaderValue.sessionId = sfLoginResult.sessionId;
            mtDataService.Url = sfLoginResult.metadataServerUrl;


            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;
            MemoryStream msObject = new MemoryStream();
            XmlWriter xw = XmlWriter.Create(msObject, xws);
            xw.WriteStartElement("MetaData");
            
            DescribeMetadataResult res = mtDataService.describeMetadata(apiVersion.ToDouble());

            Metadata[] results = mtDataService.readMetadata("CustomObject", new String[] { "rstk__socust__c" });

            CustomObject co = (CustomObject)results[0];

            if (res == null || res.metadataObjects.Length == 0) {
                xw.WriteEndElement();
                xw.Flush();
                return new SqlXml(msObject);
            }

            foreach(DescribeMetadataObject obj in res.metadataObjects)
            {
                xw.WriteStartElement(obj.xmlName);
                xw.WriteAttributeString("DirectoryName", obj.directoryName);
                xw.WriteAttributeString("Suffix", obj.suffix);
                xw.WriteAttributeString("InFolder", obj.inFolder.ToString());
                xw.WriteAttributeString("MetaFile", obj.metaFile.ToString());
                if (obj.childXmlNames != null && obj.childXmlNames.Length > 0)
                {
                    foreach (string child in obj.childXmlNames)
                    {
                        xw.WriteStartElement(child);
                        xw.WriteEndElement();
                    }
                }
                if (obj.xmlName.Equals("CustomObject"))
                {
                    ListMetadataQuery query = new ListMetadataQuery();
                    query.type = "CustomObject";
                    FileProperties[] lmr = mtDataService.listMetadata(new ListMetadataQuery[] { query }, apiVersion.ToDouble());
                    if (lmr != null)
                    {
                        foreach (FileProperties n in lmr)
                        {
                            xw.WriteStartElement("CustomObject");
                            xw.WriteAttributeString("Name", n.fullName);
                            xw.WriteAttributeString("FileName", n.fileName);

                            RetrieveRequest retrieveRequest = new RetrieveRequest();
                            retrieveRequest.apiVersion = apiVersion.ToDouble();
                            retrieveRequest.singlePackage = true;
                            retrieveRequest.packageNames = new string[] { n.fullName };


                            AsyncResult result = mtDataService.retrieve(retrieveRequest);
                            RetrieveResult retreiveResult = waitForRetrieveCompletion(mtDataService, result);
                            xw.WriteEndElement();
                            break;
                        }
                    }

                }
                        xw.WriteEndElement();
                xw.Flush();

            }
            xw.WriteEndElement();
            xw.Flush();
            return new SqlXml(msObject);

        }

        private static RetrieveResult waitForRetrieveCompletion(MetadataService service, AsyncResult asyncResult) {
            // Wait for the retrieve to complete
            int poll = 0;
            int waitTimeMilliSecs = 1000;
            string asyncResultId = asyncResult.id;
            RetrieveResult result = service.checkRetrieveStatus(asyncResultId, true); 
            while (!result.done)
            {
                System.Threading.Thread.Sleep(waitTimeMilliSecs);
                // Double the wait time for the next iteration
                waitTimeMilliSecs *= 2;
                if (poll++ > 30)
                {
                    throw new Exception("Request timed out.  If this is a large set " +
                    "of metadata components, check that the time allowed " +
                    "by MAX_NUM_POLL_REQUESTS is sufficient.");
                }
                result = service.checkRetrieveStatus(asyncResultId, true);
            };

            return result;
        }

        #endregion
    }
}