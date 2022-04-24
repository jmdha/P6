SELECT Fl.reporting_org_id, Fl.total_money FROM 
	(flows AS Fl JOIN transactions AS Ts ON 
	 	--Ts.month > '2021-01-01' AND 
	 	Fl.reporting_org_id = Ts.reporting_org_id
	)