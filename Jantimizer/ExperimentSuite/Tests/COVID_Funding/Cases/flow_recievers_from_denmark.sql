SELECT Fl.reciever_org_name FROM 
	(transactions AS Ts JOIN flows AS Fl ON 
	 	Fl.reporting_org_id = Ts.reporting_org_id AND
	 	Ts.recipent_country = 'Denmark'
	)