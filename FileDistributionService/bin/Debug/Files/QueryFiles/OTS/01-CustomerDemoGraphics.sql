


use jseproduction

 --Customer Demogroahics
 SELECT
	*,year(AccountCreatedDate) as Year
	,month(AccountCreatedDate) as MonthNumber
	,datename(mm,AccountCreatedDate) as Month

 FROM (
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
       ISNULL([PermanentAddress2],'') + ' ' +
       ISNULL([PermanentAddress3],'')  AS PermanentAddress
	   ,CASE 
			WHEN (PermanentAddress3 LIKE '%cath%'  OR [MailingAddress3] LIKE '%cath%') AND CountryId='E7681F0D-7C48-4E43-945B-C33F4E30D1D6'  THEN 'St. Catherine'
			WHEN PermanentAddress3  LIKE '%ann%'  OR [MailingAddress3] LIKE '%ann%' THEN 'St. Ann'
			WHEN PermanentAddress3  LIKE '%eliz%'  OR [MailingAddress3] LIKE '%eliz%' THEN 'St. Elizabeth'
			WHEN PermanentAddress3  LIKE '%james%'  OR [MailingAddress3] LIKE '%james%' THEN 'St. James'
			WHEN PermanentAddress3  LIKE '%mary%'  OR [MailingAddress3] LIKE '%mary%' THEN 'St. Mary'
			WHEN PermanentAddress3  LIKE '%cath%'  OR [MailingAddress3] LIKE '%cath%' THEN 'St. Catherine'

			WHEN PermanentAddress3  LIKE '%Thomas%'  OR [MailingAddress3] LIKE '%Thomas%' THEN 'St. Thomas'
			WHEN PermanentAddress3  LIKE '%Habour%'  OR [MailingAddress3] LIKE '%Habour%' THEN 'St. Catherine'
			WHEN PermanentAddress3  LIKE '%Clarendon%'  OR [MailingAddress3] LIKE '%Clarendon%' THEN 'Clarendon'
			WHEN PermanentAddress3  LIKE '%manc%'  OR [MailingAddress3] LIKE '%manc%' THEN 'Manchester'
			WHEN PermanentAddress3  LIKE '%Andrew%'  OR [MailingAddress3] LIKE '%Andrew%' THEN 'St. Andrew'
			WHEN PermanentAddress3  LIKE '%Trela%'  OR [MailingAddress3] LIKE '%Trela%' THEN 'St. Catherine'
			WHEN PermanentAddress3  LIKE '%catherine%'  OR [MailingAddress3] LIKE '%catherine%' THEN 'Trelawney'
			WHEN PermanentAddress3  LIKE '%westmore%'  OR [MailingAddress3] LIKE '%catherine%' THEN 'Westmoreland'
			ELSE ''
			END AS Parish
      ,[Telephone1]
      ,[Telephone2]
      ,[Telephone3]
      ,[Customers].[IsActive]
	  --,DateRegistered
	  ,CountryId
      ,CountryName
      ,[IdNumber]
      ,[IdTypeENUM]
      ,[IdIssueDate]
      ,[IdExpiryDate]	  
	  ,(select  min(AuditTime)
			from [dbo].[AuditHeaders] 
				where AuditTypeEnum in (5,10)
				and UserName=l.UserName)
				as AccountCreatedDate
		,l.lastLoginDate
  FROM [dbo].[Customers]
  LEFT JOIN Countries  ON Customers.CountryId=Countries.Id
  LEFT JOIN logins l on l.id= customers.id
  ) as Cust
    where Month(AccountCreatedDate) IN(5) 
	and year(AccountCreatedDate) = 2017 --='4EA6AB51-3C72-404C-90D3-CC696A29BC26'
	--and lastLoginDate='1753-01-01 00:00:00.000'
  order by MonthNumber

