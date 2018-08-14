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

CREATE TABLE [dbo].[SalesForceAPI](
	[SalesForceUserID] [varchar](50) NULL,
	[SalesForcePassword] [varchar](50) NULL,
	[SalesForceToken] [varchar](50) NULL,
	[Mode] [varchar](10) NULL,
	[ServiceUrl] [varchar](255) NULL,
	[DataGatewayServiceUrl]	varchar(255) NULL
) ON [PRIMARY]

GO
Insert [SalesForceAPI] Values ('[APIUser]','[SalesForcePassword]',null,'Dev','https://test.salesforce.com/services/Soap/u/40.0')
Insert [SalesForceAPI] Values ('[APIUser]','[SalesForcePassword]',null,'Prod','https://login.salesforce.com/services/Soap/u/40.0')


CREATE TABLE [dbo].[SalesForceAPIMode](
	[Mode] [varchar](10) NULL
) ON [PRIMARY]

GO

Insert [SalesForceAPIMode] Values ('Dev')
Go


CREATE TABLE [dbo].[SalesForceReplObjects](
	[ObjectName] [nvarchar](80) NOT NULL,
	[BatchId] uniqueidentifier NULL,
	[BatchSize] [int] NOT NULL,
	[Resynch] [bit] NOT NULL,
	[ReplicateDeletes] [bit] NOT NULL,
	[LastSynchDate] [datetime] NULL,
	[PreviousModifiedDate] [datetime] NULL,
	[LastId] varchar(18) NULL,
	[LastModifiedDate] [datetime] NULL,
	[SystemModstamp] [datetime] NULL,
	[ObjectCount] [int] NULL,
	[ChangeCount] [int] NULL,
	[SchemaChanges] [xml] NULL,
	[Changes] [xml] NULL,
 CONSTRAINT [PK_SalesForceReplicatedObjects] PRIMARY KEY CLUSTERED 
(
	[ObjectName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

Insert [SalesForceReplObjects]([ObjectName],[BatchSize],[Resynch],[ReplicateDeletes]) Values ('Account', 100, 0, 0)
Insert [SalesForceReplObjects]([ObjectName],[BatchSize],[Resynch],[ReplicateDeletes]) Values ('Contact', 100, 0, 0)

Go

Create procedure [dbo].[rpl_Replicate_SalesforceObjects] (@UserTemp bit = 1, @SingleObjectName varchar(255) = null, @BatchSize int = -1)
As

-- Exec rpl_Replicate_SalesforceObjects 0, Account, 33

Set NoCount On

Declare @Xml xml,
		@OldBatchSize int = -1

	If @BatchSize != -1 and @SingleObjectName is not null Begin
		Select @OldBatchSize = BatchSize From SalesForceReplObjects Where ObjectName = @SingleObjectName
		Update	SalesForceReplObjects 
		Set		BatchSize = @BatchSize
		Where	ObjectName = @SingleObjectName
	End


	Declare @BatchId uniqueidentifier = NewId()
	
	Update	SalesForceReplObjects
	Set		LastSynchDate = getDate(),
			BatchId = @BatchId
	Where	ObjectName in (
				Select Top 10 ObjectName From SalesForceReplObjects Where IsNull(@SingleObjectName , ObjectName) = ObjectName Order By LastSynchDate 
			)

	Begin Try
		Set @Xml = dbo.sfReplicate(@BatchId, @UserTemp)

		Update	SalesForceReplObjects 
		Set		BatchSize = @OldBatchSize
		Where	ObjectName = @SingleObjectName
	End Try
	Begin Catch 
		Update	SalesForceReplObjects 
		Set		BatchSize = @OldBatchSize
		Where	ObjectName = @SingleObjectName

		Select	ERROR_NUMBER() AS ErrorNumber,  
				ERROR_SEVERITY() AS ErrorSeverity,  
				ERROR_STATE() AS ErrorState,  
				ERROR_PROCEDURE() AS ErrorProcedure,  
				ERROR_LINE() AS ErrorLine,  
				ERROR_MESSAGE() AS ErrorMessage
		
		Return		  
	End Catch

	If (@UserTemp = 0) Select @Xml

	Create Table #Results (
		ObjectName varchar(80),
		LastId varchar(18),
		LastModifiedDate datetime,
		SystemModstamp datetime,
		ObjectCount int,
		ChangeCount int
	)
	
	Declare @ObjectName varchar(80),
			@ChangeCount int,
			@TableSchema nvarchar(max),
			@ProcedureScript nvarchar(max),
			@Prefix varchar(5),
			@SchemaChanges xml,
			@Data xml,
			@Sql varchar(max),
			@ProcedureName varchar(50)

	Select	Rowset.Col.value('@ObjectName','varchar(80)') as ObjectName
			,Rowset.Col.value('@LastId','varchar(18)') as LastId
			,Rowset.Col.value('@LastModifiedDate','datetime') as LastModifiedDate
			,Rowset.Col.value('@SystemModstamp','datetime') as SystemModstamp
			,Rowset.Col.value('SchemaChanges[1]/Sql[1]/text()[1]','nvarchar(max)') as TableSchema
			,Rowset.Col.value('Procedure[1]/text()[1]','nvarchar(max)') as ProcedureScript
			,Rowset.Col.query('SchemaChanges') as SchemaChanges
			,Rowset.Col.query('Data') as Changes
			,Rowset.Col.query('Error') as Errors
	Into	#ChangeSet
	From	@Xml.nodes('/Replication/ChangeSet') As Rowset(Col)	

	Declare sc cursor local for
		Select	ObjectName, 
				TableSchema,
				ProcedureScript,
				SchemaChanges,
				Changes
		From	#ChangeSet c

	Open sc

	Fetch Next From sc Into @ObjectName, @TableSchema, @ProcedureScript, @SchemaChanges, @Data

	While @@Fetch_Status = 0 Begin
		Begin Try

			If @TableSchema is not null Begin
				Exec (@TableSchema)	
			End

			If @UserTemp = 1 Begin
				Set @ProcedureName = Replace(convert(varchar(50),newID()),'-','')
				Set @Prefix = '#'
			End
			Else Begin
				Set @ProcedureName = @ObjectName
				Set @Prefix = 'repl_'
				Set @Sql ='If Object_Id(''' + @Prefix + @ProcedureName +''') is not null Drop Procedure ' + @Prefix + @ProcedureName
				Print (@Sql)
				Exec (@Sql)
			End

			Set @ProcedureScript = Replace(@ProcedureScript,'<ProcedureName>',@ProcedureName)

			Set @Sql = @Prefix + @ProcedureName

			If (@UserTemp = 0) Print (@ProcedureScript)
			Exec (@ProcedureScript)

			Insert #Results
			Exec @Sql @Data

			If (@UserTemp = 1) Begin
				Set @Sql ='Drop Procedure ' + @Prefix + @ProcedureName
				Exec (@Sql)
			End

		End Try 
		Begin Catch
			Update	SalesForceReplObjects
			Set		LastSynchDate = getDate(),
					Changes = Convert(Xml,'<Error>' + Error_Message() + '</Error>')
			Where	ObjectName = @ObjectName
			
			Select	ERROR_NUMBER() AS ErrorNumber,  
					ERROR_SEVERITY() AS ErrorSeverity,  
					ERROR_STATE() AS ErrorState,  
					ERROR_PROCEDURE() AS ErrorProcedure,  
					ERROR_LINE() AS ErrorLine,  
					ERROR_MESSAGE() AS ErrorMessage
					
		End Catch

		Fetch Next From sc Into @ObjectName, @TableSchema, @ProcedureScript, @SchemaChanges, @Data
	End

	Close sc
	Deallocate sc

	Update	SalesForceReplObjects
	Set		LastSynchDate = getDate(),
			Changes = chg.Errors
	From	#ChangeSet chg
	Where	SalesForceReplObjects.ObjectName = chg.ObjectName and
			Convert(Varchar(Max),Errors) != ''

	Update	SalesForceReplObjects
	Set		Resynch = 0,
			LastSynchDate = getDate(),
			PreviousModifiedDate = IsNull(SalesForceReplObjects.LastModifiedDate,'2000-01-01'),
			LastId = IsNull(rpl.LastId,''),
			LastModifiedDate = rpl.LastModifiedDate,
			SystemModstamp = rpl.SystemModstamp,
			Changes = chg.Changes,
			SchemaChanges = chg.SchemaChanges,
			ChangeCount = rpl.ChangeCount,
			ObjectCount = rpl.ObjectCount
	From	#Results rpl,
			#ChangeSet chg
	Where	SalesForceReplObjects.ObjectName = rpl.ObjectName and 
			chg.ObjectName = rpl.ObjectName
	
	Select 	[ObjectName],
			[BatchSize],
			[Resynch],
			[ReplicateDeletes],
			[LastSynchDate],
			[PreviousModifiedDate],
			[LastId],
			[LastModifiedDate],
			[SystemModstamp],
			[ObjectCount],
			[ChangeCount] 
	From	SalesForceReplObjects
	Where	BatchId = @BatchId

Go

Create procedure rpl_AddObject (
			@ObjectName varchar(100))
As

	If (Not Exists (Select 1 From SalesForceReplObjects Where ObjectName = @ObjectName )) Begin
		Insert	SalesForceReplObjects (
				ObjectName, 
				BatchSize, 
				Resynch, 
				ReplicateDeletes, 
				LastSynchDate, 
				PreviousModifiedDate, 
				LastId, 
				LastModifiedDate, 
				SystemModstamp)
		Values	(@ObjectName,
				100,
				1,
				0,
				'2001-01-01',
				'2001-01-01',
				'',
				'2001-01-01',
				'2001-01-01')

		exec rpl_Replicate_SalesforceObjects 1, @ObjectName, 5000
	End
Go	 


Print 'Be sure to set the credentials in the SalesForceAPI table.'
Print 'The API Mode is defaulted to dev. If you wish to change the mode edit the record in the SalesForceAPIMode table.'

Print ''
Print 'Add any standard or custom object from Salesforce you want replicated in to the SalesForceReplObjects table.'
Print 'The max BatchSize is 2500 if you enter a number larger it will default to 2500 instead. A value of zero will become a batch size of 2500.'
