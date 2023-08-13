import logging

import azure.functions as func
from azure.data.tables import TableClient, EntityProperty
import os
import random, string


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    connection_string = os.getenv("AzureWebJobsStorage")
    T_name = os.getenv("Accounts_Table_Name")
    
    try:
        name = req.params.get('AccountName')
        password = req.params.get('Password')
        secret_key = name + "$" + generate_random_key(10)

        with TableClient.from_connection_string(connection_string, table_name=T_name) as table:
            logging.info('Table opened')

            if "$" in name:
                return func.HttpResponse(f"Username {name} cannot contain '$'", status_code=500)
            
            filter = f"PartitionKey eq '{name}'"
            entities = table.query_entities(filter)
            
            
            for ent in entities:
                return func.HttpResponse(f"Username {name} already in use", status_code=500)


            new_account = {"PartitionKey" : name, "RowKey" : secret_key, "Password" : password}
            
            table.create_entity(new_account)

            return func.HttpResponse(f"Username {name} created", status_code=200)
    
    except:
        return func.HttpResponse(f"Something went wrong", status_code=500)


def generate_random_key(length):
    letters = string.ascii_lowercase
    res = ''.join(random.choice(letters) for i in range(length))
    return res
