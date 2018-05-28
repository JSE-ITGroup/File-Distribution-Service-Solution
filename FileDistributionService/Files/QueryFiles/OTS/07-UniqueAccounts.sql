


 
 -------Extraction Accounts-----------------------------------------------
  use jseproduction
 -------------------------------------------------------------------------

SELECT
 
         distinct
		 a.AccountNumber
		,a.DateRegistered
		,case 
			when a.AccountStatusEnum =1 then 'Pending' 
			when a.AccountStatusEnum =2 then 'Approved' 
			when a.AccountStatusEnum =3 then 'Regjected' 
			when a.AccountStatusEnum =4 then 'Disabled' 
			when a.AccountStatusEnum =5 then 'Locked' 
			else null
			end as Status
		,a.Balance
		,(SELECT MAX(NAME) FROM COMPANIES WHERE ID=a.CompanyID) as Broker
		,a.AccountType
		,a.AccountName as Name
		,c.*
from [dbo].Accounts as a
 
     JOIN (
	SELECT Customers.Id AS CustomerID
      ,[SSN]
      ,ISNULL(LTRIM(RTRIM([Title])),'') + ' ' +
       ISNULL(LTRIM(RTRIM([FirstName])),'') + ' ' + 
       ISNULL(LTRIM(RTRIM([MiddleName])),'') + ' ' +
       ISNULL(LTRIM(RTRIM([LastName])),'') AS Name
      ,[Email]
      ,CAST([DateOfBirth] AS Date) as  [DateOfBirth]
      ,ISNULL(LTRIM(RTRIM([MailingAddress1])),'') + ' ' +
       ISNULL(LTRIM(RTRIM([MailingAddress2])),'') + ' ' +
       ISNULL(LTRIM(RTRIM([MailingAddress3])),'')  AS MailingAddress
      ,ISNULL([PermanentAddress1],'') + ' ' +
       ISNULL([PermanentAddress2],'') + ' '+
       ISNULL([PermanentAddress3],'')  AS PermanentAddress
	   ,CASE 
			WHEN (PermanentAddress3 LIKE '%cath%'  OR [MailingAddress3] LIKE '%cath%') AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'St. Catherine'
			WHEN (PermanentAddress3  LIKE '%ann%'  OR [MailingAddress3] LIKE '%ann%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'St. Ann'
			WHEN (PermanentAddress3  LIKE '%eliz%'  OR [MailingAddress3] LIKE '%black%'  ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'St. Elizabeth'
			WHEN (PermanentAddress3  LIKE '%james%'  OR [MailingAddress3] LIKE '%monte%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'St. James'
			WHEN (PermanentAddress3  LIKE '%mary%'  OR [MailingAddress3] LIKE '%mary%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'St. Mary'
			WHEN (PermanentAddress3  LIKE '%clar%'  OR [MailingAddress3] LIKE '%may%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'Clarendon'
			WHEN (PermanentAddress3  LIKE '%tre%'  OR [MailingAddress3] LIKE '%fal%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6' THEN 'Trelawney'
			WHEN (PermanentAddress3  LIKE '%thomas%'  OR [MailingAddress3] LIKE '%thomas%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6' THEN 'St. Thomas'
			WHEN (PermanentAddress3  LIKE '%andre%'  OR [MailingAddress3] LIKE '%andre%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6' THEN 'St. Andrew'
			WHEN (PermanentAddress3  LIKE '%Hanover%'  OR [MailingAddress3] LIKE '%Westmore%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6' THEN  'Westmoreland'
			WHEN (PermanentAddress3  LIKE '%manc%'  OR [MailingAddress3] LIKE '%mand%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6' THEN 'Manchester'
			WHEN (PermanentAddress3  LIKE '%king%'  OR [MailingAddress3] LIKE '%and%' ) AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6' THEN 'Kingston/St. Andrew'
			ELSE ''
			END AS Parish
      ,[Telephone1]
      ,[Telephone2]
      ,[Telephone3]
      ,[IsActive]
	  --,DateRegistered
	  ,CountryId
      ,CountryName
      ,[IdNumber]
      ,[IdTypeENUM]
      ,[IdIssueDate]
      ,[IdExpiryDate]
  FROM [dbo].[Customers]
  LEFT JOIN Countries ON Customers.CountryId=Countries.Id

	      ) as c on c.CustomerID= a.CustomerID
		    
Where month(a.DateRegistered)  in(6)
     and  year(a.DateRegistered)=2017

/*
-----------UNique Accounts-----------
  USE STAGING
---------------------------------------------------

select
  distinct *
,case 
     when (select Account_Reference  from [dbo].[Accounts] where Account_Reference =a.AccountNumber) is null then 'New Account'
else 'Existing Account' end as AccountNumber
      from [dbo].OTS1 as a
WHERE MONTH(DateRegistered) = 11
    and Year(DateRegistered) = 2016
	  
	*/  



		