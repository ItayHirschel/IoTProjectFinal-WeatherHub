import logging
import os
import azure.functions as func
from azure.data.tables import TableClient, EntityProperty


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    connection_string = os.getenv("AzureWebJobsStorage")
    T_name = os.getenv("Accounts_Table_Name")
    
    try:
        name = req.params.get('AccountName')
        password = req.params.get('Password')

        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')

            filter = f"PartitionKey eq '{name}'"
            entities = table.query_entities(filter)
            
            temp = 0
            for ent in entities:
                entity_p = ent
                temp += 1
            
            if (0 == temp):
                return func.HttpResponse(f"Username {name} doesn't exist", status_code=500)
            elif (1 < temp):
                return func.HttpResponse(f"Username {name} has duplicates", status_code=500)
            
            if entity_p["Password"] != password:
                return func.HttpResponse(f"Password doesn't match", status_code=500)
            
            return func.HttpResponse(entity_p["RowKey"], status_code=200)
        
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)

