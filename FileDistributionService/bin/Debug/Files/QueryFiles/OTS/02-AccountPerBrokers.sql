use jseproduction

  -----------------------------------------------------------------------------------

  --Number of Accounts Approved Per Broker by Month

  --------------------------------------------------------------------------------------
  use jseproduction
  SELECT
		COUNT(distinct (AccountNumber)) NumofAccountApproved
		,BrokerMemberNumber
		,case 
			when AccountStatusEnum =1 then 'Pending'
			when AccountStatusEnum =2 then 'Approved'
			when AccountStatusEnum =3 then 'Rejected'
			when AccountStatusEnum =4 then 'Disabled'
			when AccountStatusEnum =5 then 'Locked'
			end as Status
		,(select max(Name) from [Companies] where SystemCompanyId =BrokerMemberNumber ) as  BrokerName
		,Datename(mm,DateRegistered) as Month
		,Month(DateRegistered) as MonthNumber
		,Year(DateRegistered) as Year 
  FROM Accounts
  Where Month(DateRegistered) in (5) and 
        year(DateRegistered)  in  (2017)
  GROUP BY  BrokerMemberNumber
			,Datename(mm,DateRegistered) 
			,Month(DateRegistered)
			,Year(DateRegistered)  
			,case 
			when AccountStatusEnum =1 then 'Pending'
			when AccountStatusEnum =2 then 'Approved'
			when AccountStatusEnum =3 then 'Rejected'
			when AccountStatusEnum =4 then 'Disabled'
			when AccountStatusEnum =5 then 'Locked'
			end 
  ORDER BY Year,MonthNumber,BrokerMemberNumber


