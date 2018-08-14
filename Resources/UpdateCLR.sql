Set NoCount On

Exec sp_configure 'Show Advanced',1
Reconfigure
Exec sp_configure 'clr enabled',1
Reconfigure


Alter Database Production Set Trustworthy On
go

SET QUOTED_IDENTIFIER ON
GO

IF object_id('sfCreateUDF') is not null
	Drop Function dbo.sfCreateUDF

IF object_id('sfGetObjectList') is not null
	Drop Function dbo.sfGetObjectList

IF object_id('sfGetRelationships') is not null
	Drop Function dbo.sfGetRelationships

IF object_id('sfGetData') is not null
	Drop Function dbo.sfGetData

IF object_id('sfGetXml') is not null
	Drop Function dbo.sfGetXml

IF object_id('sfEncryptPwd') is not null
	Drop Function dbo.sfEncryptPwd

IF object_id('sfGetCount') is not null
	Drop Function dbo.sfGetCount

IF object_id('sfDelete') is not null
	Drop Function dbo.sfDelete
	
IF object_id('sfUpsert') is not null
	Drop Function dbo.sfUpsert

IF object_id('sfReplicate') is not null
	Drop Function dbo.sfReplicate

IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N'SalesForceLib_Serializer' and is_user_defined = 1)
DROP ASSEMBLY SalesForceLib_Serializer
GO

IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N'SalesForceLib' and is_user_defined = 1)
DROP ASSEMBLY SalesForceLib
GO

CREATE ASSEMBLY SalesForceLib
FROM 'c:\Databases\CLR\SalesForceCLR.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE ASSEMBLY SalesForceLib_Serializer
FROM 'c:\Databases\CLR\SalesForceCLR.XmlSerializers.dll'
WITH PERMISSION_SET = UNSAFE
GO

CREATE FUNCTION dbo.sfDelete(@data xml)
RETURNS  TABLE (
	ObjectType nvarchar(50) NULL,
	Action nvarchar(50) NULL,
	Id nvarchar(18) NULL,
	ExternalId nvarchar(50) NULL,
	Success bit NULL,
	Errors nvarchar(max) NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfDelete
GO

CREATE FUNCTION dbo.sfUpsert(@data xml)
RETURNS  TABLE (
	ObjectType nvarchar(50) NULL,
	Action nvarchar(50) NULL,
	Id nvarchar(18) NULL,
	ExternalId nvarchar(50) NULL,
	Success bit NULL,
	Errors nvarchar(max) NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfUpsert
GO

CREATE FUNCTION dbo.sfGetData(@objectName nvarchar(max), @Criteria nvarchar(max))
RETURNS  TABLE (
	ObjectType nvarchar(50), 
	Id nvarchar(18), 
	Name nvarchar(200), 
	CreatedById nvarchar(50), 
	CreatedDate DateTime, 
	LastModifiedById nvarchar(50), 
	LastModifiedDate DateTime, 
	LastActivityDate DateTime, 
	SystemModstamp datetime,
	Data xml
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfGetData
GO

CREATE FUNCTION dbo.sfCreateUDF(@objectName nvarchar(max))
RETURNS  TABLE (
	Source nvarchar(255)
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfCreateUDF
GO

CREATE FUNCTION dbo.sfGetObjectList()
RETURNS  TABLE (
	Name nvarchar(80), 
	Activateable bit, 
	Createable bit, 
	Custom bit, 
	CustomSetting bit, 
	Deletable bit, 
	DeprecatedAndHidden bit, 
	FeedEnabled bit, 
	KeyPrefix nvarchar(80), 
	Label  nvarchar(80), 
	LabelPlural  nvarchar(80), 
	Layoutable bit, 
	Mergeable bit, 
	Queryable bit, 
	Replicateable bit, 
	Retrieveable bit, 
	Searchable bit, 
	Triggerable bit, 
	Undeletable bit, 
	Updateable bit
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfGetObjectList
GO

CREATE FUNCTION dbo.sfGetRelationships(@objectName nvarchar(max))
RETURNS  TABLE (
	ParentObject nvarchar(100), 
	ChildObject nvarchar(100), 
	ChildField nvarchar(100), 
	RelationshipName nvarchar(100), 
	CascadeDelete bit, 
	RestrictedDelete bit, 
	IsMasterDetail bit
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfGetRelationships
GO

CREATE FUNCTION [dbo].[sfGetXml](@objectName nvarchar(max), @Criteria nvarchar(max))
RETURNS [xml] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfGetXml
GO

CREATE FUNCTION [dbo].[sfGetCount](@objectName nvarchar(max), @Criteria nvarchar(max))
RETURNS int WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfGetCount
GO

CREATE FUNCTION [dbo].[sfEncryptPwd](@pwd nvarchar(max), @key nvarchar(max))
RETURNS nvarchar(max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].EncryptPwd
GO

CREATE FUNCTION [dbo].[sfReplicate](@batchId nvarchar(max), @useTempProcedures bit)
RETURNS [xml] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfReplicate
GO

/********************************************************** Replication *********************************************************/
/********************************************************** Replication *********************************************************/
/********************************************************** Replication *********************************************************/

CREATE FUNCTION [dbo].[_sfReplicate](@salesForceUserID nvarchar(max), @salesForcePassword nvarchar(max), @salesForceToken nvarchar(max), @serviceURL nvarchar(max), @useTempProcedures bit)
RETURNS [xml] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR]._sfReplicate
GO
CREATE FUNCTION [dbo].[sfReplicate](@useTempProcedures bit)
RETURNS [xml] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME SalesForceLib.[Salesforce.CLR].sfReplicate
GO