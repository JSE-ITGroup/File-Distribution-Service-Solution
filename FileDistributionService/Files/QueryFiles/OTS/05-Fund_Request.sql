



------------------------------------Request Extraction ---------------------------
--Fund Request
---------------------------------------------------------------------------------
use jseproduction
SELECT 
		f.*
		,case
			when RequestStatusENUM=0 then 'Pending'
			when RequestStatusENUM=1 then 'Approved'
			when RequestStatusENUM=2 then 'Rejected'
			else ''
			end as RequestStatus
		
		,(select max(Name) from [Companies] where SystemCompanyId =a.BrokerMemberNumber ) as  BrokerName
from [dbo].[FundRequests]  as f
     join accounts as a on f.Accountnumber= a.Accountnumber
where month(RequestDate) IN(6)  --and RequestStatusENUM=1
     and year(RequestDate) = 2017
order by RequestDate
				
-------------------------------------------------------------------------------------
---Actively Trading ( THere are person that trades at least once per month)
-------------------------------------------------------------------------------------
