
use jseproduction

DECLARE @YEAR INT,@Month INT,@Period INT

SET @Year=2017;
SET @Month=6;
SET @Period = 5;


WITH CTE
AS
(
select 
		count(*) as NumOfOrders,
		a.AccountNumber,
		--o.CustomerID,
		Year(t.CreatedDate) as Year,
		--Month(t.CreatedDate) as MonthNumber,
		datename(mm,t.CreatedDate) as Month--,
		--Month(t.CreatedDate) as Month
from [dbo].[Orders] as o
		JOIN trades t on t.OrderId =o.Id
		join Accounts as a on a.Id=AccountId 
group by 
	    a.AccountNumber,
		o.accountId,
		--o.CustomerID,
		Year(t.CreatedDate) ,
		--Month(t.CreatedDate) ,
		datename(mm,t.CreatedDate),
		Month(t.CreatedDate) 
--order by  AccountID
)
,CTE2
as
(
SELECT 
	* FROM CTE
PIVOT(sum(NumOfOrders) FOR Month in ([January],[February],[March],[April],[May],[June],[July],[August],[September],[October],[November],[December])) as RecurrTrades
Where year = @Year
)


select 
    cu.FirstName + '   ' + cu.LastName as FullName
   ,c.Name as BrokereageHouse
   ,cte2.*
   
from cte2
  JOIN accounts a  on cte2.AccountNumber= a.AccountNumber
  JOIN companies c on a.BrokerMemberNumber=c.SystemCompanyid
  JOIN customers as  cu on cu.Id= a.CustomerId


  

