----------------------------------------------------------------
  --Account With Funds.
-----------------------------------------------------------------
use jseproduction

  SELECT 
		a.AccountNumber
		,a.Balance
		,BrokerMemberNumber
		,c.Name
		,a.AccountName
		,a.AvailableBalance
		,DateRegistered
		, CONVERT(DATE,GETDATE()-1) AS  ASAT
		FROM ACCOUNTS a
		JOIN Companies c on  c.Id = a.CompanyId
  WHERE a.AvailableBalance > 0
  AND YEAR(DateRegistered) = 2017 AND MONTH(DateRegistered) = 6
